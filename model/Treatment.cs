using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arches.model
{
    internal class Treatment
    {
        public string description { get; set; }

        public Treatment(string description)
        {
            this.description = description;
        }

        public Treatment()
        {
            this.description = "NULL";
        }
    }
}
