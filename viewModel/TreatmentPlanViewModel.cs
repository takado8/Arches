using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
                var textBlock = makeTextBlock();
                placeTextBlock(textBlock);
            }
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

        void deleteTextBlock(string toothCode)
        {
            int existingTextBlockIndex = -1;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                var existingTextBlock = (TextBlock)stackPanel.Children[i];
                if (existingTextBlock.Name.Equals(toothCode))
                {
                    existingTextBlockIndex = i;
                }
            }
            if (existingTextBlockIndex != -1)
            {
                stackPanel.Children.RemoveAt(existingTextBlockIndex);
            }
        }

        void placeTextBlock(TextBlock textBlock)
        {
            deleteTextBlock(textBlock.Name);
            stackPanel.Children.Add(textBlock);
        }

        TextBlock makeTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = System.Windows.TextWrapping.Wrap;
            textBlock.Name = selectedToothCode;
            textBlock.Text = "Ząb nr " + selectedToothCode.Substring(1) + "\n";

            foreach (var item in selectedTeeth[selectedToothCode])
            {
                textBlock.Text += " - " + item + "\n";
            }
            return textBlock;
        }

    }
}
