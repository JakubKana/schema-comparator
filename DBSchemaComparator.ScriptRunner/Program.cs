using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.General;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            
            if (args.Length == 3)
            {
                string databaseType = args[2].ToLower();
                DatabaseType dbType = GetDatabaseType(databaseType);
                SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder(args[0]);
                
                
                if (dbType == DatabaseType.Unsupported)
                {
                    Logger.Error(new ArgumentException(), $"Unsupported database type {databaseType}");
                    Environment.Exit((int)ExitCodes.UnsupportedDbType);
                }
                DatabaseHandler db = new DatabaseHandler(stringBuilder.ConnectionString, dbType);

                if ()
                
                
                
               

            }
            {
                Logger.Info(new ArgumentException(), $"Invalid input argument count [{args.Length}]. Exactly arguments 3 required.");
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
