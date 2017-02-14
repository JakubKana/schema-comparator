using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;

using NLog;


namespace DBSchemaComparator.App
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            _logger.Info("Starting a Schema comparator application.");
           
            
            var databaseHandler = new DatabaseHandler(Settings.GetConnectionString(Settings.Instance.DatabaseConnections.DatabaseConnections.First()), DatabaseType.SqlServer);

            databaseHandler.SelectTablesSchemaInfo();
            databaseHandler.SelectColumnsSchemaInfo();
           
            Console.ReadLine();


            _logger.Info("Exiting application.");
        }
    }
}
