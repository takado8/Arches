using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using Arches.model;
using Arches.service;

namespace Arches.viewModel
{
    internal class TreatmentsListViewModel
    {
        public ObservableCollection<TextBlock> items { get; } = new();
        private SQLiteDataStorage sqliteDataStorage = new();

        public TreatmentsListViewModel()
        {
            var itemsFromDb = sqliteDataStorage.getItems();
            foreach (var item in itemsFromDb)
            {
                var txtBlock = new TextBlock() { Text = item.description, TextWrapping = TextWrapping.Wrap };
                items.Add(txtBlock);
            }
        }

        public bool addItem(string newTreatmentDescription)
        {
            if (string.IsNullOrEmpty(newTreatmentDescription) || string.Equals(Constants.textBoxPlaceholder, newTreatmentDescription))
            {
                return false;
            }
            foreach (TextBlock item in items)
            {
                if (item.Text.Equals(newTreatmentDescription))
                {
                    return false;
                }
            }
            var txtBlock = new TextBlock() { Text = newTreatmentDescription, TextWrapping = TextWrapping.Wrap }; 
            items.Add(txtBlock);
            Treatment treatment = new(newTreatmentDescription);
            sqliteDataStorage.addItemAsync(treatment);
            return true;
        }

        public void deleteItems(System.Collections.IList itemsToDelete)
        {
            while(itemsToDelete.Count > 0)
            {
                var item = itemsToDelete[0];
                if (item != null)
                {
                    TextBlock txtBlock = (TextBlock)item;
                    items.Remove(txtBlock);
                    sqliteDataStorage.delItemAsync(txtBlock.Text);
                }
            }
        }
    }
}
