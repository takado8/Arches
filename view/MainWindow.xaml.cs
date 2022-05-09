using Arches.service;
using Arches.view;
using Arches.viewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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
        TreatmentPlanPdfService pdfService;
        bool lockListboxSelectedEvent = false;
        bool isFileSaved = false;

        public MainWindow()
        {
            InitializeComponent();
            treatmentsListViewModel = new TreatmentsListViewModel();
            treatmentPlanViewModel = new TreatmentPlanViewModel(this, new TreatmentPlanFlowDocumentGenerator());
            pdfService = new TreatmentPlanPdfService();
            listbox.ItemsSource = treatmentsListViewModel.items;
        }
        private void ellipseToothAreaClicked(object sender, MouseButtonEventArgs e)
        {
            Ellipse clickedAreaEllipse = (Ellipse)sender;
            treatmentPlanViewModel.selectTooth(clickedAreaEllipse);

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
            saveFile();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            if (unsavedFileSafety())
            {
                Environment.Exit(0);
            }
        }

        private void stackPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewerStackPanel.ScrollToVerticalOffset(scrollViewerStackPanel.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private string? saveFile()
        {
            var path = FilePathBrowser.showSaveFileDialog(textBoxName.Text, textBoxSurname.Text, datepickerBirthday.Text);
            if (path != null)
            {
                lockListboxSelectedEvent = true;
                listbox.SelectedItems.Clear();
                lockListboxSelectedEvent = false;
                treatmentPlanViewModel.deselectTooth();

                pdfService.saveTreatmentPlanAsPdfFile(path, imageGrid, treatmentPlanViewModel.getPrintableTreatmentPlan());
                //MessageBox.Show("Plik został zapisany.", "Zapisano plik");
                isFileSaved = true;
                
                string argument = "/select, \"" + path + "\"";
                Process.Start("explorer.exe", argument);
                return path;
            }
            return null;
        }

        private void MenuItemNewFile_Click(object sender, RoutedEventArgs e)
        {
            if (unsavedFileSafety())
            {
                newFile();
            }
        }

        private bool unsavedFileSafety()
        {
            if (!isFileSaved)
            {
                var result = MessageBox.Show("Plik nie jest zapisany, zapisać?", "Zapisać plik?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (saveFile() == null) return false;
                }
            }
            return true;
        }

        private void newFile()
        {
            lockListboxSelectedEvent = true;
            listbox.SelectedItems.Clear();
            lockListboxSelectedEvent = false;
            treatmentPlanViewModel.clear();
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            datepickerBirthday.Text = "";
            isFileSaved = false;
        }
    }
}
