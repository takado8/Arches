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
            List mainList = initMainList(1);
            
            FlowDocument flowDocument = new();
            flowDocument.Blocks.Add(mainList);
            FlowDocumentScrollViewer flowViewer = new();
            flowViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.Document = flowDocument;

            foreach (var keyValuePair in selectedTeeth)
            {
                List subList = initSubList();
                ListItem mainListItem = new ListItem(new Paragraph(new Run(" ząb nr: " + keyValuePair.Key[1] + "." + keyValuePair.Key[2])));
                foreach (var value in keyValuePair.Value)
                {
                    ListItem subListItem = new ListItem(new Paragraph(new Run(value)));
                    subList.ListItems.Add(subListItem);
                }
                mainListItem.Blocks.Add(subList);
                mainList.ListItems.Add(mainListItem);
            }
            return flowViewer;
        }

        public static List<FlowDocumentScrollViewer> createPrintableTreatmentPlan(Dictionary<string, List<string>> selectedTeeth)
        {
            List<FlowDocumentScrollViewer> listParts = new();
           
            List mainList = initMainList(1);
            var flowViewer = initFlowViewer(mainList);
            int i = 1;
            foreach (var keyValuePair in selectedTeeth)
            {
                List subList = initSubList();
                ListItem mainListItem = new ListItem(new Paragraph(new Run(" ząb nr: " + keyValuePair.Key[1] + "." + keyValuePair.Key[2])));
                foreach (var value in keyValuePair.Value)
                {
                    ListItem subListItem = new ListItem(new Paragraph(new Run(value)));
                    subList.ListItems.Add(subListItem);
                }
                mainListItem.Blocks.Add(subList);
                mainList.ListItems.Add(mainListItem);

                if (isInvalidHeight(flowViewer))
                {
                    mainList.ListItems.Remove(mainListItem);
                    listParts.Add(flowViewer);
                    mainList = initMainList(i);
                    flowViewer = initFlowViewer(mainList);
                    mainList.ListItems.Add(mainListItem);
                }
                i++;
            }
            listParts.Add(flowViewer);
            return listParts;
        }

        private static bool isInvalidHeight(FlowDocumentScrollViewer flowViewer)
        {
            flowViewer.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            flowViewer.Arrange(new Rect(flowViewer.DesiredSize));
            flowViewer.UpdateLayout();
            return flowViewer.DesiredSize.Height > 775;
        }
        private static FlowDocumentScrollViewer initFlowViewer(List mainList)
        {
            FlowDocument flowDocument = new();
            flowDocument.Blocks.Add(mainList);
            FlowDocumentScrollViewer flowViewer = new();
            flowViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.Document = flowDocument;
            return flowViewer;
        }

        private static List initMainList(int startIndex)
        {
            return new List() 
            {
                MarkerOffset = 5,
                MarkerStyle = TextMarkerStyle.Decimal,
                StartIndex = startIndex,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(10, 0, 0, 0),
                Padding = new Thickness(2, 0, 0, 0)
            };
        }
        private static List initSubList()
        {
            return new List()
            {
                MarkerOffset = 5,
                MarkerStyle = TextMarkerStyle.Disc,
                StartIndex = 1,
                Padding = new Thickness(10, 0, 0, 0),
                Margin = new Thickness(0, 5, 0, 5)
            };
        }
    }
}
