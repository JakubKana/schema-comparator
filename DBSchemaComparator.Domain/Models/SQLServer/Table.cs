using System.Collections.Generic;
using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
   [TableName("TABLES")]
   public class Table
    {
        [Ignore]
        public List<Column> Columns { get; set; }

        [Column("TABLE_NAME")]
        public string TableName { get; set; }
    }
}
