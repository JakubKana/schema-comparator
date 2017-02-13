namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public class Column
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsNotNull { get; set; }
        public bool IsIdentification { get; set; }

    }
}
