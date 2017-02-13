using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    class Column
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsNotNull { get; set; }


    }
}
