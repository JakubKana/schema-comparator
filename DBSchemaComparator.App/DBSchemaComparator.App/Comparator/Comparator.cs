using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Test;

namespace DBSchemaComparator.App.Comparator
{
    public class Comparator
    {
        private string ConnStringLeft { get; set; }
        private string ConnStringRight { get; set; }

        public DatabaseType DatabaseType
        {
            get { return DatabaseType; }
            private set { DatabaseType = value; }
        }


        private List<TestResult> _testResults = new List<TestResult>();

        public Comparator(string connStringLeft, string connStringRight)
        {
            ConnStringLeft = connStringLeft;
            ConnStringRight = connStringRight;
        }

        public Comparator(string connStringLeft,string connStringRight, DatabaseType dbType) : this(connStringLeft, connStringRight)
        {
            DatabaseType = dbType;
        }



        public bool ConnectToDatabases()
        {
            var leftConnection = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            var rightConnection = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);

            var isLeftConnect = leftConnection.IsConnect();
            var isRightConnect = rightConnection.IsConnect();


            return true;
        }

    }
}
