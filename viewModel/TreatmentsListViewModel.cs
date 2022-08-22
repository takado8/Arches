using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Arches.model;
using Arches.service;
using System.Collections.Generic;
using System.Windows.Input;

namespace Arches.viewModel
{
    internal class TreatmentsListViewModel
    {
        public ObservableCollection<TreeViewItem> items { get; } = new();
        private SQLiteDataStorage sqliteDataStorage = new();
        private ITreeViewItemSelected treeViewItemSelected;
        public TreeViewItem? selectedParentItem = null;
        private double treeViewWidth;
        public bool parentItemSelectedCallbackLocked = false;

        public TreatmentsListViewModel(double treeViewWidth, ITreeViewItemSelected treeViewItemSelected)
        {
            this.treeViewWidth = treeViewWidth;
            this.treeViewItemSelected = treeViewItemSelected;
            var itemsFromDb = sqliteDataStorage.getItems();
            if (itemsFromDb != null)
            {
                foreach (var item in itemsFromDb)
                {
                    var txtBlock = makeTextBlock(item.header);
                    TreeViewItem parentItem = new() { Header = txtBlock };
                    parentItem.Selected += ParentItemSelected;
                    if (item.treatments != null)
                    {
                        foreach (var treatment in item.treatments)
                        {
                            var childItem = new TreeViewItem() { Header = makeTextBlock(treatment.description) };
                            
                            childItem.PreviewMouseLeftButtonDown += ChildItem_PreviewMouseLeftButtonDown;
                            childItem.Selected += ChildItem_Selected;
                            parentItem.Items.Add(childItem);
                        }
                    }
                    items.Add(parentItem);
                }
            }
        }

        public bool addItem(string newTreatmentDescription)
        {
            if (string.IsNullOrEmpty(newTreatmentDescription) || string.Equals(Constants.textBoxPlaceholder, newTreatmentDescription))
            {
                return false;
            }
            foreach (TreeViewItem item in items)
            {
                if (((TextBlock)((Border)item.Header).Child).Text.Equals(newTreatmentDescription))
                {
                    return false;
                }
            }
            var txtBlock = makeTextBlock(newTreatmentDescription);
            TreeViewItem parentItem = new() { Header = txtBlock };
            parentItem.Selected += ParentItemSelected;
            items.Add(parentItem);
            TreatmentCategory treatment = new(newTreatmentDescription);
            sqliteDataStorage.addTreatmentCategoryAsync(treatment);
            return true;
        }

        public bool updateItem(TreeViewItem parentItem, string newChildItemDescription)
        {
            if (parentItem != null && !string.IsNullOrWhiteSpace(newChildItemDescription))
            {
                var parentDescription = ((TextBlock)((Border)parentItem.Header).Child).Text;
                TreatmentCategory? fromDb = sqliteDataStorage.getItem(parentDescription);
                if (fromDb != null)
                { 
                    TreeViewItem childItem = new() { Header = makeTextBlock(newChildItemDescription) };
                    childItem.PreviewMouseLeftButtonDown += ChildItem_PreviewMouseLeftButtonDown;
                    parentItem.Items.Add(childItem);

                    if (fromDb.treatments == null)
                    {
                        fromDb.treatments = new List<Treatment>();
                    }
                    Treatment newTreatment = new Treatment(newChildItemDescription) { treatmentCategoryId = fromDb.Id };
                    sqliteDataStorage.addTreatmentAsync(newTreatment);
                    return true;
                }
            }
            return false;
        }

        public void deleteTreatmentCategoryItem(TreeViewItem itemToDelete)
        {
            if (itemToDelete != null)
            {
                parentItemSelectedCallbackLocked = true;
                if (selectedParentItem == itemToDelete)
                {
                    selectedParentItem = null;
                }
                var description = ((TextBlock)((Border)itemToDelete.Header).Child).Text;
                items.Remove(itemToDelete);
                parentItemSelectedCallbackLocked = false;
                sqliteDataStorage.deleteTreatmentCategoryAsync(description);
            }      
        }

        public void deleteTreatmentItem(TreeViewItem itemToDelete)
        {
            if (itemToDelete != null)
            {
                var description = ((TextBlock)((Border)itemToDelete.Header).Child).Text;
                TreeViewItem parent = itemToDelete.Parent as TreeViewItem;
                parent.Items.Remove(itemToDelete);
                sqliteDataStorage.deleteTreatmentAsync(description);
            }
        }

        private Border makeTextBlock(string descritpion)
        {
            Border border = new Border() { BorderBrush = Constants.getUnselectedItemBrush(), BorderThickness=new Thickness(1),
                CornerRadius = new CornerRadius(15), Padding=new Thickness(5,5,5,9), VerticalAlignment=VerticalAlignment.Center,
            HorizontalAlignment=HorizontalAlignment.Center};
            border.Child = new TextBlock() { Text = descritpion, TextWrapping = TextWrapping.Wrap, Width = treeViewWidth - 72,
                HorizontalAlignment=HorizontalAlignment.Center, VerticalAlignment=VerticalAlignment.Center, TextAlignment=TextAlignment.Left };
            return border;
        }

        private void ParentItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item == null || e.Handled || parentItemSelectedCallbackLocked) return;
            item.IsExpanded = !item.IsExpanded;
            e.Handled = true;
            item.IsSelected = false;
            if (selectedParentItem != null)
            {
                ((Border)selectedParentItem.Header).Background = Constants.getUnselectedItemBrush();
            }
            ((Border)item.Header).Background = Constants.getSelectedCategoryItemBrush();
            selectedParentItem = item;
        }
        private void ChildItem_Selected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void ChildItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item.IsSelected = false;
            Border itemBorder = (Border)item.Header;
            if (itemBorder.Background == Constants.getSelectedItemBrush())
            {
                itemBorder.Background = Constants.getUnselectedItemBrush();
            }
            else
            {
                itemBorder.Background = Constants.getSelectedItemBrush();
            }
            treeViewItemSelected.treeViewChildItemSelected(item);
            e.Handled = true;
        }
        public void TreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { 
            if (selectedParentItem != null)
            {
                ((Border)selectedParentItem.Header).Background = Constants.getUnselectedItemBrush();
                selectedParentItem.IsSelected = false;
                selectedParentItem = null;
            }
        }
    }
}
