using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Arches.model
{
    [Table("treatmentsCategories")]
    internal class TreatmentCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string header { get; set; }

        [OneToMany]
        public List<Treatment>? treatments { get; set; }
        
        public TreatmentCategory(string header)
        {
            this.header = header;
        }

        public TreatmentCategory()
        {
            this.header = "null";
        }
    }
}
