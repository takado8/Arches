using Arches.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Arches.viewModel
{
    internal class TreatmentPlanViewModel
    {
        StackPanel stackPanel;
        Dictionary<string, List<string>> selectedTeeth = new Dictionary<string, List<string>>();
        string selectedToothCode = "";
        public TreatmentPlanViewModel(StackPanel stackPanel)
        {
            this.stackPanel = stackPanel;
        }

        public void updateTreatmentPlan(System.Collections.IList treatments)
        {
            if (!String.IsNullOrEmpty(selectedToothCode) && selectedTeeth.ContainsKey(selectedToothCode))
            {
                selectedTeeth[selectedToothCode].Clear();
                for (int i = 0; i < treatments.Count; i++)
                {
                    var treatmentsRaw = treatments[i];
                    if(treatmentsRaw != null)
                    {
                        TextBlock treatment = (TextBlock)treatmentsRaw;
                        selectedTeeth[selectedToothCode].Add(treatment.Text);
                    }
                }
                var treatmentPlan = TreatmentPlanFlowDocumentGenerator.createTreatmentPlan(selectedTeeth);
                stackPanel.Children.Clear();
                stackPanel.Children.Add(treatmentPlan);
            }
        }

        public FlowDocumentScrollViewer getTreatmentPlan()
        {
            return stackPanel.Children.Count > 0 ? (FlowDocumentScrollViewer)stackPanel.Children[0] : new FlowDocumentScrollViewer();
        }

        public void selectTooth(Ellipse ellipse, string toothCode)
        {
            if (selectedTeeth.ContainsKey(toothCode))
            {
                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                selectedToothCode = "";
                selectedTeeth.Remove(toothCode);
            }
            else
            {
                selectedTeeth.Add(toothCode, new List<string>());
                ellipse.Fill = new SolidColorBrush(Colors.Red);
                selectedToothCode = toothCode;
            }
        }
    }
}
