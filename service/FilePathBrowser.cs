using System;
using System.IO;
using System.Windows;

namespace Arches.service
{
    internal static class FilePathBrowser
    {
        private static string initialDirectory = System.IO.Path.Join(Environment.CurrentDirectory, "Zapisane Pliki");
       
        public static string? showSaveFileDialog(string name, string surname, string birthdate)
        {
            string filename;
            if (isValidCredentials(name, surname))
            {
                if (birthdate == null)
                {
                    birthdate = "";
                }
                else if (!string.IsNullOrEmpty(birthdate))
                {
                    birthdate = "_ur_" + birthdate;
                }
                filename = surname + "_" + name + birthdate + "_dnia_" + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss"); ;
            }
            else
            {
                filename = DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss");
            }
            createDefaultDirectory();
            
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

        private static bool isValidCredentials(string name, string surname)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                return false;
            }
            return true;
        }

        private static void createDefaultDirectory()
        {
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
        }
    }
}
