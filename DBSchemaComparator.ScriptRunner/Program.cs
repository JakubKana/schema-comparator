using System;
using System.Collections.Generic;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using DBSchemaComparator.Domain.Models.Test;
using DBSchemaComparator.ScriptRunner.Deployment;
using NLog;

namespace DBSchemaComparator.ScriptRunner
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Settings _settings;
        static void Main(string[] args)
        {

            //Create Test Tree
            var mainTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.Root, "Root node");
            try
            {
                if (args.Length == 4 && args[2].ToLower() == "delete")
                {
                    // Read arguments
                    string connectionString = args[0];
                    string databaseType = args[1].ToLower();
                    string resultPath = args[3];
                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);
                    _settings = new Settings(connectionString, resultPath, databaseType);
                    try
                    {
                        switch (dbType)
                        {
                            case DatabaseType.SqlServer:
                                var deploy = new MsSqlDeploy();
                                var connStringBuilder = Settings.GetMsSqlStringBuilder(connectionString);
                                var dbName = connStringBuilder.InitialCatalog;
                                connStringBuilder.Remove("Initial Catalog");
                                var connStringWithoutCatalog = connStringBuilder.ConnectionString;
                                deploy.DeleteDatabase(dbName, connStringWithoutCatalog, dbType);
                                break;

                            case DatabaseType.MySql:
                                var deploy1 = new MySqlDeploy();
                                var connStringBuilder1 = Settings.GetMySqlStringBuilder(connectionString);
                                var dbName1 = connStringBuilder1.Database;
                                var connStringWithoutCatalog1 = connStringBuilder1.ConnectionString;
                                deploy1.DeleteDatabase(dbName1, connStringWithoutCatalog1, dbType);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Logger.Error(ex, $"Unsupported database type {databaseType}");
                        Environment.Exit((int) ExitCodes.InvalidArguments);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Unexpected exception occured.");
                        Environment.Exit((int) ExitCodes.UnsupportedDbType);
                    }

                    Environment.Exit((int) ExitCodes.Success);
                }
                else if (args.Length == 4)
                {
                    Logger.Info("Running deploying script task.");
                    // Read arguments
                    string connectionString = args[0];
                    

                    string databaseType = args[1];
                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);

                    string pathToScript = args[2];
                    string resultPath = args[3];

                    Logger.Debug($"Arguments passed \narg[0]:{connectionString} \narg[1]:{databaseType} \narg[2]:{pathToScript} \narg[3]:{resultPath}.");

                    _settings = new Settings(connectionString, resultPath, databaseType);
                    try
                    {
                        switch (dbType)
                        {
                            case DatabaseType.SqlServer:
                                var deploy = new MsSqlDeploy();
                                var connStringBuilder = Settings.GetMsSqlStringBuilder(connectionString);
                                var dbName = connStringBuilder.InitialCatalog;
                                connStringBuilder.Remove("Initial Catalog");
                                var connStringWithoutCatalog = connStringBuilder.ConnectionString;
                                deploy.DeployDatabase(mainTestNode, connectionString, dbName, connStringWithoutCatalog, dbType, pathToScript);
                                break;
                            case DatabaseType.MySql:
                                var deploy1 = new MySqlDeploy();
                                var connStringBuilder1 = Settings.GetMySqlStringBuilder(connectionString);
                                var dbName1 = connStringBuilder1.Database;
                                connStringBuilder1.Remove("Database");
                                var connStringWithoutCatalog1 = connStringBuilder1.ConnectionString;
                                deploy1.DeployDatabase(mainTestNode, connectionString, dbName1, connStringWithoutCatalog1, dbType, pathToScript);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        Logger.Info("Task successful.");
                        Environment.Exit((int) ExitCodes.Success);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Logger.Error(ex, $"Unsupported database type {databaseType}");
                        Environment.Exit((int) ExitCodes.InvalidArguments);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Unexpected exception occured.");
                        Environment.Exit((int) ExitCodes.UnexpectedError);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    $"Invalid input argument count [{args.Length}]. Exactly arguments 3 required.");
                Environment.Exit((int) ExitCodes.InvalidArguments);
            }
        }

        
    }
}
