﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
                NodeType = ObjectType.Root
            };

            LeftDatabase = new DatabaseHandler(ConnStringLeft, DatabaseType.SqlServer);
            RightDatabase = new DatabaseHandler(ConnStringRight, DatabaseType.SqlServer);

           
            var scriptFromFile = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Create.sql"));

            var parsedScript = ScriptParser.GetMSScriptArray(scriptFromFile);
            LeftDatabase.ExecuteTransactionScript(parsedScript);

            //Test Tables
            var tablesTestNode = TestTables();
            mainTestNode.Nodes.Add(tablesTestNode);
           
            //Test StoredProcedures
            var spTestNode = TestProcedures();
            mainTestNode.Nodes.Add(spTestNode);

            //Test Functions
            var functionsTestNode = TestFunctions();
            mainTestNode.Nodes.Add(functionsTestNode);

            // Test Indexes
            var indexesTestNode = TestIndexes();
            mainTestNode.Nodes.Add(indexesTestNode);

            // Test Views
            var viewsTestNode = TestViews();
            mainTestNode.Nodes.Add(viewsTestNode);

            // Test Integrity Constraints
            var integrityConstraintsNode = TestIntegrityConstraints();
            mainTestNode.Nodes.Add(integrityConstraintsNode);

            //List of all nodes within a Tree Structure
            var listofnodes = Extensions.DepthFirstTraversal(mainTestNode, r => r.Nodes).ToList();
        }

        private TestNodes TestViews()
        {
            Logger.Info("Begin TestViews");
            var viewsTestNode = CreateTestNode(null, ObjectType.ViewsTests, "Set of tests for views");

            var leftDbViews = LeftDatabase.GetViewsInfo();
            var rightDbViews = RightDatabase.GetViewsInfo();

            foreach (var leftDbView in leftDbViews)
            {
                TestLeftDbViews(viewsTestNode, rightDbViews, leftDbView);
            }
            foreach (var rightDbView in rightDbViews)
            {
                TestRightDbViews(viewsTestNode, leftDbViews, rightDbView);
            }

            Logger.Info("End TestViews method.");
            return viewsTestNode;
        }

        private void TestRightDbViews(TestNodes viewsTestNode, IList<View> leftDbViews, View rightDbView)
        {
            var leftViews = leftDbViews.FirstOrDefault(r => string.Equals(r.Name, rightDbView.Name, StringComparison.CurrentCultureIgnoreCase));

            if (leftViews == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View: {rightDbView.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.View, $"Testing View L: missing R: {rightDbView.Name}", testNode.Results);

                viewsTestNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View {rightDbView.Name}");

                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.View, $"Testing View L: {leftViews.Name} R: {rightDbView.Name}", testNode.Results);
                var testProceduresBodyNode = TestViewBody(leftViews, rightDbView);

                testNode.Nodes.Add(testProceduresBodyNode);
                viewsTestNode.Nodes.Add(testNode);
            }
        }

        private void TestLeftDbViews(TestNodes viewsTestNode, IList<View> rightDbViews, View leftDbView)
        {
            var rightViews = rightDbViews.FirstOrDefault(r => string.Equals(r.Name, leftDbView.Name, StringComparison.CurrentCultureIgnoreCase));

            if (rightViews == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View: {leftDbView.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.View, $"Testing View L: {leftDbView.Name} R: missing", testNode.Results);

                viewsTestNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View: {leftDbView.Name}");

                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.View, $"Testing View L: {leftDbView.Name} R: {rightViews.Name}", testNode.Results);
                var testFunctionBodyNode = TestViewBody(leftDbView, rightViews);

                testNode.Nodes.Add(testFunctionBodyNode);
                viewsTestNode.Nodes.Add(testNode);
            }
        }

        private TestNodes TestFunctions()
        {
            Logger.Info("Begin TestFunctions method.");

            var functionsTestNode = CreateTestNode(null, ObjectType.TablesTests, "Set of tests for functions" );

            var leftDbFunctions = LeftDatabase.GetFunctionsInfo().ToList();
            var rightDbFunctions = RightDatabase.GetFunctionsInfo().ToList();

            foreach (var leftFunctions in leftDbFunctions)
            {
                TestLeftDbFunctions(functionsTestNode, rightDbFunctions, leftFunctions);
            }

            foreach (var rightFunction in rightDbFunctions)
            {
                TestRightDbFunctions(functionsTestNode, leftDbFunctions, rightFunction);
            }

            Logger.Info("End TestFunctions method.");
            return functionsTestNode;
        }

        private void TestRightDbFunctions(TestNodes functionsTestNode, List<Function> leftDbFunctions, Function rightFunction)
        {
            var leftFunction = leftDbFunctions.FirstOrDefault(r => string.Equals(r.Name, rightFunction.Name, StringComparison.CurrentCultureIgnoreCase));

            if (leftFunction == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function: {rightFunction.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Function, $"Testing Function L: missing R: {rightFunction.Name}", testNode.Results);

                functionsTestNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function {rightFunction.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Function, $"Testing Function L: {leftFunction.Name} R: {rightFunction.Name}", testNode.Results);
                var testProceduresBodyNode = TestFunctionBody(leftFunction, rightFunction);
                testNode.Nodes.Add(testProceduresBodyNode);
                functionsTestNode.Nodes.Add(testNode);
            }
        }

        private void TestLeftDbFunctions(TestNodes functionsTestNode, List<Function> rightDbFunctions, Function leftFunctions)
        {
            var rightFunctions = rightDbFunctions.FirstOrDefault(r => string.Equals(r.Name, leftFunctions.Name, StringComparison.CurrentCultureIgnoreCase));

            if (rightFunctions == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function: {leftFunctions.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Function, $"Testing Functions L: {leftFunctions.Name} R: missing", testNode.Results);

                functionsTestNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function: {leftFunctions.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Function, $"Testing Functions L: {leftFunctions.Name} R: {rightFunctions.Name}", testNode.Results);
                var testFunctionBodyNode = TestFunctionBody(leftFunctions, rightFunctions);
                testNode.Nodes.Add(testFunctionBodyNode);
                functionsTestNode.Nodes.Add(testNode);
            }
        }

        private TestNodes TestFunctionBody(Function leftFunctions, Function rightFunctions)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.Function, "Test for Function Body");
            var normalizedLeft = Extensions.Normalize(leftFunctions.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightFunctions.Body).Trim();
            Logger.Info($"Testing functions bodies L: {leftFunctions.Body} R: {rightFunctions.Body}");

            if (normalizedLeft == normalizedRight)
            {
                AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.Function, $"Testing body of SP L: {leftFunctions.Name} R: {rightFunctions.Name}", bodyTest.Results);
            }
            else
            {
                AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.Function, $"Testing body of SP L: {leftFunctions.Name} R: {rightFunctions.Name}", bodyTest.Results);
            }
            return bodyTest;

        }

        private TestNodes TestViewBody(View leftView, View rightView)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.View, "Test for View Body");
            var normalizedLeft = Extensions.Normalize(leftView.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightView.Body).Trim();
            Logger.Info($"Testing views bodies L: {leftView.Body} R: {rightView.Body}");

            if (normalizedLeft == normalizedRight)
            {
                AddTestResult("SUCCESS", ErrorTypes.IsMatch, ObjectType.View, $"Testing body of SP L: {leftView.Name} R: {rightView.Name}", bodyTest.Results);
            }
            else
            {
                AddTestResult("ERROR", ErrorTypes.NotMatch, ObjectType.View, $"Testing body of SP L: {leftView.Name} R: {rightView.Name}", bodyTest.Results);
            }
            return bodyTest;

        }

        public TestNodes TestTables()
        {
            var tablesTestsNode = CreateTestNode(null, ObjectType.TablesTests, "Set of tests for tables");
            
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
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {rightDatabaseTable.TableName}");
                AddTestResult("ERROR", ErrorTypes.LmissingRpresent, ObjectType.Table, $"Testing table L: missing R: {rightDatabaseTable.TableName}", testNode.Results);
                tablesTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {rightDatabaseTable.TableName}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Testing table L: {leftTable.TableName} R: {rightDatabaseTable.TableName}", testNode.Results);
                //Test Columns
                var testColumnsNode = TestColumns(leftTable, rightDatabaseTable);
                testNode.Nodes.Add(testColumnsNode);
                
                //Append testNode
               
                tablesTestsNode.Nodes.Add(testNode);
            }
        }

        private void LeftDatabaseTests(TestNodes tablesTestsNode, List<Table> rightDatabaseTables, Table leftDatabaseTable)
        {
            var leftTableName = leftDatabaseTable.TableName.ToLower();
            var rightTable = rightDatabaseTables.FirstOrDefault(r => r.TableName.ToLower() == leftTableName);

            if (rightTable == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {leftDatabaseTable.TableName}");
                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Table, $"Testing table L: {leftDatabaseTable.TableName} R: missing", testNode.Results);
                tablesTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {leftDatabaseTable.TableName}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Testing table L: {leftDatabaseTable.TableName} R: {rightTable.TableName}", testNode.Results);

                //Test Columns
                var testColumnsNode = TestColumns(leftDatabaseTable, rightTable);
                testNode.Nodes.Add(testColumnsNode);
           
                //Append testNode
                tablesTestsNode.Nodes.Add(testNode);
            }
        }


        private TestNodes TestColumns(Table leftTable, Table rightTable)
        {
            Logger.Info($"Start testing columns for tables L:{leftTable.TableName} R:{rightTable.TableName}");
            var testColumnsNode = CreateTestNode(new List<TestResult>(), ObjectType.ColumnsTests, "Set of tests for Columns"); 

            foreach (var leftTableColumn in leftTable.Columns)
            {
                var columnNode = CreateTestNode(new List<TestResult>(), ObjectType.Column,
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
                var columnNode = CreateTestNode(new List<TestResult>(), ObjectType.Column,
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
            Logger.Info("Begin TestProcedures method.");
            var proceduresTestsNode = CreateTestNode(null, ObjectType.StoredProceduresTests, "Set of tests for stored procedures");

            var leftDatabaseSp = LeftDatabase.GetStoredProceduresInfo().ToList();
            var rightDatabaseSp = RightDatabase.GetStoredProceduresInfo().ToList();

            foreach (var leftSp in leftDatabaseSp)
            {
                TestLeftDbProcedures(proceduresTestsNode, rightDatabaseSp, leftSp);
            }

            foreach (var rightSp in rightDatabaseSp)
            {
                TestRightDbProcedures(proceduresTestsNode, leftDatabaseSp, rightSp);
            }

            Logger.Info($"End TestProcedures method.");

            return proceduresTestsNode;
        }

        private void TestRightDbProcedures(TestNodes proceduresTestsNode, IEnumerable<StoredProcedure> leftDatabaseSp, StoredProcedure rightSp)
        {
            var leftSp = leftDatabaseSp.FirstOrDefault(r => r.Name.ToLower() == rightSp.Name.ToLower());

            if (leftSp == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP: {rightSp.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.StoredProcedure, $"Testing Stored Procedure L: missing R: {rightSp.Name}", testNode.Results);

                proceduresTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP {rightSp.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {leftSp.Name} R: {rightSp.Name}", testNode.Results);
                var testProceduresBodyNode = TestProceduresBody(leftSp, rightSp);
                testNode.Nodes.Add(testProceduresBodyNode);
                proceduresTestsNode.Nodes.Add(testNode);
            }
        }

        private void TestLeftDbProcedures(TestNodes proceduresTestsNode, IEnumerable<StoredProcedure> rightDatabaseSp, StoredProcedure leftSp)
        {
            var rightSp = rightDatabaseSp.FirstOrDefault(r => r.Name.ToLower() == leftSp.Name.ToLower());

            if (rightSp == null)
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP: {leftSp.Name}");

                AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {leftSp.Name} R: missing", testNode.Results);

                proceduresTestsNode.Nodes.Add(testNode);
            }
            else
            {
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP: {leftSp.Name}");
                AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.StoredProcedure, $"Testing Stored Procedure L: {leftSp.Name} R: {rightSp.Name}", testNode.Results);
                var testProceduresBodyNode = TestProceduresBody(leftSp, rightSp);
                testNode.Nodes.Add(testProceduresBodyNode);
                proceduresTestsNode.Nodes.Add(testNode);
            }
        }

        private TestNodes TestProceduresBody(StoredProcedure leftSp, StoredProcedure rightSp)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, "Test for Stored Procedure Body");
            var normalizedLeft = Extensions.Normalize(leftSp.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightSp.Body).Trim();
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

        public TestNodes TestIndexes()
        {
            Logger.Info("Begin TestIndexes method.");
            var indexesTestsNode = CreateTestNode(null, ObjectType.IndexTests, "Set of tests for indexes");
            var leftDatabaseIndexes = LeftDatabase.GetIndexesInfo().ToList();
            var rightDatabaseIndexes = RightDatabase.GetIndexesInfo().ToList();

            foreach (var leftDatabaseIndex in leftDatabaseIndexes)
            {
                var indexNode = CreateTestNode(new List<TestResult>(), ObjectType.Index,
                    $"Index Name {leftDatabaseIndex.IndexName} applied on {leftDatabaseIndex.TableName}.{leftDatabaseIndex.ColumnName}");
                var rigthIndex = rightDatabaseIndexes.FirstOrDefault(r => 
                string.Equals(r.IndexName, leftDatabaseIndex.IndexName, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(r.TableName, leftDatabaseIndex.TableName, StringComparison.CurrentCultureIgnoreCase));
                if (rigthIndex == null)
                {
                    AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Index, $"Indexes L: {leftDatabaseIndex.IndexName} R: misssing", indexNode.Results);
                }
                else
                {
                    AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Index, $"Indexes L: {leftDatabaseIndex.IndexName} R: {rigthIndex.IndexName}", indexNode.Results);
                }
                indexesTestsNode.Nodes.Add(indexNode);
            }

            foreach (var rightDatabaseIndex in rightDatabaseIndexes)
            {
                var indexNode = CreateTestNode(new List<TestResult>(), ObjectType.Index,
                   $"Index Name {rightDatabaseIndex.IndexName} applied on {rightDatabaseIndex.TableName}.{rightDatabaseIndex.ColumnName}");
                var leftIndex = leftDatabaseIndexes.FirstOrDefault(l => 
                string.Equals(l.IndexName, rightDatabaseIndex.IndexName, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(l.TableName, rightDatabaseIndex.TableName, StringComparison.CurrentCultureIgnoreCase));
                if (leftIndex == null)
                {
                    AddTestResult("ERROR", ErrorTypes.LpresentRmissing, ObjectType.Index, $"Indexes L: missing R: {rightDatabaseIndex.IndexName}", indexNode.Results);
                }
                else
                {
                    AddTestResult("SUCCESS", ErrorTypes.LpresentRpresent, ObjectType.Index, $"Indexes L: {leftIndex.IndexName} R: {rightDatabaseIndex.IndexName}", indexNode.Results);
                }
                indexesTestsNode.Nodes.Add(indexNode);
            }
            
            Logger.Info("End TestTables method.");

            return indexesTestsNode;
        }

        public TestNodes TestIntegrityConstraints()
        {
            Logger.Info("Begin TestIntegrityConstraints method");
            var integrityConstraintsTestNode = CreateTestNode(null, ObjectType.IntegrityConstraintsTests, "Set of tests for integrity constraints");




            Logger.Info("End TestIntegrityConstraints method");
            return integrityConstraintsTestNode;
        }

        private static TestNodes CreateTestNode(List<TestResult> testResults, ObjectType testType, string description)
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
            var matchingvalues = testResults.Exists(test =>
            test.ErrorType == errorType &&
            test.TestedObjectName == testedObjName &&
            test.ObjectType == objType && 
            test.Description == description);

            if (matchingvalues) return;

            Logger.Info($"Adding TestResult for {objType} of name {testedObjName} with description: {description}");

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
