using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Arches
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string textBoxPlaceholder = "Nowy zabieg...";
        ObservableCollection<string> items = new ObservableCollection<string>();
        Dictionary<string, List<string>> selectedTeeth = new Dictionary<string, List<string>>();
        string selectedToothName = "";

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                items.Add("list item " + i.ToString());
            }
            DataContext = this;
            listbox.ItemsSource = items;
      
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var name = ellipse.Name.Substring(7);
            selectedToothName = "t" + name;
            selectedTeeth.Add(selectedToothName, new List<string>());
            string strUri = String.Format("/dental_arches/{0}a.png", name);
            archImage.Source = new BitmapImage(new Uri(strUri, UriKind.Relative));
        }

        private void listbox_Selected(object sender, RoutedEventArgs e)
        {
            var selected = listbox.SelectedItems;
            if (!String.IsNullOrEmpty(selectedToothName) && selectedTeeth.ContainsKey(selectedToothName)){
                selectedTeeth[selectedToothName].Clear();
                for (int i = 0; i < selected.Count; i++) {
                    selectedTeeth[selectedToothName].Add(selected[i].ToString());
                }
                var textBlock = makeTextBlock();
                placeTextBlock(textBlock);
            }
            //else if(selected.Count == 0){
            //    selectedTeeth[selectedToothName].Clear();

            //}

        }

        void placeTextBlock(TextBlock textBlock)
        {
            int existingTextBlockIndex = -1;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                var existingTextBlock = (TextBlock)stackPanel.Children[i];
                if (existingTextBlock.Name == selectedToothName)
                {
                    existingTextBlockIndex = i;
                }
            }
            if (existingTextBlockIndex != -1)
            {
                stackPanel.Children.RemoveAt(existingTextBlockIndex);
            }
            stackPanel.Children.Add(textBlock);
        }

        TextBlock makeTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Name = selectedToothName;
            textBlock.Text = "Ząb nr " + selectedToothName.Substring(1) + "\n";

            foreach (var item in selectedTeeth[selectedToothName])
            {
                textBlock.Text += " - " + item + "\n";
            }
            return textBlock;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string newItem = textBoxNewListItem.Text;
            if (!string.IsNullOrEmpty(newItem) && !items.Contains(newItem) && !string.Equals(textBoxPlaceholder, newItem))
            {
                items.Add(newItem);
                textBoxNewListItem.Text = "";
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = listbox.SelectedItem;
            if (selectedItem != null)
            {
                var selectedString = selectedItem.ToString();
                items.Remove(selectedString);
            }
        }

        private void textBoxNewListItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxNewListItem.Text == textBoxPlaceholder)
            {
                textBoxNewListItem.Text = "";
            }
        }
    }
}
