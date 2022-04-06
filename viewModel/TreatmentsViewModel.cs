using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace Arches.viewModel
{
    internal class TreatmentsViewModel
    {
        public ObservableCollection<string> items { get; } = new ObservableCollection<string>();

        public TreatmentsViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                items.Add("list item " + i.ToString());
            }
        }

        public bool addItem(string newItem)
        {
            if (!string.IsNullOrEmpty(newItem) && !items.Contains(newItem) && !string.Equals(Constants.textBoxPlaceholder, newItem))
            {
                items.Add(newItem);
                return true;
            }
            return false;
        }

        public void deleteItem(Object item)
        {
            if (item != null)
            {
                var selectedString = item.ToString();
                items.Remove(selectedString);
            }
        }
        
    }
}
