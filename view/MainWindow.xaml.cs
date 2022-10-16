using Arches.service;
using Arches.view;
using Arches.viewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Arches.tests;

namespace Arches
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITreeViewItemSelected, INotifyCursorFramePosition, ISetNewItemTextBoxText
    {
        TreatmentsListViewModel treatmentsListViewModel;
        TreatmentPlanViewModel treatmentPlanViewModel;
        TreatmentPlanPdfService pdfService;
        double cursorFramePosition = 0;
        bool isFileSaved = false;

        public MainWindow()
        {
            InitializeComponent();
            treatmentsListViewModel = new TreatmentsListViewModel(treeView.Width, this, this);
            pdfService = new TreatmentPlanPdfService();
            treatmentPlanViewModel = new TreatmentPlanViewModel(this, new TreatmentPlanFlowDocumentGenerator(this,
               stackPanel));
            treeView.ItemsSource = treatmentsListViewModel.items;
            treeView.MouseLeftButtonDown += treatmentsListViewModel.TreeView_MouseLeftButtonDown;
            stackPanel.SizeChanged += ScrollViewerStackPanel_SizeChanged;
            textBoxNewListItem.Text = Constants.textBoxPlaceholderNewCategory;
        }

        public void setTextBoxText(string txt)
        {
            textBoxNewListItem.Text = txt;
        }

        private void ScrollViewerStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scrollToCursorFramePosition();
        }

        public void cursorFramePositionChanged(double cursorFramePosition)
        {
            this.cursorFramePosition = cursorFramePosition;
        }

        private void scrollToCursorFramePosition()
        {
            var position = cursorFramePosition;
            scrollViewerStackPanel.ScrollToVerticalOffset(position);
        }

        private void ellipseToothAreaClicked(object sender, MouseButtonEventArgs e)
        {
            Ellipse clickedAreaEllipse = (Ellipse)sender;
            treatmentPlanViewModel.selectTooth(clickedAreaEllipse);

            var treatments = treatmentPlanViewModel.getSelectedToothTreatmentsList();
            clearAllChildSelection();
            foreach (TreeViewItem treatment in treatments)
            {
                ((Border)treatment.Header).Background = Constants.getSelectedItemBrush();
            }
            scrollToCursorFramePosition();
        }

        private void clearAllChildSelection()
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                foreach (TreeViewItem childItem in item.Items)
                {
                    ((Border)childItem.Header).Background = Constants.getUnselectedItemBrush();
                }
            }
        }

        public void treeViewChildItemSelected(TreeViewItem treeViewItem)
        {
            treatmentPlanViewModel.updateTreatmentsForSelectedTooth(treeViewItem);
        }

        private void addTreatmentToList()
        {
            bool result;
            var selected = treatmentsListViewModel.selectedParentItem;
            string newItemDescription = textBoxNewListItem.Text;
            if (isNewItemNameValid(newItemDescription))
            {
                if (selected == null)
                {
                    result = treatmentsListViewModel.addItem(newItemDescription);
                }
                else
                {
                    result = treatmentsListViewModel.updateItem(selected, newItemDescription);
                }
                if (result)
                {
                    textBoxNewListItem.Text = "";
                }
                else
                {
                    MessageBox.Show("Nie udało się dodać zabiegu. Sprawdź czy zabieg nie znajduje się już na liście.",
                        "Błąd");
                }
            }
            else
            {
                MessageBox.Show("Nazwa zawiera niedozwolone znaki.");
            }
        }

        private bool isNewItemNameValid(string newItemName)
        {
            var regex = @"[^a-zA-Z0-9\.:\-_/)( ąęółżźćńśĄĘÓŁŻŹĆŃŚ]";
            var matches = Regex.Matches(newItemName, regex);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    return false;
                }
            }
            return true;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            addTreatmentToList();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteSelectedItems();
        }

        private void deleteSelectedItems()
        {
            if (treatmentPlanViewModel.isSelectionEmpty())
            {
                List<TreeViewItem> selectedChildren = new();
                string itemsDescriptions = "";

                foreach (TreeViewItem parentItem in treatmentsListViewModel.items)
                {
                    foreach (TreeViewItem childItem in parentItem.Items)
                    {
                        var childItemBorder = (Border)childItem.Header;
                        if (childItemBorder.Background == Constants.getSelectedItemBrush())
                        {
                            var childItemTextBlock = (TextBlock)childItemBorder.Child;
                            itemsDescriptions += childItemTextBlock.Text + ",\n";
                            selectedChildren.Add(childItem);
                        }
                    }
                }
                if (selectedChildren.Count > 0)
                {
                    string warningMsg = "Usunąć wybrane pozycje?\n" + itemsDescriptions.Remove(itemsDescriptions.Length - 2);
                    if (deleteItemsMessageBoxWarning(warningMsg))
                    {
                        foreach (var item in selectedChildren)
                        {
                            treatmentsListViewModel.deleteTreatmentItem(item);
                        }
                    }
                }
                else
                {
                    var selected = (TreeViewItem)treeView.SelectedItem;
                    var selectedBorder = (Border)selected.Header;
                    var selectedTextblock = (TextBlock)selectedBorder.Child;
                    string warningMsg = "Usunąć kategorię '" + selectedTextblock.Text + "' razem ze wszystkimi elementami które zawiera?";
                    if (deleteItemsMessageBoxWarning(warningMsg))
                    {
                        treatmentsListViewModel.deleteTreatmentCategoryItem(selected);
                        var selectedNew = (TreeViewItem)treeView.SelectedItem;
                        if (selectedNew != null)
                        {
                            selectedNew.IsSelected = false;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Usuwanie pozycji z listy jest możliwe tylko w nowym dokumencie. Zapisz dokument i otwórz nowy aby dokonać zmian.", "Nie można usunąć");
            }
        }

        private bool deleteItemsMessageBoxWarning(string message)
        {
            return MessageBox.Show(message, "Usuwanie pozycji", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
        }

        private void textBoxNewListItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if(textBoxNewListItem.Text.Equals(Constants.textBoxPlaceholderNewTreatment) ||
               textBoxNewListItem.Text.Equals(Constants.textBoxPlaceholderNewCategory))
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
            else if (e.Key == Key.Delete)
            {
                deleteSelectedItems();
            }
        }


        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                deleteSelectedItems();
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            saveFile();
        }

        private void stackPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewerStackPanel.ScrollToVerticalOffset(scrollViewerStackPanel.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private string? saveFile()
        {
            var path = FilePathBrowser.showSaveFileDialog();
            if (path != null)
            {
                clearAllChildSelection();
                treatmentPlanViewModel.deselectTooth();
                pdfService.saveTreatmentPlanAsPdfFile(path, imageGrid, treatmentPlanViewModel.getPrintableTreatmentPlan());
                isFileSaved = true;

                openDirAndSelectSavedFile(path);
                return path;
            }
            return null;
        }

        private void openDirAndSelectSavedFile(string path)
        {
            string argument = "/select, \"" + path + "\"";
            Process.Start("explorer.exe", argument);
        }

        private void MenuItemNewFile_Click(object sender, RoutedEventArgs e)
        {
            if (isFileSavedOrRejected())
            {
                newFile();
            }
        }

        private bool isFileSavedOrRejected()
        {
            if (!isFileSaved)
            {
                var result = MessageBox.Show("Plik nie jest zapisany, zapisać?", "Zapisać plik?", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    if (saveFile() == null) return false;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void newFile()
        {
            clearAllChildSelection();
            treatmentPlanViewModel.clear();
            isFileSaved = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isFileSavedOrRejected())
            {
                e.Cancel = true;
            }
        }

        private void textBoxNewListItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtRaw = textBoxNewListItem.Text;
            if (!string.IsNullOrEmpty(txtRaw) && txtRaw.Length > 2)
            {
                var changes = e.Changes;
                int changeIndex = -1;
                foreach (var change in changes)
                {
                    changeIndex = change.Offset;
                }
                if (changeIndex > -1 && changeIndex < txtRaw.Length)
                {
                    string txt = txtRaw.Substring(0, changeIndex);
                    if (changeIndex < txtRaw.Length - 1)
                    {
                        txt += txtRaw.Substring(changeIndex + 1);
                    }
                    if (txt.Equals(Constants.textBoxPlaceholderNewTreatment) ||
                     txt.Equals(Constants.textBoxPlaceholderNewCategory))
                    {
                        string newChar = txtRaw[changeIndex].ToString();
                        textBoxNewListItem.Text = newChar;
                        textBoxNewListItem.CaretIndex = 1;
                    }
                }
            }
        }

    }
}
