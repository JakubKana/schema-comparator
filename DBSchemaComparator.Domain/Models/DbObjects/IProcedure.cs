using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.Domain.Models.SQLServer
{
    public interface IProcedure
    {
        string Name { get; set; }
        string Body { get; set; }
    }
}
