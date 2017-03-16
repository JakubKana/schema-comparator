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
using DBSchemaComparator.ScriptRunner.Deployment;
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
            Xml xmlCreator = new Xml();
            //Create Test Tree
            var mainTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.Root, "Root node");
            if (args.Length == 3)
            {
                // Read arguments
                string databaseType = args[1].ToLower();
                DatabaseType dbType = GetDatabaseType(databaseType);
                string connectionString = args[0];
                string pathToScript = args[2];
                
                try
                {
                    switch (dbType)
                    {
                        case DatabaseType.SqlServer:

                            MsSqlDatabaseHandler db = new MsSqlDatabaseHandler(connectionString, dbType);

                            MsSqlDeploy deploy = new MsSqlDeploy();

                            var dbName = Settings.GetMsSqlStringBuilder(db.Database.ConnectionString).InitialCatalog;
                            
                            if (!deploy.CheckDatabaseExists(dbName))
                            {
                                deploy.CreateDatabase(db.Database, dbName);
                            }

                            deploy.DeployMsScript(pathToScript, mainTestNode, db);

                            break;
                        case DatabaseType.MySql:
                            throw new NotImplementedException();
                           
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                        Environment.Exit((int)ExitCodes.Success);
                }

                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Error(ex, $"Unsupported database type {databaseType}");
                    Environment.Exit((int)ExitCodes.InvalidArguments);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Unexpected exception occured.");
                    Environment.Exit((int)ExitCodes.UnexpectedError);
                }

               
            }
            else if (args.Length == 3 && args[0].ToLower() == "delete")
            {
                // Read arguments
                string databaseType = args[2].ToLower();
                DatabaseType dbType = GetDatabaseType(databaseType);
                string connectionString = args[1];

                try
                {
                    switch (dbType)
                    {
                        case DatabaseType.SqlServer:
                            MsSqlDatabaseHandler db = new MsSqlDatabaseHandler(connectionString, dbType);
                            var dbName = Settings.GetMsSqlStringBuilder(db.Database.ConnectionString).InitialCatalog;

                            break;
                        case DatabaseType.MySql:


                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Error(ex, $"Unsupported database type {databaseType}");
                    Environment.Exit((int)ExitCodes.InvalidArguments);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Unexpected exception occured.");
                    Environment.Exit((int)ExitCodes.UnsupportedDbType);
                }

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
                    throw new ArgumentOutOfRangeException();
                   
            }
        }
    }
}
