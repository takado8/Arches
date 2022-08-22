using Arches.service;
using Arches.model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Arches.tests
{
    public static class TestDb
    {
        static SQLiteDataStorage sqlite = new SQLiteDataStorage();

        public static async void testDb()
        {
            //given
            string categoryHeader = "desc2";
            string treatmentDescritpion1 = "treatment1";
            string treatmentDescritpion2 = "treatment2";

            TreatmentCategory treatmentCategory = new TreatmentCategory(categoryHeader);
            Treatment treatment1 = new Treatment(treatmentDescritpion1);
            Treatment treatment2 = new Treatment(treatmentDescritpion2);

            treatmentCategory.treatments = new List<Treatment>();
            treatmentCategory.treatments.Add(treatment1);
            treatmentCategory.treatments.Add(treatment2);

            //when 
            await sqlite.addTreatmentCategoryAsync(treatmentCategory);
            TreatmentCategory resultFromDb = sqlite.getItem(categoryHeader);

            //then
            if (!treatmentCategory.header.Equals(resultFromDb.header))
            {
                throw new ArgumentException("Descritpion1: " + treatmentCategory.header +
                    "\nDescritpion2: " + resultFromDb.header);
            }
            if (!resultFromDb.header.Equals(categoryHeader))
            {
                throw new ArgumentException("Descritpion doesn't match. Expected: " + categoryHeader +
                    "\nActual: " + resultFromDb.header);
            }
            if (resultFromDb.treatments == null)
            {
                throw new Exception("treatments list is null.");
            }
            if (resultFromDb.treatments.Count < 1)
            {
                throw new Exception("treatments count < 1");
            }  
            if (treatmentCategory.treatments.Count != resultFromDb.treatments.Count)
            {
                throw new ArgumentException("Count1: " + treatmentCategory.treatments.Count +
                    "\nCount2: " + resultFromDb.treatments.Count);
            }
            if (!treatmentCategory.treatments[0].description.Equals(resultFromDb.treatments[0].description))
            {
                throw new Exception("treatments descriptions doesn't match. Descritpion1: " + treatmentCategory.treatments[0].description +
                    "\nDescritpion2: " + resultFromDb.treatments[0].description);
            }
            if (!resultFromDb.treatments[0].description.Equals(treatmentDescritpion1))
            {
                throw new Exception("treatment description doesn't match. Expected: " + treatmentDescritpion1 +
                    "\nActual: " + resultFromDb.treatments[0].description);
            }
            if (!resultFromDb.treatments[1].description.Equals(treatmentDescritpion2))
            {
                throw new Exception("treatment description doesn't match. Expected: " + treatmentDescritpion2 +
                    "\nActual: " + resultFromDb.treatments[1].description);
            }
            MessageBox.Show("Test OK.");
        }
    }
}
