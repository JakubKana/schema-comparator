using System.Collections.Generic;
using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{

    public class Index
    {
        [Column("IndexName")]
        public string IndexName { get; set; }
        [Column("IndexType")]
        public string IndexType { get; set; }
        [Column("TableName")]
        public string TableName { get; set; }
        [Column("IsPrimaryKey")]
        public bool IsPrimaryKey { get; set; }
        [Column("IsUnique")]
        public bool IsUnique { get; set; }
        [Column("ColumnName")]
        public string ColumnName { get; set; }
    }
}
