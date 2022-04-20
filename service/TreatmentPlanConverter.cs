using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Arches.service
{
    internal static class TreatmentPlanConverter
    {
        internal static BitmapSource FlowDocumentToBitmap(FlowDocument document, Size size)
        {
            document = CloneDocument(document);
            var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
            paginator.PageSize = size;
            var visual = new DrawingVisual();
            using (var drawingContext = visual.RenderOpen())
            {
                // draw white background
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(size));
            }
            visual.Children.Add(paginator.GetPage(0).Visual);
            var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        private static FlowDocument CloneDocument(FlowDocument document)
        {
            var copy = new FlowDocument();
            var sourceRange = new TextRange(document.ContentStart, document.ContentEnd);
            var targetRange = new TextRange(copy.ContentStart, copy.ContentEnd);

            using (var stream = new MemoryStream())
            {
                sourceRange.Save(stream, DataFormats.XamlPackage);
                targetRange.Load(stream, DataFormats.XamlPackage);
            }
            return copy;
        }

        internal static BitmapSource mergeUIElementsToImg(params FrameworkElement[] elements)
        {
            double size_multiplier = 1.5;
            double totalWidth = size_multiplier * elements.Sum(element => element.ActualWidth);
            double totalHeight = size_multiplier * elements.MaxBy(element => element.ActualHeight).ActualHeight;
            var size = new Size(totalWidth, totalHeight);
            var rectangleFrame = new Rectangle
            {
                Width = (int)size.Width,
                Height = (int)size.Height,
                Fill = Brushes.White
            };
            rectangleFrame.Arrange(new Rect(size));
            var renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(rectangleFrame);
            var xPointCordinate = 0.0;
            foreach(var element in elements)
            {
                var drawingContext = new DrawingVisual();

                using (DrawingContext draw = drawingContext.RenderOpen())
                {
                    var visualBrush = new VisualBrush(element);
                    var elementSize = new Size(size_multiplier * element.ActualWidth, size_multiplier * element.ActualHeight);
                    draw.DrawRectangle(visualBrush, null, new Rect(new Point(xPointCordinate, 0), elementSize));
                }
                xPointCordinate += size_multiplier * element.ActualWidth;
                renderBitmap.Render(drawingContext);
            }
            return renderBitmap;
        }
    }
}
