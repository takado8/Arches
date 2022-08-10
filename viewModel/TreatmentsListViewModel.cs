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
                    parentItem.Selected += TreeViewItem_Selected;
                    if (item.treatments != null)
                    {
                        foreach (var treatment in item.treatments)
                        {
                            var childItem = new TreeViewItem() { Header = makeTextBlock(treatment.description) };
                            childItem.PreviewMouseLeftButtonDown += ChildItem_PreviewMouseLeftButtonDown;
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
            parentItem.Selected += TreeViewItem_Selected;
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

        private TextBlock makeTextBlock(string descritpion)
        {
            return new TextBlock() { Text = descritpion, TextWrapping = TextWrapping.Wrap, Width = treeViewWidth - 15 };
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if (tvi == null || e.Handled) return;
            tvi.IsExpanded = !tvi.IsExpanded;
            e.Handled = true;
            tvi.IsSelected = false;
        }

        private void ChildItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item.IsSelected = true;
            item.Background = Brushes.LightBlue;
        }
    }
}
