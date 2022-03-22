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
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "TextBlock text...";
            stackPanel.Children.Add(textBlock);
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var name = ellipse.Name.Substring(7);
            selectedToothName = name;
            selectedTeeth.Add(name, new List<string>());
            string strUri = String.Format("/dental_arches/{0}a.png", name);
            archImage.Source = new BitmapImage(new Uri(strUri, UriKind.Relative));
        }

        private void listbox_Selected(object sender, RoutedEventArgs e)
        {
            var selected = sender.ToString();
            if (!String.IsNullOrEmpty(selectedToothName) && selectedTeeth.ContainsKey(selectedToothName)
                    && !String.IsNullOrEmpty(selected))
            {
                selectedTeeth[selectedToothName].Add(selected);
                
                TextBlock textBlock = new TextBlock();
                textBlock.Name = selectedToothName;
                textBlock.Text = selectedToothName + "\n";
                foreach (var item in selectedTeeth[selectedToothName])
                {
                    textBlock.Text += " - " + item + "\n";
                }
                for(int i = 0; i < stackPanel.Children.Count; i++)
                {
                    var child = (TextBlock) stackPanel.Children[i];
                    if (child.Name == selectedToothName)
                    {

                    }
                }
            }

            
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
