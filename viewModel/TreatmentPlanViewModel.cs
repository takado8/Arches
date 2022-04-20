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
        StackPanel stackPanel;
        Dictionary<string, List<string>> selectedTeethDescriptions = new();
        Dictionary<string, List<TextBlock>> teethTreatmentsList = new();
        List<string> selectedTeeth = new();
        string selectedToothCode = "";
        Ellipse? selectedToothEllipse;

        public TreatmentPlanViewModel(StackPanel stackPanel)
        {
            this.stackPanel = stackPanel;
        }

        public void updateTreatmentsForSelectedTooth(IList treatments)
        {
            if (!String.IsNullOrEmpty(selectedToothCode) && selectedTeethDescriptions.ContainsKey(selectedToothCode))
            {
                if (teethTreatmentsList.ContainsKey(selectedToothCode))
                {
                    teethTreatmentsList[selectedToothCode].Clear();
                }
                else
                {
                    teethTreatmentsList.Add(selectedToothCode, new List<TextBlock>());
                }
                selectedTeethDescriptions[selectedToothCode].Clear();
                for (int i = 0; i < treatments.Count; i++)
                {
                    var treatmentRaw = treatments[i];
                    if(treatmentRaw != null)
                    {
                        TextBlock treatment = (TextBlock)treatmentRaw;
                        teethTreatmentsList[selectedToothCode].Add(treatment);
                        selectedTeethDescriptions[selectedToothCode].Add(treatment.Text);
                    }
                }
                updateTreatmentPlan();
            }
        }

        public List<TextBlock>? getSelectedToothTreatmentsList()
        {
            //MessageBox.Show(selectedToothCode);
            if (teethTreatmentsList.ContainsKey(selectedToothCode))
            {
                return teethTreatmentsList[selectedToothCode];
            }
            return null;
        }
        private void updateTreatmentPlan()
        {
            Dictionary<string, List<string>> selectedTeethDescriptionsFiltered = new();
            foreach(var toothCode in selectedTeeth)
            {
                selectedTeethDescriptionsFiltered.Add(toothCode, selectedTeethDescriptions[toothCode]);
            }
            var treatmentPlan = TreatmentPlanFlowDocumentGenerator.createTreatmentPlan(selectedTeethDescriptionsFiltered);
            stackPanel.Children.Clear();
            stackPanel.Children.Add(treatmentPlan);
            //MessageBox.Show("list count: " + teethTreatmentsList.Count.ToString());
            //foreach(var toothcode in teethTreatmentsList)
            //{
            //    MessageBox.Show(toothcode.Key);
            //}
        }

        public FlowDocumentScrollViewer getTreatmentPlan()
        {
            return stackPanel.Children.Count > 0 ? (FlowDocumentScrollViewer)stackPanel.Children[0] : new FlowDocumentScrollViewer();
        }

        public void selectTooth(Ellipse ellipse)
        {
            var name = ellipse.Name.Substring(7);
            var toothCode = "t" + name;
            
            if (selectedToothCode.Equals(toothCode))
            { 
                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                selectedTeeth.Remove(toothCode);
                selectedToothCode = "";
                selectedToothEllipse = null;
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
                }
                if (!selectedTeethDescriptions.ContainsKey(toothCode))
                {
                    selectedTeethDescriptions.Add(toothCode, new List<string>());
                }
            }
            updateTreatmentPlan();
        }
    }
}
