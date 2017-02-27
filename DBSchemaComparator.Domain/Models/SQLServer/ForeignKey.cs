using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public class ForeignKey : IConstraint
    {
        [Column("FOREIGN_KEY_NAME")]
        public string ConstraintName { get; set; }
        [Column("TABLE_NAME")]
        public string ConstraintTable { get; set; }
        [Column("COLUMN_NAME")]
        public string ColumnApplied { get; set; }
        [Column("REFERENCED_TABLE")]
        public string ReferencedTable { get; set; }
        [Column("REFERENCED_COLUMN")]
        public string ReferencedColumn { get; set; }

    }
}
