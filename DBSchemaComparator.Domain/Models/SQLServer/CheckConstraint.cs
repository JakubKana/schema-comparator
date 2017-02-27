using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public class CheckConstraint : IConstraint
    {
        [Column("CONSTRAINT_NAME")]
        public string ConstraintName { get; set; }
        [Column("TABLE_NAME")]
        public string ConstraintTable { get; set; }
        [Column("CHECK_CLAUSE")]
        public string ConstraintBody { get; set; }
        [Column("COLUMN_NAME")]
        public string ColumnApplied { get; set; }
    }
}
