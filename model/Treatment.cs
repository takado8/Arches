using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arches.model
{
    [Table("treatments")]
    internal class Treatment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string description { get; set; }

        [ForeignKey(typeof(TreatmentCategory))]
        public int treatmentCategoryId { get; set; }

        public Treatment(string description)
        {
            this.description = description;
        }

        public Treatment()
        {
            description = "null";
        }
    }
}
