using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
   
    [TableName("COLUMNS")]
    [ExplicitColumns]
    public class Column
    {
        [Column("TABLE_NAME")]
        public string TableName { get; set; }
        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }
        [Column("DATA_TYPE")]
        public string DataType { get; set; }
        [Column("IS_NULLABLE")]
        public string IsNullable { get; set; }
        [Ignore]
        public bool IsIdentification { get; set; }

    }
}
