using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Arches.model
{
    [Table("treatments")]
    internal class Treatment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
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
