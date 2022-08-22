using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.Windows.Xps;

namespace Arches.service
{
    internal class TreatmentPlanPdfService
    {
        private const double PAGE_A4_WIDTH = 11.69 * 96;
        private const double PAGE_A4_HEIGHT = 8.27 * 96;

        public void saveTreatmentPlanAsPdfFile(string filePath, FrameworkElement imageGrid, List<FlowDocumentScrollViewer> treatmentPlans)
        {
            if (treatmentPlans.Count < 1) return;

            MemoryStream lMemoryStream = new MemoryStream();
            Package package = Package.Open(lMemoryStream, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = makeFixedPage();
            Grid grid = makeFirstPageGrid();
            
            Image image = makeImage(imageGrid);

            var treatmentPlanFirstPart = treatmentPlans[0];
            treatmentPlanFirstPart.SetValue(Grid.ColumnProperty, 1);

            grid.Children.Add(image);
            grid.Children.Add(treatmentPlanFirstPart);

            Grid outerGrid = makeFirstPageOuterGrid();
            outerGrid.Children.Add(grid);

            fixedPage.Children.Add(outerGrid);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            if (treatmentPlans.Count > 1)
            {
                for (int i = 1; i < treatmentPlans.Count; i += 2)
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
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Create);
                outStream.CopyTo(fileStream);
                outStream.Flush();
                outStream.Close();
                fileStream.Flush();
                fileStream.Close();
            }
            catch(IOException ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message, "Błąd zapisu!");
            }
        }

        private BitmapSource mergeUIElementsToImg(params FrameworkElement[] elements)
        {
            double width_multiplier = 1.4;
            double height_multiplier = 1.1;
            double totalWidth = width_multiplier * elements.Sum(element => element.ActualWidth);
            double totalHeight = height_multiplier * elements.MaxBy(element => element.ActualHeight).ActualHeight;
            var size = new Size(totalWidth, totalHeight);
            var rectangleFrame = new Rectangle
            {
                Width = (int)size.Width,
                Height = (int)size.Height,
                Fill = Brushes.Transparent
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
        private Grid makeFirstPageOuterGrid()
        {
            Grid grid = new Grid();
            grid.Width = PAGE_A4_WIDTH;
            grid.Height = PAGE_A4_HEIGHT;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;
            return grid;
        }
        private Grid makeFirstPageGrid()
        {
            Grid grid = new Grid();
            grid.Width = PAGE_A4_WIDTH - 20;
            grid.Height = PAGE_A4_HEIGHT - 20;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            return grid;
        }

        private Grid makeGrid()
        {
            Grid grid = new Grid();
            grid.Width = PAGE_A4_WIDTH;
            grid.Height = PAGE_A4_HEIGHT;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            return grid;
        }

        private FixedPage makeFixedPage()
        {
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = PAGE_A4_WIDTH;
            fixedPage.Height = PAGE_A4_HEIGHT;
            return fixedPage;
        }

        private Image makeImage(FrameworkElement imageGrid)
        {
            var img = mergeUIElementsToImg(imageGrid);
            Image image = new Image();
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Source = img;
            image.SetValue(Grid.ColumnProperty, 0);
            return image;
        }
    }
}
