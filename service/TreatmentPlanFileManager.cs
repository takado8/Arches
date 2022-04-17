using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Arches.service
{
    internal static class TreatmentPlanFileManager
    {
        internal static void saveTreatmentPlanAsImage(FlowDocumentScrollViewer flowDocumentViewer, Grid archImage)
        {
            var bitMap = TreatmentPlanConverter.mergeUIElementsToImg(archImage, flowDocumentViewer);            
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitMap));
           
            using (var stream = new FileStream("doc.jpg", FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
    }
}
