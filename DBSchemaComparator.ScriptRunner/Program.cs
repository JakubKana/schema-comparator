using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using DBSchemaComparator.Domain.Models.SQLServer;
using DBSchemaComparator.Domain.Models.Test;
using DBSchemaComparator.ScriptRunner.Parser;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            
            if (args.Length == 3)
            {
                string databaseType = args[1].ToLower();

                DatabaseType dbType = GetDatabaseType(databaseType);

                string connectionString = args[0];

                string pathToScript = args[2];


                var mainTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.Root, "Root node");

                if (dbType == DatabaseType.Unsupported)
                {
                    Logger.Error(new ArgumentException(), $"Unsupported database type {databaseType}");
                    Environment.Exit((int)ExitCodes.UnsupportedDbType);
                }

                DatabaseHandler db = new DatabaseHandler(connectionString, dbType);

                try
                {
                    var parsedScript = ScriptParser.GetMsScriptArray(pathToScript);

                    var scriptFromFile = File.ReadAllText(Path.Combine(pathToScript));

                    var dbCreated = ScriptParser.ExecuteTransactionScript(parsedScript, db.Database);

                }
                catch (IOException ex)
                {
                    Logger.Error(ex, "Unable to load script file.");
                    Environment.Exit((int)ExitCodes.ScriptFailed);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error when executing script file.");
                    Environment.Exit((int)ExitCodes.ScriptFailed);
                }








                //if (!leftDbCreated)
                //{
                //    AddTestResult("Creation of Left Database objects failed",ErrorTypes.CreationScriptFailed, ObjectType.Script, "Create Script",mainTestNode.Results);
                //}

                //if (!rightDbCreated)
                //{
                //    AddTestResult("Creation of Right Database objects failed", ErrorTypes.CreationScriptFailed, ObjectType.Script, "Create Script", mainTestNode.Results);
                //}

                //AddTestResult("Deploying of Left Database objects success",
                //        ErrorTypes.CreationScriptSuccess,
                //        ObjectType.Script,
                //        LeftDatabase.Database.ConnectionString,
                //        mainTestNode.Results);

                //AddTestResult("Deploying of Right Database objects success",
                //    ErrorTypes.CreationScriptSuccess,
                //    ObjectType.Script,
                //    LeftDatabase.Database.ConnectionString,
                //    mainTestNode.Results);

                Environment.Exit((int)ExitCodes.Success);
            }
            else if (args.Length == 3 && args[0].ToLower() == "delete")
            {



                Environment.Exit((int)ExitCodes.Success);
            } 
            else
            {
                Logger.Info(new ArgumentOutOfRangeException(), $"Invalid input argument count [{args.Length}]. Exactly arguments 3 required.");
                Environment.Exit((int)ExitCodes.InvalidArguments);
            }

        }

        

        private static DatabaseType GetDatabaseType(string dataType)
        {
            switch (dataType)
            {
                case "mssql":
                    return DatabaseType.SqlServer;
                case "mysql":
                    return DatabaseType.MySql;
                default:
                   return DatabaseType.Unsupported;
            }
        }
    }
}
