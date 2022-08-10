using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Arches.model;
using Arches.service;
using System.Collections.Generic;

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
                    //MessageBox.Show(item.treatments.Count.ToString());
                    var txtBlock = new TextBlock() { Text = item.header, TextWrapping = TextWrapping.Wrap, Width = treeViewWidth - 15 };
                    TreeViewItem parentItem = new() { Header = txtBlock };
                    if (item.treatments != null)
                    {
                        foreach (var treatment in item.treatments)
                        {
                            parentItem.Items.Add(new TreeViewItem() { Header = treatment.description });
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
            items.Add(parentItem);
            TreatmentCategory treatment = new(newTreatmentDescription);
            sqliteDataStorage.addTreatmentCategoryAsync(treatment);
            return true;
        }
        
        public async void updateItem(TreeViewItem parentItem, string newChildItemDescription)
        {
            if (parentItem != null && !string.IsNullOrWhiteSpace(newChildItemDescription))
            {
                TreeViewItem childItem = new() { Header = makeTextBlock(newChildItemDescription) };
                parentItem.Items.Add(childItem);
                var parentDescription = ((TextBlock)parentItem.Header).Text;
                TreatmentCategory fromDb = sqliteDataStorage.getItem(parentDescription);
                if (fromDb.treatments == null)
                {
                    fromDb.treatments = new List<Treatment>();
                }
                Treatment newTreatment = new Treatment(newChildItemDescription) { treatmentCategoryId = fromDb.Id };
                await sqliteDataStorage.addTreatmentAsync(newTreatment);
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
    }
}
