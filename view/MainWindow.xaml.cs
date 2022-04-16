using Arches.service;
using Arches.view;
using Arches.viewModel;
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
        TreatmentsListViewModel treatmentsListViewModel;
        TreatmentPlanViewModel treatmentPlanViewModel;

        public MainWindow()
        {
            InitializeComponent();
            treatmentsListViewModel = new TreatmentsListViewModel();
            treatmentPlanViewModel = new TreatmentPlanViewModel(stackPanel);
            listbox.ItemsSource = treatmentsListViewModel.items;
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ellipse = (Ellipse)sender;
            var name = ellipse.Name.Substring(7);
            var toothCode = "t" + name;

            treatmentPlanViewModel.selectTooth(ellipse, toothCode);
        }

        private void listbox_Selected(object sender, RoutedEventArgs e)
        {
            treatmentPlanViewModel.updateTreatmentPlan(listbox.SelectedItems);
        }
       
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            string newItem = textBoxNewListItem.Text;
            bool result = treatmentsListViewModel.addItem(newItem);
            if (result)
            {
                textBoxNewListItem.Text = "";
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = listbox.SelectedItem;
            treatmentsListViewModel.deleteItem(selectedItem);
        }

        private void textBoxNewListItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxNewListItem.Text == Constants.textBoxPlaceholder)
            {
                textBoxNewListItem.Text = "";
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            TreatmentPlanFileManager.saveTreatmentPlanAsImage(treatmentPlanViewModel.getTreatmentPlan());
            MessageBox.Show("Image saved.");

        }
    }
}
