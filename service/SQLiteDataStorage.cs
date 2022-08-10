using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Arches.model;
using SQLiteNetExtensions;
using SQLite;
using System.Collections.Generic;

namespace Arches.service
{
    internal class SQLiteDataStorage
    {
        const string databaseName = "treatments_db2";
        string databasePath = Path.Combine(Environment.CurrentDirectory, databaseName);

        SQLiteAsyncConnection connection;
        //SQLiteConnection conn;

        public SQLiteDataStorage()
        {
            connection = new SQLiteAsyncConnection(databasePath);
            connection.CreateTableAsync<TreatmentCategory>();
            connection.CreateTableAsync<Treatment>();
        }

        public List<TreatmentCategory>? getItems()
        {
            try
            {
                var categoriesTask = connection.QueryAsync<TreatmentCategory>("SELECT * FROM treatmentsCategories;");
                categoriesTask.Wait();
                var categories = categoriesTask.Result;
              
                foreach (var treatmentCategory in categories)
                {
                    var treatmentsResult = connection.QueryAsync<Treatment>("SELECT * FROM treatments WHERE treatmentCategoryId=" + treatmentCategory.Id + ";");
                    treatmentsResult.Wait();
                    var treatments = treatmentsResult.Result;
                    treatmentCategory.treatments = treatments;
                }
                return categories;
            }
            catch { return null; }
        }

        public TreatmentCategory getItem(string itemHeader)
        {
            var result = connection.QueryAsync<TreatmentCategory>("SELECT * FROM treatmentsCategories WHERE header='" + itemHeader + "';");
            result.Wait();
            TreatmentCategory treatmentCategory = result.Result[0];

            var treatmentsResult = connection.QueryAsync<Treatment>("SELECT * FROM treatments WHERE treatmentCategoryId=" + treatmentCategory.Id + ";");
            treatmentsResult.Wait();
            var treatments = treatmentsResult.Result;
            treatmentCategory.treatments = treatments;
            return treatmentCategory;
        }
      
        public async Task addTreatmentCategoryAsync(TreatmentCategory treatmentCategory)
        {
            await connection.InsertAsync(treatmentCategory);
            if (treatmentCategory.treatments != null)
            {
                foreach (var treatment in treatmentCategory.treatments)
                {
                    treatment.treatmentCategoryId = treatmentCategory.Id;
                }
                await connection.InsertAllAsync(treatmentCategory.treatments);
            }
        }

        public async Task addTreatmentAsync(Treatment treatment)
        {
            await connection.InsertAsync(treatment);
        }

        public async Task delItemAsync(string itemHeader)
        {
            var result = connection.QueryAsync<TreatmentCategory>("SELECT * FROM treatmentsCategories WHERE header='" + itemHeader + "';");
            result.Wait();
            TreatmentCategory treatmentCategory = result.Result[0];

            await connection.QueryAsync<TreatmentCategory>("DELETE FROM treatmentsCategories WHERE header='" + itemHeader + "';");
            await connection.QueryAsync<Treatment>("DELETE FROM treatments WHERE treatmentCategoryId=" + treatmentCategory.Id + ";");
        }
    }
}
