using System.Collections.Generic;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
   public class Table
    {
        public List<Column> Columns { get; set; }
        public string TableName { get; set; }
    }
}
