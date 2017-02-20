using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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

       

        public void ConnectToDatabases(string connStringLeft, string connStringRight)
        {
            var mainTestNode = new TestNodes
            {
                Nodes = new List<TestNodes>(),
                Results = null,
                Description = "Root node",
                NodeType = NodeType.Root
            };

            LeftDatabase = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            RightDatabase = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);

            //Test Tables
            var tablesTestNode = TestTables();
            mainTestNode.Nodes.Add(tablesTestNode);
            //Test Indexes
            var indexesTestNode = TestIndexes();

            //Test StoredProcedures
            var spTestNode = TestProcedures();
            mainTestNode.Nodes.Add(spTestNode);

            //Test Functions
            var functionsTestNode = TestFunctions();
            mainTestNode.Nodes.Add(functionsTestNode);

            //List of all nodes within a Tree Structure
            var listofnodes = Extensions.DepthFirstTraversal(mainTestNode, r => r.Nodes).ToList();
        }

        private TestNodes TestFunctions()
        {
            var functionsTestNode = CreateTestNode(new List<TestResult>(), )

        }

        public TestNodes TestTables()
        {
            var tablesTestsNode = CreateTestNode(null, NodeType.TablesTests, "Set of tests for tables");
            
            Logger.Info("Begin TestTables method.");

            var leftDatabaseTables = LeftDatabase.GetTablesSchemaInfo().ToList();
            var rightDatabaseTables = RightDatabase.GetTablesSchemaInfo().ToList();

            foreach (var leftDatabaseTable in leftDatabaseTables)
            {
                LeftDatabaseTests(tablesTestsNode, rightDatabaseTables, leftDatabaseTable);
            }
            foreach (var rightDatabaseTable in rightDatabaseTables)
            {
                RightDatabaseTests(tablesTestsNode, leftDatabaseTables, rightDatabaseTable);
            }
            Logger.Info($"End TestTables method.");
            return tablesTestsNode;
        }

        private void RightDatabaseTests(TestNodes tablesTestsNode, List<Table> leftDatabaseTables, Table rightDatabaseTable)
        {
            var rightTableName = rightDatabaseTable.TableName.ToLower();
            var leftTable = leftDatabaseTables.FirstOrDefault(l => l.TableName.ToLower() == rightTableName);
            if (leftTable == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.Table, $"Test for Table {rightDatabaseTable.TableName}");
                AddTestResult("ERROR", ErrorTypes.LmissingRpresent, ObjectType.Table, $"Testing table L: missing R: {rightDatabaseTable.TableName}", testNode.Results);
                tablesTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.Table, $"Test for Table {rightDatabaseTable.TableName}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Testing table L: {leftTable.TableName} R: {rightDatabaseTable.TableName}", testNode.Results);
                var testColumnsNode = TestColumns(leftTable, rightDatabaseTable);
                testNode.Nodes.Add(testColumnsNode);
                tablesTestsNode.Nodes.Add(testNode);
            }
        }

        private void LeftDatabaseTests(TestNodes tablesTestsNode, List<Table> rightDatabaseTables, Table leftDatabaseTable)
        {
            var leftTableName = leftDatabaseTable.TableName.ToLower();
            var rightTable = rightDatabaseTables.FirstOrDefault(r => r.TableName.ToLower() == leftTableName);

            if (rightTable == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.Table, $"Test for Table {leftDatabaseTable.TableName}");
                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Table, $"Testing table L: {leftDatabaseTable.TableName} R: missing", testNode.Results);
                tablesTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.Table, $"Test for Table {leftDatabaseTable.TableName}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Testing table L: {leftDatabaseTable.TableName} R: {rightTable.TableName}", testNode.Results);
                var testColumnsNode = TestColumns(leftDatabaseTable, rightTable);
                testNode.Nodes.Add(testColumnsNode);
                tablesTestsNode.Nodes.Add(testNode);
            }
        }


        private TestNodes TestColumns(Table leftTable, Table rightTable)
        {
            Logger.Info($"Start testing columns for tables L:{leftTable.TableName} R:{rightTable.TableName}");
            var testColumnsNode = CreateTestNode(new List<TestResult>(), NodeType.ColumnsTests, "Set of tests for Columns"); 

            foreach (var leftTableColumn in leftTable.Columns)
            {
                var columnNode = CreateTestNode(new List<TestResult>(), NodeType.Column,
                    $"Column {leftTableColumn.TableName}.{leftTableColumn.ColumnName}");
                var rightTableColumn = rightTable.Columns.FirstOrDefault(r => r.ColumnName == leftTableColumn.ColumnName);
                if (rightTableColumn == null)
                {
                    AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Column, $"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: misssing", columnNode.Results);
                }
                else
                {
                    AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Column, $"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", columnNode.Results);
                    //Check column DataTypes
                    CheckColumnType(leftTableColumn, rightTableColumn, columnNode.Results);
                    //Check column IsNullable
                    CheckColumnIsNullable(leftTableColumn, rightTableColumn, columnNode.Results);
                    //Check Identity settings
                    CheckColumnIdentity(leftTableColumn, rightTableColumn, columnNode.Results);
                }
                testColumnsNode.Nodes.Add(columnNode);
            }
            foreach (var rightTableColumn in rightTable.Columns)
            {
                var columnNode = CreateTestNode(new List<TestResult>(), NodeType.Column,
                  $"Column {rightTableColumn.TableName}.{rightTableColumn.ColumnName}");

                var leftTableColumn = leftTable.Columns.FirstOrDefault(l => l.ColumnName == rightTableColumn.ColumnName);
                if (leftTableColumn == null)
                {
                    AddTestResult("ERROR", ErrorTypes.LmissingRpresent, ObjectType.Column, $"Columns L: missing R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", columnNode.Results);
                }
                else
                {
                    AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Column, $"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", columnNode.Results);
                    //Check Columns
                    CheckColumnType(leftTableColumn, rightTableColumn, columnNode.Results);
                    //Check column IsNullable
                    CheckColumnIsNullable(leftTableColumn, rightTableColumn, columnNode.Results);
                    //Check Identity settings
                    CheckColumnIdentity(leftTableColumn, rightTableColumn, columnNode.Results);

                }
                testColumnsNode.Nodes.Add(columnNode);
            }
            Logger.Info($"Returning TestNodes for testing columns.");
            return testColumnsNode;
        }

        private void CheckColumnIdentity(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            if (leftTableColumn.IsIdentification == rightTableColumn.IsIdentification)
            {
                AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.IsIdentification,
                  $"Identification setting for tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.IsIdentification} " +
                  $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.IsIdentification}",
                  testResults);
            }
            else
            {
                AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.IsIdentification,
                 $"Identification setting for tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.IsIdentification} " +
                 $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.IsIdentification}",
                 testResults);
            }
            
        }

        private void CheckColumnIsNullable(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            if (leftTableColumn.IsNullable == rightTableColumn.IsNullable)
            {
                AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.IsNullable,
                    $"IsNullable setting for tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.IsNullable} " +
                    $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.IsNullable}",
                    testResults);
            }
            else
            {
                AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.IsNullable,
    $"IsNullable setting for tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.IsNullable} " +
    $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.IsNullable}",
    testResults);
            }

        }

        private void CheckColumnType(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
                if (leftTableColumn.DataType == rightTableColumn.DataType) {
                    AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.DataType, $"DataTypes of tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.DataType} " +
                                                                                      $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.DataType}",testResults);
                } else {
                    AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.DataType, $"DataTypes of tested columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName}.{leftTableColumn.DataType} " +
                                                                                     $"R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}.{rightTableColumn.DataType}", testResults);
                }
        }


        public TestNodes TestProcedures()
        {
            var proceduresTestsNode = CreateTestNode(null, NodeType.StoredProceduresTests, "Set of tests for stored procedures");

            var leftDatabaseSp = LeftDatabase.GetStoredProceduresInfo().ToList();
            var rightDatabaseSp = RightDatabase.GetStoredProceduresInfo().ToList();

            Logger.Info("Begin TestProcedures method.");

            foreach (var leftSp in leftDatabaseSp)
            {
                TestLeftDbProcedures(proceduresTestsNode, rightDatabaseSp, leftSp);
            }

            foreach (var rightSp in rightDatabaseSp)
            {
                TestRightDbProcedures(proceduresTestsNode, leftDatabaseSp, rightSp);
            }

            Logger.Info($"End TestTables method.");

            return proceduresTestsNode;
        }

        private void TestRightDbProcedures(TestNodes proceduresTestsNode, List<StoredProcedure> leftDatabaseSp, StoredProcedure rightSp)
        {
            var leftSp = leftDatabaseSp.FirstOrDefault(r => r.Name.ToLower() == rightSp.Name.ToLower());

            if (leftSp == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.StoredProcedure, $"Test for SP: {rightSp.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {rightSp.Name} R: missing", testNode.Results);

                proceduresTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.StoredProcedure, $"Test for SP {rightSp.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Testing Stored Procedure L: {leftSp.Name} R: {rightSp.Name}", testNode.Results);
                var testProceduresBodyNode = TestProceduresBodyColumns(leftSp, rightSp);
                testNode.Nodes.Add(testProceduresBodyNode);
                proceduresTestsNode.Nodes.Add(testNode);
            }
        }

        private void TestLeftDbProcedures(TestNodes proceduresTestsNode, List<StoredProcedure> rightDatabaseSp, StoredProcedure leftSp)
        {
            var rightSp = rightDatabaseSp.FirstOrDefault(r => r.Name.ToLower() == leftSp.Name.ToLower());

            if (rightSp == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.StoredProcedure, $"Test for SP: {leftSp.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {leftSp.Name} R: missing", testNode.Results);

                proceduresTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), NodeType.StoredProcedure, $"Test for SP: {leftSp.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {leftSp.Name} R: {rightSp.Name}", testNode.Results);
                var testProceduresBodyNode = TestProceduresBodyColumns(leftSp, rightSp);
                testNode.Nodes.Add(testProceduresBodyNode);
                proceduresTestsNode.Nodes.Add(testNode);
            }
        }

        private TestNodes TestProceduresBodyColumns(StoredProcedure leftSp, StoredProcedure rightSp)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), NodeType.StoredProcedure, "Test for Stored Procedure Body");
            var normalizedLeft = Normalize(leftSp.Body);
            var normalizedRight = Normalize(rightSp.Body);
            Logger.Info($"Testing stored procedures bodies L: {leftSp.Body} R: {rightSp.Body}");

            if (normalizedLeft == normalizedRight)
            {
                AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.StoredProcedure, $"Testing body of SP L: {leftSp.Name} R: {rightSp.Name}", bodyTest.Results);
            }
            else
            {
                AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.StoredProcedure, $"Testing body of SP L: {leftSp.Name} R: {rightSp.Name}", bodyTest.Results);
            }
            return bodyTest;
        }

        public void TestIndexes()
        {
            
        }



        public void TestIntegrityConstraints()
        {

        }

        private string Normalize(string text)
        {
            Logger.Debug($"Normalizing text:\n {text}");
            string pattern = @"(\s+)";
             
            var normalizedText = Regex.Replace(text, pattern, " ");

            Logger.Debug($"Normalized text:\n {normalizedText}");
            return normalizedText;
        }

        private static TestNodes CreateTestNode(List<TestResult> testResults, NodeType testType, string description)
        {
            var node = new TestNodes
            {
                Nodes = new List<TestNodes>(),
                Results = testResults,
                Description = description,
                NodeType = testType
            };
            return node;
        }


        private void AddTestResult(string description, ErrorTypes errorType, ObjectType objType, string testedObjName, List<TestResult> testResults)
        {
            var matchingvalues = testResults.FirstOrDefault(test =>
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
