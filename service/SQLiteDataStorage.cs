using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arches.model;
using SQLite;


namespace Arches.service
{
    internal class SQLiteDataStorage
    {
        const string dataBaseName = "treatments_db";
        string dataBasePath = Path.Combine(Environment.CurrentDirectory, dataBaseName);

        SQLiteAsyncConnection connection;

        public SQLiteDataStorage()
        {
            connection = new SQLiteAsyncConnection(dataBasePath);
            connection.CreateTableAsync<Treatment>();
        }

        public Treatment[] getItems()
        {
            var list = connection.QueryAsync<Treatment>("select * from Treatment;");
            list.Wait();
            return list.Result.ToArray();
        }

        public async Task addItemAsync(Treatment treatment)
        {
            await connection.InsertAsync(treatment);
        }

        public async Task delItemAsync(string treatmentDescription)
        {
            await connection.QueryAsync<Treatment>("DELETE FROM Treatment WHERE description='" + treatmentDescription + "';");
        }
    }
}
