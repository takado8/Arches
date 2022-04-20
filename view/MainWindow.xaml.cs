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
            lockListboxSelectedEvent = true;
            listbox.SelectedItems.Clear();
            lockListboxSelectedEvent = false;
            var treatments = treatmentPlanViewModel.getSelectedToothTreatmentsList();
            if (treatments != null)
            {
                //MessageBox.Show(treatments.Count.ToString());
                lockListboxSelectedEvent = true;
                foreach (var treatment in treatments)
                {
                    //MessageBox.Show(treatment.Text);
                    listbox.SelectedItems.Add(treatment);
                }
                lockListboxSelectedEvent = false;
            }
            //else
            //{
            //    MessageBox.Show("null");
            //}
            
        }

        private void listbox_Selected(object sender, RoutedEventArgs e)
        {
            if (lockListboxSelectedEvent) return;
            treatmentPlanViewModel.updateTreatmentsForSelectedTooth(listbox.SelectedItems);
        }
       
        private void addTreatmentToList()
        {
            string newItem = textBoxNewListItem.Text;
            bool result = treatmentsListViewModel.addItem(newItem);
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
            if(textBoxNewListItem.Text == Constants.textBoxPlaceholder)
            {
                textBoxNewListItem.Text = "";
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            TreatmentPlanFileManager.saveTreatmentPlanAsImage(treatmentPlanViewModel.getTreatmentPlan(), imageGrid);
            MessageBox.Show("Image saved.");
        }

        private void textBoxNewListItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addTreatmentToList();
            }
        }
    }
}
