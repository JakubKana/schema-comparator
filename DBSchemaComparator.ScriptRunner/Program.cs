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
                    string databaseType = args[1].ToLower();
                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);

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
                                connStringBuilder1.Remove("");
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
                else if (args.Length == 3)
                {
                    // Read arguments
                    string connectionString = args[0];
                    

                    string databaseType = args[1];
                    DatabaseType dbType = BaseDatabase.GetDatabaseType(databaseType);

                    string pathToScript = args[2];

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
