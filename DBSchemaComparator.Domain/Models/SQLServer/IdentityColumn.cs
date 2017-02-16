using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    [TableName("COLUMNS")]
    public class IdentityColumn
    {
        [Column("TABLE_NAME")]
        public string TableName { get; set; }
        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }
    }
}