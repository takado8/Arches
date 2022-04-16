using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls;


namespace Arches.viewModel
{
    internal class TreatmentsListViewModel
    {
        public ObservableCollection<TextBlock> items { get; } = new ObservableCollection<TextBlock>();

        public TreatmentsListViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                var txtBlock = new TextBlock() { Text = "list item " + i.ToString(), TextWrapping = System.Windows.TextWrapping.Wrap };
                items.Add(txtBlock);
            }
        }

        public bool addItem(string newItem)
        {
            if (string.IsNullOrEmpty(newItem) || string.Equals(Constants.textBoxPlaceholder, newItem))
            {
                return false;
            }
            foreach (TextBlock item in items)
            {
                if (item.Text == newItem)
                {
                    return false;
                }
            }
            var txtBlock = new TextBlock() { Text = newItem, TextWrapping = System.Windows.TextWrapping.Wrap }; 
            items.Add(txtBlock);
            return true;
        }

        public void deleteItem(Object item)
        {
            if (item != null)
            {
                //var selectedString = item.ToString();
                TextBlock txtBlock = (TextBlock)item;
                items.Remove(txtBlock);
            }
        }
    }
}
