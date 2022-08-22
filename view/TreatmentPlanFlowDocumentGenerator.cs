using System;
using System.Windows.Documents;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Arches.viewModel;

namespace Arches.view
{
    internal class TreatmentPlanFlowDocumentGenerator
    {
        INotifyCursorFramePosition cursorPositionNotifier;
        public TreatmentPlanFlowDocumentGenerator(INotifyCursorFramePosition cursorPositionNotifier)
        {
            this.cursorPositionNotifier = cursorPositionNotifier;
        }
        public FlowDocumentScrollViewer createTreatmentPlan(Dictionary<string, List<string>> selectedTeeth, string selectedToothCode)
        {
            List mainList = initMainList(1);
            FlowDocument flowDocument = new();
            flowDocument.Blocks.Add(mainList);
            FlowDocumentScrollViewer flowViewer = new();
            flowViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            flowViewer.Document = flowDocument;
            double flowViewerLastSize = 0;
            double cursorFramePosition = 0;
            foreach (var keyValuePair in selectedTeeth)
            {
                List subList = initSubList();
                var run = new Run(" ząb nr: " + keyValuePair.Key[1] + "." + keyValuePair.Key[2]);
                var paragraph = new Paragraph(run);
                ListItem mainListItem = new ListItem(paragraph);
                
                foreach (var value in keyValuePair.Value)
                {
                    ListItem subListItem = new ListItem(new Paragraph(new Run(value)));
                    subList.ListItems.Add(subListItem);
                }
                mainListItem.Blocks.Add(subList);
                mainList.ListItems.Add(mainListItem);
                if (keyValuePair.Key.Equals(selectedToothCode) && selectedTeeth.Count > 1)
                {
                    mainListItem.BorderThickness = new Thickness(1);
                    mainListItem.BorderBrush = Constants.getCursorFrameBrush();
                    cursorFramePosition = flowViewerLastSize;
                    cursorPositionNotifier.cursorFramePositionChanged(cursorFramePosition);
                    //MessageBox.Show(getDesiredHeight(flowViewer).ToString());
                }
                flowViewerLastSize = getDesiredHeight(flowViewer);
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

        private double getDesiredHeight(FlowDocumentScrollViewer flowViewer)
        {
            flowViewer.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            flowViewer.Arrange(new Rect(flowViewer.DesiredSize));
            flowViewer.UpdateLayout();
            return flowViewer.DesiredSize.Height;
        }

        private bool isInvalidHeight(FlowDocumentScrollViewer flowViewer)
        {
            return getDesiredHeight(flowViewer) > 800;
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
