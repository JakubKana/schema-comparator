using System.Collections.Generic;
using System.Linq;
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
            
            List<string> stringList = new List<string>();

            stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest1;Integrated Security=True");
            stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest2;Integrated Security=True");

         //   var comparator = new ObjectComparator(connectionStrings.ElementAt(0), connectionStrings.ElementAt(1));
            var comparator = new ObjectComparator(stringList.ElementAt(0), stringList.ElementAt(1));
            comparator.CompareDatabases();
            
         
            _logger.Info("Exiting application.");
        }
    }
}
