using Microsoft.Win32;
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
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.Windows.Xps;
using PdfSharp;

namespace Arches.service
{
    internal static class TreatmentPlanConverter
    {
        private static string INITIAL_DIRECTORY = System.IO.Path.Join(Environment.CurrentDirectory, "Zapisane Pliki");
        private const double PAGE_A4_WIDTH = 11.69 * 96;
        private const double PAGE_A4_HEIGHT = 8.27 * 96;

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
            double width_multiplier = 1.5;
            double height_multiplier = 1.2;
            double totalWidth = width_multiplier * elements.Sum(element => element.ActualWidth);
            double totalHeight = height_multiplier * elements.MaxBy(element => element.ActualHeight).ActualHeight;
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
                    var elementSize = new Size(width_multiplier * element.ActualWidth, height_multiplier * element.ActualHeight);
                    draw.DrawRectangle(visualBrush, null, new Rect(new Point(xPointCordinate, 0), elementSize));
                }
                xPointCordinate += width_multiplier * element.ActualWidth;
                renderBitmap.Render(drawingContext);
            }
            return renderBitmap;
        }
        private static Grid makeGrid()
        {
            Grid grid = new Grid();
            grid.Width = PAGE_A4_WIDTH;
            grid.Height = PAGE_A4_HEIGHT;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            return grid;
        }

        private static FixedPage makeFixedPage()
        {
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = PAGE_A4_WIDTH;
            fixedPage.Height = PAGE_A4_HEIGHT;
            return fixedPage;
        }

        private static Image makeImage(FrameworkElement imageGrid)
        {
            var img = mergeUIElementsToImg(imageGrid);
            Image image = new Image();
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Source = img;
            image.SetValue(Grid.ColumnProperty, 0);
            return image;
        }
        public static void UIElementToPdf2(FrameworkElement imageGrid, List<FlowDocumentScrollViewer> treatmentPlans)
        {
            if (treatmentPlans.Count < 1)
            {
                return;
            }
            var treatmentPlan = treatmentPlans[0];

            MemoryStream lMemoryStream = new MemoryStream();
            Package package = Package.Open(lMemoryStream, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = makeFixedPage();

            Grid grid = makeGrid();
            Image image = makeImage(imageGrid);

            grid.Children.Add(image);
            grid.Children.Add(treatmentPlan);
            treatmentPlan.SetValue(Grid.ColumnProperty, 1);

            fixedPage.Children.Add(grid);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

            fixedDoc.Pages.Add(pageContent);

            if (treatmentPlans.Count > 1)
            {
                for (int i = 1; i < treatmentPlans.Count; i+=2)
                {
                    PageContent pageContent2 = new PageContent();
                    FixedPage fixedPage2 = makeFixedPage();
                    Grid grid2 = makeGrid();

                    grid2.Children.Add(treatmentPlans[i]);
                    treatmentPlans[i].SetValue(Grid.ColumnProperty, 0);
                   
                    if (i + 1 < treatmentPlans.Count)
                    {
                        grid2.Children.Add(treatmentPlans[i + 1]);
                        treatmentPlans[i + 1].SetValue(Grid.ColumnProperty, 1);
                    }
                    fixedPage2.Children.Add(grid2);
                    ((System.Windows.Markup.IAddChild)pageContent2).AddChild(fixedPage2);
                    fixedDoc.Pages.Add(pageContent2);
                }
            }
            writer.Write(fixedDoc);

            doc.Close();
            package.Close();

            MemoryStream outStream = new MemoryStream();
            PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);

            // Write pdf file
            var filePath = TreatmentPlanFileManager.showSaveFileDialog("test1", "test1", "16.02.1986");
            if (filePath == null) return;
           
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            outStream.CopyTo(fileStream);

            outStream.Flush();
            outStream.Close();
            fileStream.Flush();
            fileStream.Close();
        }
    }
}
