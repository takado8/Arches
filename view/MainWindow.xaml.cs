﻿using Arches.service;
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

namespace Arches
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITreeViewItemSelected, INotifyCursorFramePosition
    {
        TreatmentsListViewModel treatmentsListViewModel;
        TreatmentPlanViewModel treatmentPlanViewModel;
        TreatmentPlanPdfService pdfService;
        double cursorFramePosition = 0;
        bool isFileSaved = false;

        public MainWindow()
        {
            InitializeComponent();

            treatmentsListViewModel = new TreatmentsListViewModel(treeView.Width, this);
            pdfService = new TreatmentPlanPdfService();
            treatmentPlanViewModel = new TreatmentPlanViewModel(this, new TreatmentPlanFlowDocumentGenerator(this,
               stackPanel));
            treeView.ItemsSource = treatmentsListViewModel.items;
            treeView.MouseLeftButtonDown += treatmentsListViewModel.TreeView_MouseLeftButtonDown;            
            stackPanel.SizeChanged += ScrollViewerStackPanel_SizeChanged;
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

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            addTreatmentToList();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (treatmentPlanViewModel.isSelectionEmpty())
            {
                List<TreeViewItem> selectedChildren = new();

                foreach (TreeViewItem parentItem in treatmentsListViewModel.items)
                {
                    foreach (TreeViewItem childItem in parentItem.Items)
                    {
                        if (((Border)childItem.Header).Background == Constants.getSelectedItemBrush())
                        {
                            selectedChildren.Add(childItem);
                        }
                    }
                }
                if (selectedChildren.Count > 0)
                {
                    foreach (var item in selectedChildren)
                    {
                        treatmentsListViewModel.deleteTreatmentItem(item);
                    }
                }
                else
                {
                    var selected = (TreeViewItem)treeView.SelectedItem;
                    treatmentsListViewModel.deleteTreatmentCategoryItem(selected);
                    var selectedNew = (TreeViewItem)treeView.SelectedItem;
                    if (selectedNew != null)
                    {
                        selectedNew.IsSelected = false;
                    }
                }
            }   
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
                clearAllChildSelection();
                treatmentPlanViewModel.deselectTooth();
                pdfService.saveTreatmentPlanAsPdfFile(path, imageGrid, treatmentPlanViewModel.getPrintableTreatmentPlan());
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
            clearAllChildSelection();
            treatmentPlanViewModel.clear();
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            datepickerBirthday.Text = "";
            isFileSaved = false;
        }

    }
}
