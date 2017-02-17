using System.Collections.Generic;
using System.Linq;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.SQLServer;
using DBSchemaComparator.Domain.Models.Test;
using NLog;

namespace DBSchemaComparator.App.Comparator
{
    public class ObjectComparator
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string ConnStringLeft { get; set; }
        private string ConnStringRight { get; set; }

        public DatabaseType DatabaseType
        {
            get { return DatabaseType; }
            private set { DatabaseType = value; }
        }
        public DatabaseHandler LeftDatabase { get; set; }
        public DatabaseHandler RightDatabase { get; set; }

      //  public List<TestResult> TestResults = new List<TestResult>();

        public ObjectComparator(string connStringLeft, string connStringRight)
        {
            ConnStringLeft = connStringLeft;
            ConnStringRight = connStringRight;
            ConnectToDatabases(ConnStringLeft, ConnStringRight);
        }

        public ObjectComparator(string connStringLeft, string connStringRight, DatabaseType dbType) : this(connStringLeft, connStringRight)
        {
            DatabaseType = dbType;
        }
        private static TestNodes CreateTestNode(List<TestNodes> childNodes, List<TestResult> testResults)
        {
            var node = new TestNodes
            {
                Nodes = childNodes,
                Results = testResults
            };
            return node;
        }
        public void ConnectToDatabases(string connStringLeft, string connStringRight)
        {
            var mainTestNode = new TestNodes
            {
                Nodes = new List<TestNodes>(),
                Results = null 
            };

            LeftDatabase = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            RightDatabase = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);

            //Test Tables
            var tablesTestNode = TestTables();
            mainTestNode.Nodes.Add(tablesTestNode);
            //Test Indexes
          //var result = Extensions.DepthFirstTraversal(mainTestNode, nodes => nodes.Nodes);
            //Test 
        }

        public TestNodes TestTables()
        {
            var tablesTestsNode = CreateTestNode(new List<TestNodes>(), null);
            
            Logger.Info("Begin TestTables method.");

            var leftDatabaseTables = LeftDatabase.GetTablesSchemaInfo().ToList();
            var rightDatabaseTables = RightDatabase.GetTablesSchemaInfo().ToList();

            foreach (var leftDatabaseTable in leftDatabaseTables)
            {
                var leftTableName = leftDatabaseTable.TableName.ToLower();
                var rightTable = rightDatabaseTables.FirstOrDefault(r => r.TableName.ToLower() == leftTableName);
               
                if (rightTable == null)
                {
                    var testNode = CreateTestNode(null, new List<TestResult>());
                    AddTestResult("Table missing.", ErrorTypes.LpresentRmissing, ObjectType.Table, leftTableName, testNode.Results);
                    tablesTestsNode.Nodes.Add(testNode);
                }
                else
                {
                    var testNode = CreateTestNode(new List<TestNodes>(), new List<TestResult>());
                    AddTestResult("Tables present.", ErrorTypes.LpresentRpresent, ObjectType.Table, leftTableName, testNode.Results);
                    var testColumnsNode = TestColumns(leftDatabaseTable, rightTable);
                    testNode.Nodes.Add(testColumnsNode);
                    tablesTestsNode.Nodes.Add(testNode);
                }
            }
            foreach (var rightDatabaseTable in rightDatabaseTables)
            {
                var rightTableName = rightDatabaseTable.TableName.ToLower();
                var leftTable = leftDatabaseTables.FirstOrDefault(l => l.TableName.ToLower() == rightTableName);
                if (leftTable == null)
                {
                    var testNode = CreateTestNode(null, new List<TestResult>());
                    AddTestResult("Table missing.", ErrorTypes.LmissingRpresent, ObjectType.Table, rightTableName, testNode.Results);
                    tablesTestsNode.Nodes.Add(testNode);
                }
                else
                {
                    var testNode = CreateTestNode(new List<TestNodes>(), new List<TestResult>());
                    AddTestResult("Tables present.", ErrorTypes.LpresentRpresent, ObjectType.Table, rightTableName, testNode.Results);
                    var testColumnsNode = TestColumns(leftTable, rightDatabaseTable);
                    testNode.Nodes.Add(testColumnsNode);
                    tablesTestsNode.Nodes.Add(testNode);
                }
            }
            Logger.Info($"End TestTables method.");
            return tablesTestsNode;
        }

       

        private TestNodes TestColumns(Table leftTable, Table rightTable)
        {
            Logger.Info($"Start testing columns for tables L:{leftTable.TableName} R:{rightTable.TableName}");
            var testColumnsNode = new TestNodes
            {
                Nodes = null,
                Results = new List<TestResult>()
            };
            foreach (var leftTableColumn in leftTable.Columns)
            {
                var rightTableColumn = rightTable.Columns.FirstOrDefault(r => r.ColumnName == leftTableColumn.ColumnName);
                if (rightTableColumn == null)
                {
                    AddTestResult("Column missing.", ErrorTypes.LpresentRmissing, ObjectType.Column, leftTableColumn.ColumnName, testColumnsNode.Results);
                }
                else
                {
                    AddTestResult("Column present.", ErrorTypes.LpresentRpresent, ObjectType.Column, leftTableColumn.ColumnName, testColumnsNode.Results);
                    
                    //Check columns
                }
            }
            foreach (var rightTableColumn in rightTable.Columns)
            {
                var leftTableColumn = leftTable.Columns.FirstOrDefault(l => l.ColumnName == rightTableColumn.ColumnName);
                if (leftTableColumn == null)
                {
                    AddTestResult("Column missing.", ErrorTypes.LmissingRpresent, ObjectType.Column, rightTableColumn.ColumnName, testColumnsNode.Results);

                }
                else
                {
                    AddTestResult("Column present.", ErrorTypes.LpresentRpresent, ObjectType.Column, rightTableColumn.ColumnName, testColumnsNode.Results);
                    //Check Columns
                   
                }
            }
            Logger.Info($"Returning TestNodes for testing columns.");
            return testColumnsNode;
        }


        public void TestProcedures()
        {

        }

        public void TestIndexes()
        {
            
        }



        public void TestIntegrityConstraints()
        {

        }

        private void AddTestResult(string description, ErrorTypes errorType, ObjectType objType, string testedObjName, List<TestResult> testResults)
        {
            var matchingvalues = testResults.FirstOrDefault(test =>
            test.Description.Contains(description) &&
            test.ErrorType == errorType &&
            test.TestedObjectName == testedObjName &&
            test.ObjectType == objType);

            if (matchingvalues != null) return;

            Logger.Info($"Adding TestResult for {objType} of name {testedObjName}");

            TestResult testResult = new TestResult
            {
                Description = description,
                ErrorType = errorType,
                ObjectType = objType,
                TestedObjectName = testedObjName
            };
            testResults.Add(testResult);
        }

    }
}
