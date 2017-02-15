using System.Collections.Generic;
using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
   
   public class Table
    {
        public List<Column> Columns { get; set; }

        [Column("TABLE_NAME")]
        public string TableName { get; set; }
    }
}
