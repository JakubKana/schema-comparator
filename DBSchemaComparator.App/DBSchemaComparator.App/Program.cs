using System;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using DBSchemaComparator.App.Comparator;
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
            var connectionStrings = Settings.GetDatabaseConnectionStrings(Settings.Instance.DatabaseConnections);

            var comparator = new ObjectComparator(connectionStrings.ElementAt(0),connectionStrings.ElementAt(1));

            comparator.TestTables();
            var indexes = comparator.LeftDatabase.GetIndexesInfo();

            _logger.Info("Exiting application.");
        }
    }
}
