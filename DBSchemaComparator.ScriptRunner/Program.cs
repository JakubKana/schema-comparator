﻿using System;
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

            //Create Test Tree
            var mainTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.Root, "Root node");
            try
            {
                if (args.Length == 3 && args[2].ToLower() == "delete")
                {
                    // Read arguments
                    string connectionString = args[0];

                    var connStringBuilder = Settings.GetMsSqlStringBuilder(connectionString);
                    var dbName = connStringBuilder.InitialCatalog;
                    connStringBuilder.Remove("Initial Catalog");

                    var connStringWithoutCatalog = connStringBuilder.ConnectionString;
                    string databaseType = args[1].ToLower();

                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);

                    try
                    {
                        switch (dbType)
                        {
                            case DatabaseType.SqlServer:
                                DeleteMsSqlDatabase(dbName, connStringWithoutCatalog, dbType);
                                break;
                            case DatabaseType.MySql:
                                throw new NotImplementedException();
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
                else if (args.Length == 3)
                {
                    // Read arguments
                    string connectionString = args[0];
                    var connStringBuilder = Settings.GetMsSqlStringBuilder(connectionString);
                    var dbName = connStringBuilder.InitialCatalog;
                    connStringBuilder.Remove("Initial Catalog");
                    var connStringWithoutCatalog = connStringBuilder.ConnectionString;

                    string databaseType = args[1];
                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);

                    string pathToScript = args[2];

                    try
                    {
                        switch (dbType)
                        {
                            case DatabaseType.SqlServer:
                                DeployMsSqlDatabase(mainTestNode, connectionString, dbName, connStringWithoutCatalog, dbType, pathToScript);
                                break;
                            case DatabaseType.MySql:

                                throw new NotImplementedException();
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
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

        private static void DeployMsSqlDatabase(TestNodes mainTestNode, string connectionString, string dbName, string connStringWithoutCatalog, DatabaseType dbType, string pathToScript)
        {
            MsSqlDatabaseHandler db = new MsSqlDatabaseHandler(connectionString, dbType);
            MsSqlDatabaseHandler db1 = new MsSqlDatabaseHandler(connStringWithoutCatalog, dbType);
            MsSqlDeploy deploy = new MsSqlDeploy();

            if (!deploy.CheckDatabaseExists(db1.Database, dbName))
            {
                deploy.CreateDatabase(db1.Database, dbName);
            }
            deploy.DeployMsScript(pathToScript, mainTestNode, db);
        }

        private static void DeleteMsSqlDatabase(string dbName, string connStringWithoutCatalog, DatabaseType dbType)
        {
            MsSqlDatabaseHandler db1 = new MsSqlDatabaseHandler(connStringWithoutCatalog, dbType);
            MsSqlDeploy deploy = new MsSqlDeploy();
            if (deploy.CheckDatabaseExists(db1.Database, dbName))
                deploy.DeleteDatabase(db1.Database, dbName);
        }
    }
}
