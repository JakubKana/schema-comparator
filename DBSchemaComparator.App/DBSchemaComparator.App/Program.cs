using System;
using System.Collections.Generic;
using System.Linq;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using NLog;


namespace DBSchemaComparator.App
{
    public class Program
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static Settings _settings;

        static void Main(string[] args)
        {
            Logger.Info("Starting a Schema comparator application.");

            List<string> connectionStrings = null;

            try
            {
                if (args.Length == 0)
                {
                    _settings = new Settings();
                }
                else if (args.Length == 1)
                {
                    _settings = new Settings(args[0]);
                }
                else if (args.Length == 2)
                {
                    _settings = new Settings(args[0], args[1]);
                }
                else if (args.Length == 3)
                {
                    _settings = new Settings(args[0], args[1], args[2]);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
                connectionStrings = Settings.GetDatabaseConnectionStrings(Settings.SettingsObject);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.Error(ex, "Invalid arguments count");
                Environment.Exit((int)ExitCodes.InvalidArguments);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unexpected error");
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
        
            
            Xml xml = new Xml();

            // List<string> stringList = new List<string>();
            // stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest1;Integrated Security=True");
            // stringList.Add("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=DBComparatorTest2;Integrated Security=True");
            // var comparator = new ObjectComparator(stringList.ElementAt(0), stringList.ElementAt(1));

            var dbType = BaseDatabase.GetDatabaseType(Settings.SettingsObject.DatabaseConnections.First().DbType);

            var comparator = new ObjectComparator(connectionStrings.ElementAt(0), connectionStrings.ElementAt(1), dbType);

            var resultTree = comparator.CompareDatabases();

            var resultPath = Settings.SettingsObject.ResultPath;

            string xmlContent = xml.GetXml(resultTree);

            xml.SaveResultTree(resultPath, xmlContent);

            // List of all nodes within a Tree Structure
            // var listofnodes = Extensions.DepthFirstTraversal(resultTree, r => r.Nodes).ToList();

            Logger.Info("Exiting application.");
            Environment.Exit((int)ExitCodes.Success);
        }
    }
}
