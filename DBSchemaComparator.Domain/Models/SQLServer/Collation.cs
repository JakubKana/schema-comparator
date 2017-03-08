using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public class Collation
    {
        [Column("COLLATION_TYPE")]
        public string CollationName { get; set; }
    }
}
