using System;
//using System.Collections.Generic;
using System.Windows.Documents;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Arches.view
{
    internal static class TreatmentPlanFlowDocumentGenerator
    {
        public static FlowDocumentScrollViewer createTreatmentPlan(Dictionary<string, List<string>> selectedTeeth)
        {
            List mainList = new List();
            mainList.MarkerOffset = 5;
            mainList.MarkerStyle = TextMarkerStyle.Decimal;
            mainList.StartIndex = 1;
            foreach (var keyValuePair in selectedTeeth)
            {
                List subList = new List();
                subList.MarkerStyle = TextMarkerStyle.Disc;
                subList.StartIndex = 1;
                subList.MarkerOffset = 5;
                ListItem mainListItem = new ListItem(new Paragraph(new Run(" ząb nr: " + keyValuePair.Key[1] + "." + keyValuePair.Key[2])));
                foreach (var value in keyValuePair.Value)
                {
                    ListItem subListItem = new ListItem(new Paragraph(new Run(value)));
                    subList.ListItems.Add(subListItem);
                }
                subList.Padding = new Thickness(10,0,0,0);
                subList.Margin = new Thickness(0, 5, 0, 5);
                mainListItem.Blocks.Add(subList);
                mainList.ListItems.Add(mainListItem);
            }
            FlowDocument flowDocument = new();
            mainList.TextAlignment = TextAlignment.Left;
            mainList.Margin = new Thickness(10,0,0,0);
            mainList.Padding = new Thickness(2,0,0,0);
            flowDocument.Blocks.Add(mainList);
            FlowDocumentScrollViewer flowViewer = new();
            flowViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.Document = flowDocument;
            return flowViewer;
        }
    }
}
