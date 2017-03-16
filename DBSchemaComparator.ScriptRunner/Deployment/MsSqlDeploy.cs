using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.App.Comparator;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Models.General;
using DBSchemaComparator.Domain.Models.Test;
using DBSchemaComparator.ScriptRunner.Parser;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public class MsSqlDeploy : BaseDatabase, IDeployment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public void CreateDatabase(Database db, string dbName)
        {
            try
            {
                using (var dbContext = db)
                {
                    var result = dbContext.Execute($"CREATE DATABASE {dbName}");
                }
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Cannot execute SQL command.");
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to create database.");
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }

        }

        public bool CheckDatabaseExists(string databaseName)
        {
            Logger.Info($"Checking if database exists {databaseName}");
            using (var conn = Database)
            {
                var result = conn.Single<long?>($"SELECT db_id('{databaseName}')");

                Logger.Debug($"Database {databaseName} result {result}");

                return result.HasValue;
            }

        }

        public void DeployMsScript(string pathToScript, TestNodes mainTestNode, MsSqlDatabaseHandler db)
        {
            try
            {
                var deployTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.ScriptTests,
                    "Set of tests for Script");

                var scriptFromFile = File.ReadAllText(Path.Combine(pathToScript));
                var parsedScript = ScriptParser.GetMsScriptArray(scriptFromFile);
                var dbCreated = ScriptParser.ExecuteTransactionScript(parsedScript, db.Database);

                if (!dbCreated)
                {
                    ObjectComparator.AddTestResult($"Creation of Database objects failed. Databaes: {db.Database.ConnectionString}",
                    ErrorTypes.CreationScriptFailed,
                    ObjectType.Script,
                    $"Script: {pathToScript}",
                    deployTestNode.Results);
                }
                else
                {
                    ObjectComparator.AddTestResult($"Deploying of Database objects successful. Database: {db.Database.ConnectionString}",
                    ErrorTypes.CreationScriptSuccess,
                    ObjectType.Script,
                    $"Script: {pathToScript}",
                    deployTestNode.Results);
                }

                ObjectComparator.SetResultLevel(deployTestNode);
                mainTestNode.Nodes.Add(deployTestNode);
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
        }

    
    }
}
