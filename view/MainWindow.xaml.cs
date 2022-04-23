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
        bool lockListboxSelectedEvent = false;

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
            treatmentPlanViewModel.selectTooth(ellipse);

            var treatments = treatmentPlanViewModel.getSelectedToothTreatmentsList();
            lockListboxSelectedEvent = true;
            listbox.SelectedItems.Clear();
            foreach (var treatment in treatments)
            {
                listbox.SelectedItems.Add(treatment);
            }
            lockListboxSelectedEvent = false;
        }

        private void listbox_Selected(object sender, RoutedEventArgs e)
        {
            if (lockListboxSelectedEvent) return;
            treatmentPlanViewModel.updateTreatmentsForSelectedTooth(listbox.SelectedItems);
        }
       
        private void addTreatmentToList()
        {
            string newItem = textBoxNewListItem.Text;
            var result = treatmentsListViewModel.addItem(newItem);
            if (result)
            {
                textBoxNewListItem.Text = "";
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            addTreatmentToList();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = listbox.SelectedItems;
            treatmentsListViewModel.deleteItems(selectedItems);
        }

        private void textBoxNewListItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxNewListItem.Text.Equals(Constants.textBoxPlaceholder))
            {
                textBoxNewListItem.Text = "";
            }
        }

        private void textBoxNewListItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addTreatmentToList();
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            TreatmentPlanFileManager.saveTreatmentPlanAsImage(treatmentPlanViewModel.getTreatmentPlan(), imageGrid);
            MessageBox.Show("Zapisano grafikę.", "Gotowe");
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
