﻿using Arches.view;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Arches.service;
using System;

namespace Arches.viewModel
{
    internal class TreatmentPlanViewModel
    {
        TreatmentPlanFlowDocumentGenerator treatmentPlanGenerator;
        MainWindow mainWindow;
        StackPanel stackPanel;
        Dictionary<string, List<string>> teethDescriptions = new();
        Dictionary<string, List<TreeViewItem>> teethTreatmentsList = new();
        List<string> selectedTeeth = new();
        public string selectedToothCode { get; private set; } = "";
        Ellipse? selectedToothEllipse;

        public TreatmentPlanViewModel(MainWindow mainWindow, TreatmentPlanFlowDocumentGenerator treatmentPlanGenerator)
        {
            this.mainWindow = mainWindow;
            this.stackPanel = mainWindow.stackPanel;
            this.treatmentPlanGenerator = treatmentPlanGenerator;
        }

        public void updateTreatmentsForSelectedTooth(TreeViewItem treatment)
        {
            if (!string.IsNullOrEmpty(selectedToothCode) && teethDescriptions.ContainsKey(selectedToothCode))
            {
                TextBlock? treatmentTextBlock = (TextBlock)((Border)treatment.Header).Child;
                TreeViewItem? presentItem = null;
                if (teethTreatmentsList.ContainsKey(selectedToothCode))
                {
                    foreach (var item in teethTreatmentsList[selectedToothCode])
                    {
                        if (((TextBlock)((Border)item.Header).Child).Text.Equals(treatmentTextBlock.Text))
                        {
                            presentItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    teethTreatmentsList.Add(selectedToothCode, new List<TreeViewItem>());
                }
                if (presentItem == null)
                {
                    teethTreatmentsList[selectedToothCode].Add(treatment);
                    teethDescriptions[selectedToothCode].Add(treatmentTextBlock.Text);
                }
                else
                {
                    teethTreatmentsList[selectedToothCode].Remove(presentItem);
                    teethDescriptions[selectedToothCode].Remove(treatmentTextBlock.Text);
                }
                updateTreatmentPlan();
            }
        }

        public List<TreeViewItem> getSelectedToothTreatmentsList()
        {
            if (teethTreatmentsList.ContainsKey(selectedToothCode))
            {
                return teethTreatmentsList[selectedToothCode];
            }
            return new List<TreeViewItem>();
        }

        private void updateTreatmentPlan()
        {
            var selectedTeethDescriptionsFiltered = getSelectedTeethDescriptions();
            stackPanel.Children.Clear();

            var treatmentPlan = new FlowDocumentScrollViewer();
            stackPanel.Children.Add(treatmentPlan);
            treatmentPlanGenerator.createTreatmentPlan(selectedTeethDescriptionsFiltered,
                selectedToothCode, treatmentPlan);
            stackPanel.UpdateLayout();
        }

        private Dictionary<string, List<string>> getSelectedTeethDescriptions()
        {
            Dictionary<string, List<string>> selectedTeethDescriptionsFiltered = new();
            foreach (var toothCode in selectedTeeth)
            {
                selectedTeethDescriptionsFiltered.Add(toothCode, teethDescriptions[toothCode]);
            }
            var ordered = selectedTeethDescriptionsFiltered
                       .OrderBy(r => r.Key)
                       .ToDictionary(c => c.Key, d => d.Value);
            return ordered;
        }

        public FlowDocumentScrollViewer getTreatmentPlan()
        {
            return stackPanel.Children.Count > 0 ? (FlowDocumentScrollViewer)stackPanel.Children[0] : new FlowDocumentScrollViewer();
        }

        public List<FlowDocumentScrollViewer> getPrintableTreatmentPlan()
        {
            return treatmentPlanGenerator.createPrintableTreatmentPlan(getSelectedTeethDescriptions());
        }

        public void selectTooth(Ellipse clickedAreaEllipse)
        {
            var toothNb = clickedAreaEllipse.Name.Substring(9);
            var ellipse = getMarkerEllipse(toothNb);
            if (ellipse == null) return;
            var toothCode = "t" + toothNb;

            if (selectedToothCode.Equals(toothCode))
            { 
                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                selectedTeeth.Remove(toothCode);
                selectedToothCode = "";
                selectedToothEllipse = null;
                switchLabelVisibility(toothCode);
            }
            else
            {
                if (!selectedToothCode.Equals("") && selectedToothEllipse != null)
                {
                    selectedToothEllipse.Fill = new SolidColorBrush(Colors.Red);
                }
                ellipse.Fill = new SolidColorBrush(Colors.ForestGreen);   
                selectedToothCode = toothCode;
                selectedToothEllipse = ellipse;
                if (!selectedTeeth.Contains(toothCode))
                {
                    selectedTeeth.Add(toothCode);
                    switchLabelVisibility(toothCode);
                }
                if (!teethDescriptions.ContainsKey(toothCode))
                {
                    teethDescriptions.Add(toothCode, new List<string>());
                }
            }
            updateTreatmentPlan();
        }

        public void deselectTooth()
        {
            if (!string.IsNullOrEmpty(selectedToothCode))
            {
                var ellipse = getMarkerEllipse(selectedToothCode.Substring(1));
                if (ellipse != null)
                {
                    ellipse.Fill = new SolidColorBrush(Colors.Red);
                    selectedToothCode = "";
                    selectedToothEllipse = null;
                }
            }
        }

        private Ellipse? getMarkerEllipse(string clickedAreaEllipseNumber)
        {
            var name = "ellipse" + clickedAreaEllipseNumber;
            foreach (Ellipse ellipse in FindVisualChildren<Ellipse>(mainWindow))
            {
                if (ellipse.Name.Equals(name))
                {
                    return ellipse;
                }
            }
            return null;
        }

        private void switchLabelVisibility(string toothCode)
        {
            foreach (Label lb in FindVisualChildren<Label>(mainWindow))
            {
                if (lb.Name.Contains(toothCode))
                {
                    if (lb.Visibility == Visibility.Hidden)
                    {
                        lb.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        lb.Visibility = Visibility.Hidden;
                    }
                    break;
                }
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
        internal void clear()
        {
            teethDescriptions = new();
            teethTreatmentsList = new();
            selectedTeeth = new();
            selectedToothCode = "";
            selectedToothEllipse = null;

            foreach(var ellipse in FindVisualChildren<Ellipse>(mainWindow))
            {
                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
            }

            foreach(var label in FindVisualChildren<Label>(mainWindow))
            {
                if (label.Name.Contains("label_t"))
                {
                    label.Visibility = Visibility.Hidden;
                }
            }
            updateTreatmentPlan();
        }

        public bool isSelectionEmpty()
        {
            return teethTreatmentsList.Count < 1;
        }

    }
}
