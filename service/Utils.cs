using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;


namespace Arches.service
{
    public static class Utils
    {
        public static double getDesiredHeight(FlowDocumentScrollViewer flowViewer)
        {
            flowViewer.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            flowViewer.Arrange(new Rect(flowViewer.DesiredSize));
            flowViewer.UpdateLayout();
            return flowViewer.DesiredSize.Height;
        }

        public static double getDesiredHeight(StackPanel stackPanel)
        {
            stackPanel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            stackPanel.Arrange(new Rect(stackPanel.DesiredSize));
            stackPanel.UpdateLayout();
            return stackPanel.DesiredSize.Height;
        }
    }
}
