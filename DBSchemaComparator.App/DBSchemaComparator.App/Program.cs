using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using NLog;

namespace DBSchemaComparator.App
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            _logger.Info("Starting a Schema comparator application.");
            Settings settings = Settings.Instance;
            string conn = Settings.GetConnectionString(new DatabaseConnection
            {
                DbName = "Name",IpAddress = "123.456.789.456",Pass = "something",Username = "username"
            });
            _logger.Debug($"Retrieved connection string {conn}");
            Console.ReadLine();
        }
    }
}
