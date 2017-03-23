using System;
using System.Data.SqlClient;
using DBSchemaComparator.Domain.Models.General;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public class BaseSqlDeploy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public virtual void CreateDatabase(Database db, string dbName)
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

        public virtual void DeleteDatabase(Database db, string dbName)
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

        public virtual bool CheckDatabaseExists(Database db, string databaseName)
        {
            Logger.Info($"Checking if database exists {databaseName}");
            long? result;

            result = db.Single<long?>($"SELECT db_id('{databaseName}')");
            Logger.Debug($"Database {databaseName} result {result}");

            return result.HasValue;
        }

    }
}