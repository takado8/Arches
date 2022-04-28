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

        public static void UIElementToPdf(FrameworkElement element)
        {
            var dialog = new SaveFileDialog();

            dialog.AddExtension = true;
            dialog.DefaultExt = "pdf";
            dialog.Filter = "PDF Document (*.pdf)|*.pdf";
            dialog.InitialDirectory = INITIAL_DIRECTORY;
            if (dialog.ShowDialog() == false)
                return;


            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            //PrintDialog printDlg = new PrintDialog();
            //Size pageSize = new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight - 100);

            var visual = element as UIElement;

            //((System.Windows.Controls.Panel)this.Content).Children.Remove(visual);
            RemoveFromParent((FrameworkElement)visual);
            fixedPage.Children.Add(visual);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

            fixedDoc.Pages.Add(pageContent);

            // write to PDF file
            string tempFilename = "temp.xps";
            File.Delete(tempFilename);
            XpsDocument xpsd = new XpsDocument(tempFilename, FileAccess.ReadWrite);
            System.Windows.Xps.XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsd);
            writer.Write(fixedDoc);
            xpsd.Close();

            PdfSharp.Xps.XpsConverter.
            Convert(
            tempFilename,
            dialog.FileName, 1);
        }

        public static void UIElementToPdf2(FrameworkElement imageGrid, FrameworkElement treatmentPlan)
        {
            var dialog = new SaveFileDialog();

            dialog.AddExtension = true;
            dialog.DefaultExt = "pdf";
            dialog.Filter = "PDF Document (*.pdf)|*.pdf";
            dialog.InitialDirectory = INITIAL_DIRECTORY;
            if (dialog.ShowDialog() == false)
                return;

            MemoryStream lMemoryStream = new MemoryStream();
            Package package = Package.Open(lMemoryStream, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = 11.69 * 96;
            fixedPage.Height = 8.27 * 96;

            Grid grid = new Grid();
           
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            var img = mergeUIElementsToImg(imageGrid);
            Image image = new Image();
            image.Source = img;
            grid.Children.Add(image);
            image.SetValue(Grid.ColumnProperty, 0);
            RemoveFromParent(treatmentPlan);

            MessageBox.Show("height: " + treatmentPlan.ActualHeight.ToString());

            grid.Children.Add(treatmentPlan);
            treatmentPlan.SetValue(Grid.ColumnProperty, 1);


            fixedPage.Children.Add(grid);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

            fixedDoc.Pages.Add(pageContent);
           
            writer.Write(fixedDoc);

            doc.Close();
            package.Close();

            // Convert 
            MemoryStream outStream = new MemoryStream();
            PdfSharp.Xps.XpsConverter.Convert(lMemoryStream, outStream, false);

            // Write pdf file
            FileStream fileStream = new FileStream(dialog.FileName, FileMode.Create);
            outStream.CopyTo(fileStream);

            // Clean up
            outStream.Flush();
            outStream.Close();
            fileStream.Flush();
            fileStream.Close();
        }


        public static void RemoveFromParent(this FrameworkElement item)
        {
            if (item != null)
            {
                var parent = (StackPanel) item.Parent;
                if (parent != null)
                {
                    parent.Children.Remove(item as UIElement);
                    //parentItemsControl.Items.Remove(item as UIElement);
                }
            }
        }
    }
}
