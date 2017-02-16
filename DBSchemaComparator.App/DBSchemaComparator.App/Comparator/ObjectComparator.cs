using System.Collections.Generic;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Models.Test;

namespace DBSchemaComparator.App.Comparator
{
    public class ObjectComparator
    {
        private string ConnStringLeft { get; set; }
        private string ConnStringRight { get; set; }

        public DatabaseType DatabaseType
        {
            get { return DatabaseType; }
            private set { DatabaseType = value; }
        }
        public DatabaseHandler LeftDatabase { get; set; }
        public DatabaseHandler RightDatabase { get; set; }

        public List<TestResult> TestResults = new List<TestResult>();

        public ObjectComparator(string connStringLeft, string connStringRight)
        {
            ConnStringLeft = connStringLeft;
            ConnStringRight = connStringRight;
            ConnectToDatabases(ConnStringLeft, ConnStringRight);
        }

        public ObjectComparator(string connStringLeft,string connStringRight, DatabaseType dbType) : this(connStringLeft, connStringRight)
        {
            DatabaseType = dbType;
        }

        public void ConnectToDatabases(string connStringLeft, string connStringRight)
        {
            
            LeftDatabase = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            RightDatabase = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);
        }

        public void TestTables()
        {
            var leftDatabaseTables = LeftDatabase.GetTablesSchemaInfo();
            var rightDatabaseTables = RightDatabase.GetTablesSchemaInfo();



        }


        public void TestProcedures(List<TestResult> testResults)
        {
            
        }
        public void TestIntegrityConstraints(List<TestResult> testResults)
        {
            
        }



    }
}
