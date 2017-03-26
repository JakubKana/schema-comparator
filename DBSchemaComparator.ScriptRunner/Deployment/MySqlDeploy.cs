using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Models.General;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public class MySqlDeploy : BaseSqlDeploy, IDeployment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public override bool CheckDatabaseExists(Database db, string databaseName)
        {
            Logger.Info($"Checking if database exists {databaseName}");
            long? result;

            result = db.Single<long?>($"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='{databaseName}';");
            Logger.Debug($"Database {databaseName} result {result}");

            return result.HasValue;
        }

        public override void CreateDatabase(Database db, string dbName)
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

        public override void DeleteDatabase(Database db, string dbName)
        {
            try
            {
                Logger.Info($"Executing DROP DATABASE {dbName} script.");
                var result = db.Execute($"DROP DATABASE '{dbName}';");
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
    }
}
