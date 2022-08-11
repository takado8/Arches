using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Arches.model;
using Arches.service;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace Arches.viewModel
{
    internal class TreatmentsListViewModel
    {
        public ObservableCollection<TreeViewItem> items { get; } = new();
        private SQLiteDataStorage sqliteDataStorage = new();
        private TreeViewItem? selectedParentItem = null;
        private double treeViewWidth;

        public TreatmentsListViewModel(double treeViewWidth)
        {
            this.treeViewWidth = treeViewWidth;
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
                if (item.Header.Equals(newTreatmentDescription))
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

        public async void updateItem(TreeViewItem parentItem, string newChildItemDescription)
        {
            if (parentItem != null && !string.IsNullOrWhiteSpace(newChildItemDescription))
            {
                var parentDescription = ((TextBlock)parentItem.Header).Text;
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
                    await sqliteDataStorage.addTreatmentAsync(newTreatment);
                }
            }
        }

        public void deleteItem(TreeViewItem itemToDelete)
        {
            if (itemToDelete != null)
            {
                var description = ((TextBlock)itemToDelete.Header).Text;
                items.Remove(itemToDelete);
                sqliteDataStorage.delItemAsync(description);
            }      
        }

        private Border makeTextBlock(string descritpion)
        {
            //< Border Margin = "5" Padding = "5" BorderThickness = "1" BorderBrush = "Red" Background = "AntiqueWhite" CornerRadius = "10" >
            Border border = new Border() { BorderBrush = Brushes.Transparent, BorderThickness=new Thickness(1),
                CornerRadius = new CornerRadius(15), Padding=new Thickness(5), Margin=new Thickness(0,0,10,0) };
            border.Child = new TextBlock() { Text = descritpion, TextWrapping = TextWrapping.Wrap, Width = treeViewWidth - 60 };
            return border;
        }

        private void ParentItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item == null || e.Handled) return;
            item.IsExpanded = !item.IsExpanded;
            e.Handled = true;
            item.IsSelected = false;
            if (selectedParentItem != null)
            {
                //selectedParentItem.Background = item.Background;
                ((Border)selectedParentItem.Header).Background = item.Background;
            }
            ((Border)item.Header).Background = Brushes.AliceBlue;
            selectedParentItem = item;
            //MessageBox.Show(tvi.Header.ToString());
            //MessageBox.Show(((TextBlock)item.Header).Text.ToString());

        }
        private void ChildItem_Selected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void ChildItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item.IsSelected = false;
            ((Border)item.Header).Background = Brushes.LightBlue;
            //MessageBox.Show(((TextBlock)item.Header).Text.ToString());
            e.Handled = true;

        }
        public void TreeView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }

      

    }
}
