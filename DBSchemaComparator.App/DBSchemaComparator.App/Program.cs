using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using NLog;
using Extensions = DBSchemaComparator.Domain.Infrastructure.Extensions;


namespace DBSchemaComparator.App
{
    public class Program
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Logger.Info("Starting a Schema comparator application.");

            List<string> connectionStrings;

            if (args.Length == 0)
            {
                connectionStrings = Settings.GetDatabaseConnectionStrings(Settings.Instance.SettingsObject);
            }
            else
            {
                new Settings(args[0]);
                connectionStrings = Settings.GetDatabaseConnectionStrings(Settings.Instance.SettingsObject);
            }
            
            Xml xml = new Xml();
            
            // List<string> stringList = new List<string>();
            // stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest1;Integrated Security=True");
            // stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest2;Integrated Security=True");
            // var comparator = new ObjectComparator(stringList.ElementAt(0), stringList.ElementAt(1));

            var comparator = new ObjectComparator(connectionStrings.ElementAt(0), connectionStrings.ElementAt(1), DatabaseType.SqlServer);

            var resultTree = comparator.CompareDatabases();

            var resultPath = Settings.Instance.SettingsObject.ResultPath;

            string xmlContent = xml.GetXml(resultTree);

            xml.SaveResultTree(resultPath, xmlContent);

            // List of all nodes within a Tree Structure
            // var listofnodes = Extensions.DepthFirstTraversal(resultTree, r => r.Nodes).ToList();

            Logger.Info("Exiting application.");
            Environment.Exit((int)ExitCodes.Success);
        }
    }
}
