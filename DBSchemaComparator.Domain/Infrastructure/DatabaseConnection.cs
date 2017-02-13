using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.Domain.Infrastructure
{
   public class DatabaseConnection
    {
        public string DbName { get; set; }
        public string IpAddress { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
    }
}
