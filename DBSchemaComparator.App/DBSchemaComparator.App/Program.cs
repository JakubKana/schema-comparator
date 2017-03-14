using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Infrastructure;

using NLog;
using Extensions = DBSchemaComparator.Domain.Infrastructure.Extensions;


namespace DBSchemaComparator.App
{
    public class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        static void Main(string[] args)
        {
            _logger.Info("Starting a Schema comparator application.");
            var connectionStrings = Settings.GetDatabaseConnectionStrings(Settings.Instance.SettingsObject);
            
            Xml xml = new Xml();
            
            //List<string> stringList = new List<string>();

            //stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest1;Integrated Security=True");
            //stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest2;Integrated Security=True");

            //var comparator = new ObjectComparator(stringList.ElementAt(0), stringList.ElementAt(1));

            var comparator = new ObjectComparator(connectionStrings.ElementAt(0), connectionStrings.ElementAt(1));

            var resultTree = comparator.CompareDatabases();
            var resultPath = Settings.Instance.SettingsObject.ResultPath;
            string xmlContent = xml.GetXml(resultTree);

            xml.SaveResultTree(resultPath,xmlContent);

            //List of all nodes within a Tree Structure
            var listofnodes = Extensions.DepthFirstTraversal(resultTree, r => r.Nodes).ToList();

            _logger.Info("Exiting application.");
        }
    }
}
