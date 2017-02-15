using System.Collections.Generic;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Test;

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
        private DatabaseHandler LeftDatabase { get; set; }
        public DatabaseHandler RightDatabase { get; set; }

        private List<TestResult> _testResults = new List<TestResult>();

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



        public bool ConnectToDatabases(string connStringLeft, string connStringRight)
        {
            LeftDatabase = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            RightDatabase = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);

            if (LeftDatabase.IsAvailible() && RightDatabase.IsAvailible())
            {
                return true;
            }
            return false;

        }

        public void testTables()
        {
            
        }

    }
}
