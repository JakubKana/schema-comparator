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
                Logger.Info("Executing transaction script.");
                var result = db.Execute($"CREATE DATABASE {dbName}");
                Logger.Info("Transaction Successful.");
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Cannot execute SQL command.");
                db.AbortTransaction();
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to create database.");
                db.AbortTransaction();
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
        }

        public void DeleteDatabase(Database db, string dbName)
        {
            try
            {
                Logger.Info($"Executing DROP DATABASE {dbName} script.");
                var result = db.Execute($"DROP DATABASE {dbName}");
                Logger.Info($"DROP DATABASE {dbName} Successful.");
            }
            catch (SqlException ex)
            {
                Logger.Error(ex, "Cannot execute SQL command.");
                db.AbortTransaction();
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to create database.");
                db.AbortTransaction();
                Environment.Exit((int)ExitCodes.UnexpectedError);
            }
        }

        public bool CheckDatabaseExists(Database db, string databaseName)
        {
            Logger.Info($"Checking if database exists {databaseName}");
            long? result;
            
            result = db.Single<long?>($"SELECT db_id('{databaseName}')");
            Logger.Debug($"Database {databaseName} result {result}");
            
            return result.HasValue;
        }

        public void DeployMsScript(string pathToScript, TestNodes mainTestNode, MsSqlDatabaseHandler db)
        {
            try
            {
                var msScriptParser = new MsSqlScriptParser();
                var deployTestNode = ObjectComparator.CreateTestNode(new List<TestResult>(), ObjectType.ScriptTests,
                    "Set of tests for Script");

                var scriptFromFile = File.ReadAllText(Path.Combine(pathToScript));
                var parsedScript = msScriptParser.GetMsScriptArray(scriptFromFile);
                var dbCreated = msScriptParser.ExecuteTransactionScript(parsedScript, db.Database);

                if (dbCreated) return;

                ObjectComparator.AddTestResult($"Creation of Database objects failed. Databaes: {db.Database.ConnectionString}",
                    ErrorTypes.CreationScriptFailed,
                    ObjectType.Script,
                    $"Script: {pathToScript}",
                    deployTestNode.Results);
                ObjectComparator.SetResultLevel(deployTestNode);
                mainTestNode.Nodes.Add(deployTestNode);

                var resultPath = Settings.Instance.SettingsObject.ResultPath;
                Xml xmlCreator = new Xml();
                var xmlContent = xmlCreator.GetXml(mainTestNode);
                xmlCreator.SaveResultTree(resultPath, xmlContent);

                Environment.Exit((int)ExitCodes.ScriptFailed);
            }
            catch (IOException ex)
            {
                Logger.Error(ex, "Unable to finish script file.");
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
