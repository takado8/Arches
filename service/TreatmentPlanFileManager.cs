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
        private static string initialDirectory = System.IO.Path.Join(Environment.CurrentDirectory, "Zapisane Pliki");
       
        public static string? showSaveFileDialog(string name, string surname, string birthdate)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname)
              || string.IsNullOrEmpty(birthdate))
            {
                MessageBox.Show("Wprowadź imię, nazwisko oraz datę urodzenia.", "Niewypełnione pola.");
                return null;
            }
            string filename = surname + "_" + name + "_" + birthdate;
            if (!Directory.Exists(initialDirectory))
            {
                try
                {
                    Directory.CreateDirectory(initialDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd podczas tworzenia folderu.");
                    initialDirectory = Environment.CurrentDirectory;
                }
            }
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = filename,
                DefaultExt = "pdf",
                Filter = "PDF Document (*.pdf)|*.pdf",
                InitialDirectory = initialDirectory
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                filename = dialog.FileName;
                var directory = System.IO.Path.GetDirectoryName(filename);
                if (Directory.Exists(directory))
                {
                    return filename;
                }
                else
                {
                    MessageBox.Show("Błąd, lokalizacja nie istnieje: " + directory, "Błąd zapisu!");
                    return null;
                }
            }
            MessageBox.Show("Błąd, plik nie został zapisany!", "Błąd zapisu!");
            return null;
        }
    }
}
