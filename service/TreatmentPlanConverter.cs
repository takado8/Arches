using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

            var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height,
                                                96, 96, PixelFormats.Pbgra32);
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
    }
}
