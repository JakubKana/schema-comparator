using System.Data.SqlClient;

namespace DBSchemaComparator.Domain.Database
{
    class DatabaseHandler
    {
       

        public SqlConnection SqlConnection;

        public DatabaseHandler(string connectionString)
        {
           SqlConnection = new SqlConnection(connectionString);
        }

        

    }
}
