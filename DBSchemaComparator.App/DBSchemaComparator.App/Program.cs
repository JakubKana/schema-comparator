using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Infrastructure;
using NLog;

namespace DBSchemaComparator.App
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
                Settings settings = Settings.Instance;

            Console.ReadLine();
        }
    }
}
