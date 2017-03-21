using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
  
    public class PrimaryKey : IConstraint
    {
        [Column("CONSTRAINT_NAME")]
        public string ConstraintName { get; set; }
        [Column("TABLE_NAME")]
        public string ConstraintTable { get; set; }
        [Column("COLUMN_NAME")]
        public string ColumnApplied { get; set; }
    }
}
