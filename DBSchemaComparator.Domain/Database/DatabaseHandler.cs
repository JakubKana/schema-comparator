using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using NLog;

namespace DBSchemaComparator.Domain.Database
{
    public class DatabaseHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SqlConnection SqlConnection
        {
            get; private set; 
        }

        private DatabaseType _dbType;


        public DatabaseHandler(string connectionString, DatabaseType databaseType)
        {
            CreateDatabaseConnection(connectionString, databaseType);
        }

        private void CreateDatabaseConnection(string connectionString, DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    _dbType = databaseType;
                    SqlConnection = new SqlConnection(connectionString);
                    break;
                case DatabaseType.MySql:
                    _dbType = databaseType;
                    // To-Do
                    break;
                default:
                    _dbType = databaseType;
                    SqlConnection = new SqlConnection(connectionString);
                    break;
            }
        }

        public void SelectTablesSchemaInfo(string tableName = null)
        {
            Logger.Info($"Querying schema information about tables.");

            var sqlQuery = new StringBuilder();
            sqlQuery.Append(@"SELECT * FROM [INFORMATION_SCHEMA].[TABLES]");
            #region DatabaseSelect
            try
            {
                using (SqlConnection)
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlQuery.ToString(), SqlConnection);
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        sqlQuery.Append(" WHERE TABLE_NAME = @TABLE_NAME");
                        SqlParameter sqlParameter = sqlCommand.CreateParameter();
                        sqlParameter.ParameterName = "@TABLE_NAME";
                        sqlParameter.Value = tableName;
                        sqlCommand.Parameters.AddWithValue("@TABLE_NAME", "%" + tableName + "%");
                    }

                    SqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            Console.WriteLine("{0}\t{1}", sqlDataReader.GetString(2), sqlDataReader.GetString(3));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    sqlDataReader.Close();
                }

            }
            catch (SqlException exception)
            {
                Logger.Warn(exception, "Unable to retrieve basic schema information from database.");
            }
            catch (Exception exception)
            {
                Logger.Warn(exception);
            }
            #endregion

        }


        public void SelectColumnsSchemaInfo(string tableName, InformationType infoType)
        {
            Logger.Info($"Selecting basic schema information from database");
           
            var sqlQuery = new StringBuilder();
            sqlQuery.Append(@"SELECT * FROM [INFORMATION_SCHEMA].[COLUMNS]");
            #region DatabaseSelect
            try
            {
                using (SqlConnection)
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlQuery.ToString(), SqlConnection);
                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        sqlQuery.Append(" WHERE TABLE_NAME = @TABLE_NAME");
                        SqlParameter sqlParameter = sqlCommand.CreateParameter();
                        sqlParameter.ParameterName = "@TABLE_NAME";
                        sqlParameter.Value = tableName;
                        sqlCommand.Parameters.AddWithValue("@TABLE_NAME", "%" + tableName + "%");
                    }

                    SqlConnection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.HasRows)
                    {
                        while (sqlDataReader.Read())
                        {
                            Console.WriteLine("{0}\t{1}", sqlDataReader.GetString(2), sqlDataReader.GetString(3));
                        }
                    }
                    else
                    {
                           Console.WriteLine("No rows found.");
                    }
                    sqlDataReader.Close();
                }

            }
            catch (SqlException exception)
            {
                Logger.Warn(exception, "Unable to retrieve basic schema information from database.");
            }
            catch (Exception exception)
            {
                Logger.Warn(exception);
            }
            #endregion

        }

        public bool IsConnect()
        {
            return SqlConnection != null && SqlConnection.State != ConnectionState.Closed;
        }
    }
}
