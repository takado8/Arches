using System;
using System.Windows.Documents;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Arches.view
{
    internal class TreatmentPlanFlowDocumentGenerator
    {
        public FlowDocumentScrollViewer createTreatmentPlan(Dictionary<string, List<string>> selectedTeeth)
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

        public List<FlowDocumentScrollViewer> createPrintableTreatmentPlan(Dictionary<string, List<string>> selectedTeeth)
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

        private bool isInvalidHeight(FlowDocumentScrollViewer flowViewer)
        {
            flowViewer.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            flowViewer.Arrange(new Rect(flowViewer.DesiredSize));
            flowViewer.UpdateLayout();
            return flowViewer.DesiredSize.Height > 800;
        }
        private FlowDocumentScrollViewer initFlowViewer(List mainList)
        {
            FlowDocument flowDocument = new();
            flowDocument.Blocks.Add(mainList);
            FlowDocumentScrollViewer flowViewer = new();
            flowViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.Document = flowDocument;
            flowViewer.VerticalAlignment = VerticalAlignment.Top;
            flowViewer.HorizontalAlignment = HorizontalAlignment.Center;
            flowViewer.VerticalContentAlignment = VerticalAlignment.Top;
            flowViewer.HorizontalContentAlignment = HorizontalAlignment.Center;
            return flowViewer;
        }

        private List initMainList(int startIndex)
        {
            return new List() 
            {
                MarkerOffset = 5,
                MarkerStyle = TextMarkerStyle.Decimal,
                StartIndex = startIndex,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(18, 0, 0, 0),
                Padding = new Thickness(2, 0, 0, 0)
            };
        }
        private List initSubList()
        {
            return new List()
            {
                MarkerOffset = 5,
                MarkerStyle = TextMarkerStyle.Disc,
                StartIndex = 1,
                Padding = new Thickness(18, 0, 0, 0),
                Margin = new Thickness(0, 5, 0, 5)
            };
        }
    }
}
