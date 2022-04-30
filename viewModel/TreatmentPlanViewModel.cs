using Arches.view;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Arches.viewModel
{
    internal class TreatmentPlanViewModel
    {
        MainWindow mainWindow;
        StackPanel stackPanel;
        Dictionary<string, List<string>> teethDescriptions = new();
        Dictionary<string, List<TextBlock>> teethTreatmentsList = new();
        List<string> selectedTeeth = new();
        string selectedToothCode = "";
        Ellipse? selectedToothEllipse;

        public TreatmentPlanViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.stackPanel = mainWindow.stackPanel;
        }

        public void updateTreatmentsForSelectedTooth(IList treatments)
        {
            if (!String.IsNullOrEmpty(selectedToothCode) && teethDescriptions.ContainsKey(selectedToothCode))
            {
                if (teethTreatmentsList.ContainsKey(selectedToothCode))
                {
                    teethTreatmentsList[selectedToothCode].Clear();
                }
                else
                {
                    teethTreatmentsList.Add(selectedToothCode, new List<TextBlock>());
                }
                teethDescriptions[selectedToothCode].Clear();
                for (int i = 0; i < treatments.Count; i++)
                {
                    var treatmentRaw = treatments[i];
                    if(treatmentRaw != null)
                    {
                        TextBlock treatment = (TextBlock)treatmentRaw;
                        teethTreatmentsList[selectedToothCode].Add(treatment);
                        teethDescriptions[selectedToothCode].Add(treatment.Text);
                    }
                }
                updateTreatmentPlan();
            }
        }

        public List<TextBlock> getSelectedToothTreatmentsList()
        {
            if (teethTreatmentsList.ContainsKey(selectedToothCode))
            {
                return teethTreatmentsList[selectedToothCode];
            }
            return new List<TextBlock>();
        }

        private void updateTreatmentPlan()
        {
            var selectedTeethDescriptionsFiltered = getSelectedTeethDescriptions();
            var treatmentPlan = TreatmentPlanFlowDocumentGenerator.createTreatmentPlan(selectedTeethDescriptionsFiltered);
            stackPanel.Children.Clear();
            stackPanel.Children.Add(treatmentPlan);
        }

        private Dictionary<string, List<string>> getSelectedTeethDescriptions()
        {
            Dictionary<string, List<string>> selectedTeethDescriptionsFiltered = new();
            foreach (var toothCode in selectedTeeth)
            {
                selectedTeethDescriptionsFiltered.Add(toothCode, teethDescriptions[toothCode]);
            }
            return selectedTeethDescriptionsFiltered;
        }

        public FlowDocumentScrollViewer getTreatmentPlan()
        {
            return stackPanel.Children.Count > 0 ? (FlowDocumentScrollViewer)stackPanel.Children[0] : new FlowDocumentScrollViewer();
        }

        public List<FlowDocumentScrollViewer> getPrintableTreatmentPlan()
        {
            return TreatmentPlanFlowDocumentGenerator.createPrintableTreatmentPlan(getSelectedTeethDescriptions());
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
    }
}
