using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.Models.SQLServer;
using DBSchemaComparator.Domain.Models.Test;
using NLog;
using Extensions = DBSchemaComparator.Domain.Infrastructure.Extensions;

namespace DBSchemaComparator.App.Comparator
{
    public class ObjectComparator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string ConnStringLeft { get; set; }
        private string ConnStringRight { get; set; }

        private DatabaseType _dbType;
        public DatabaseType DbType
        {
            get { return this._dbType; }
            private set { _dbType = value; }
        }

        public IDatabaseHandler LeftDatabase { get; set; }
        public IDatabaseHandler RightDatabase { get; set; }

        private ObjectComparator(string connStringLeft, string connStringRight)
        {
            ConnStringLeft = connStringLeft;
            ConnStringRight = connStringRight;
        }

        public ObjectComparator(string connStringLeft, string connStringRight, DatabaseType dbType) : this(connStringLeft, connStringRight)
        {
            DbType = dbType;
        }

        public TestNodes CompareDatabases()
        {
           return GetResultTree(ConnStringLeft, ConnStringRight);
        }

        private TestNodes GetResultTree(string connStringLeft, string connStringRight)
        {  
            LeftDatabase = new DatabaseHandler(ConnStringLeft, DbType);
            RightDatabase = new DatabaseHandler(ConnStringRight, DbType);
            
            var mainTestNode = CreateTestNode(new List<TestResult>(), ObjectType.Root, $"Root node.\nLeft DB: {LeftDatabase.DatabaseName}\nRight DB: {RightDatabase.DatabaseName}");

            //Test Database Collation
            TestCollation(mainTestNode.Results);
            
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
            SetResultLevel(mainTestNode);
            return mainTestNode;
          
        }

        private void TestCollation(List<TestResult> results)
        {
            Logger.Info("Begin TestCollation method.");

            var leftDbcollation = LeftDatabase.GetCollationInfo().First();
            var rightDbcollation = RightDatabase.GetCollationInfo().First();

            AddTestResult(
                $"Test for Database Collation L: {leftDbcollation.CollationName} R: {rightDbcollation.CollationName}",
                leftDbcollation.CollationName == rightDbcollation.CollationName
                    ? ErrorTypes.IsMatch
                    : ErrorTypes.NotMatch,
                ObjectType.DatabaseCollation,
                leftDbcollation.CollationName,
                results);

            Logger.Info($"End TestCollation method.");    
        }

        #region TestViews Methods
        private TestNodes TestViews()
        {
            Logger.Info("Begin TestViews");
            var viewsTestNode = CreateTestNode(null, ObjectType.ViewsTests, "Set of tests for views");

            var leftDbViews = LeftDatabase.GetViewsInfo().ToList();
            var rightDbViews = RightDatabase.GetViewsInfo().ToList();

            //Functions only in the left database
            var uniqueLeft = leftDbViews.Where(p => rightDbViews.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueLeft.ForEach(l => AddRightMissingViewNode(viewsTestNode, l));

            //Functions only in the right database
            var uniqueRight = rightDbViews.Where(p => leftDbViews.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueRight.ForEach(r => AddLeftMissingViewNode(viewsTestNode, r));

            var unionListLeftViews = leftDbViews.Where(x => rightDbViews.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            var unionListRightViews = rightDbViews.Where(x => leftDbViews.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();

            if (unionListRightViews.Any())
                TestMatchedViews(unionListLeftViews, unionListRightViews, viewsTestNode);

            SetResultLevel(viewsTestNode);

            Logger.Info("End TestViews method.");

            return viewsTestNode;
        }

        private void AddLeftMissingViewNode(TestNodes viewsTestNode, View view)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View: {view.Name}");

            AddTestResult($"Testing View L: missing R: {view.Name}", ErrorTypes.LmissingRpresent, ObjectType.View, view.Name, testNode.Results);

            SetResultLevel(testNode);
            viewsTestNode.Nodes.Add(testNode);
        }

        private void AddRightMissingViewNode(TestNodes viewsTestNode, View view)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View: {view.Name}");

            AddTestResult($"Testing View L: {view.Name} R: missing", ErrorTypes.LpresentRmissing, ObjectType.View,view.Name , testNode.Results);
         
            SetResultLevel(testNode);

            viewsTestNode.Nodes.Add(testNode);

        }

        private void TestMatchedViews(List<View> leftViewsList, List<View> rightViewsList, TestNodes viewsTestNode)
        {
            foreach (var leftView in leftViewsList)
            {
                var rightView = rightViewsList.First(x => string.Equals(x.Name, leftView.Name, StringComparison.CurrentCultureIgnoreCase));
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.View, $"Test for View {rightView.Name}");

                AddTestResult($"Testing View L: {leftView.Name} R: {rightView.Name}", ErrorTypes.LpresentRpresent, ObjectType.View, leftView.Name , testNode.Results);

                var testProceduresBodyNode = TestViewBody(leftView, rightView);
                testNode.Nodes.Add(testProceduresBodyNode);

                SetResultLevel(testNode);

                viewsTestNode.Nodes.Add(testNode);
            }
        }
     
        private TestNodes TestViewBody(View leftView, View rightView)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.View, "Test for View content");
            var normalizedLeft = Extensions.Normalize(leftView.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightView.Body).Trim();
            Logger.Info($"Testing content of views L: {leftView.Body} R: {rightView.Body}");

            AddTestResult(
                $"Testing content of view L: {leftView.Name}\n{leftView.Body} \nR: {rightView.Name}\n{rightView.Body}",
                normalizedLeft == normalizedRight ? ErrorTypes.IsMatch : ErrorTypes.NotMatch, ObjectType.View,
                leftView.Name, bodyTest.Results);
            return bodyTest;

        }
        #endregion

        #region TestFunctions Methods
        private TestNodes TestFunctions()
        {
            Logger.Info("Begin TestFunctions method.");

            var functionsTestNode = CreateTestNode(null, ObjectType.FunctionsTests, "Set of tests for functions" );

            var leftDbFunctions = LeftDatabase.GetFunctionsInfo().ToList();
            var rightDbFunctions = RightDatabase.GetFunctionsInfo().ToList();

            //Functions only in the left database
            var uniqueLeft = leftDbFunctions.Where(p => rightDbFunctions.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueLeft.ForEach(l => AddRightMissingFunctionNode(functionsTestNode, l));

            //Functions only in the right database
            var uniqueRight = rightDbFunctions.Where(p => leftDbFunctions.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueRight.ForEach(r => AddLeftMissingFunctionNode(functionsTestNode, r));

            var unionListLeftFunctions = leftDbFunctions.Where(x => rightDbFunctions.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            var unionListRightFunctions = rightDbFunctions.Where(x => leftDbFunctions.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();

            if (unionListRightFunctions.Any())
                TestMatchedFunctions(unionListLeftFunctions, unionListRightFunctions, functionsTestNode);

            SetResultLevel(functionsTestNode);

            Logger.Info("End TestFunctions method.");

            return functionsTestNode;
        }

        private void TestMatchedFunctions(List<Function> leftFuncionsList, List<Function> rightFunctionsList,
            TestNodes functionsTestNode)
        {
            foreach (var leftFn in leftFuncionsList)
            {
                var rightFn = rightFunctionsList.First(x => string.Equals(x.Name, leftFn.Name, StringComparison.CurrentCultureIgnoreCase));

                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function {rightFn.Name}");

                AddTestResult($"Testing Function L: {leftFn.Name} R: {rightFn.Name}", ErrorTypes.LpresentRpresent, ObjectType.Function, $"Reference object {leftFn.Name}", testNode.Results);

                var testFunctionsBodyNode = TestFunctionBody(leftFn, rightFn);

                testNode.Nodes.Add(testFunctionsBodyNode);

                SetResultLevel(testNode);

                functionsTestNode.Nodes.Add(testNode);
            }
        }

        private void AddLeftMissingFunctionNode(TestNodes functionsTestNode, Function function)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function: {function.Name}");

            AddTestResult($"Testing Function L: missing R: {function.Name}", ErrorTypes.LmissingRpresent, ObjectType.Function, function.Name, testNode.Results);

            SetResultLevel(testNode);

            functionsTestNode.Nodes.Add(testNode);
        }

        private void AddRightMissingFunctionNode(TestNodes functionsTestNode, Function function)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Function, $"Test for Function: {function.Name}");

            AddTestResult($"Testing Functions L: {function.Name} R: missing", ErrorTypes.LpresentRmissing, ObjectType.Function, function.Name , testNode.Results);

            SetResultLevel(testNode);

            functionsTestNode.Nodes.Add(testNode);
        }

        private TestNodes TestFunctionBody(Function leftFunction, Function rightFunction)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.Function, "Test for Function Content");

            var normalizedLeft = Extensions.Normalize(leftFunction.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightFunction.Body).Trim();

            Logger.Info($"Testing functions content L: {leftFunction.Name} R: {rightFunction.Name}");

            Logger.Debug($"Testing functions content L: {leftFunction.Body} R: {rightFunction.Body}");

            AddTestResult($"Testing content of functions L: {leftFunction.Name}\n{leftFunction.Body} \nR: {rightFunction.Name}\n{rightFunction.Body}",
                normalizedLeft == normalizedRight ? ErrorTypes.IsMatch : ErrorTypes.NotMatch, ObjectType.Function,
                leftFunction.Name, bodyTest.Results);

            SetResultLevel(bodyTest);

            return bodyTest;

        }
        #endregion

        #region TestTables Methods

        public TestNodes TestTables()
        {
            var tablesTestsNode = CreateTestNode(null, ObjectType.TablesTests, "Set of tests for tables");
            
            Logger.Info("Begin TestTables method.");

            var leftDatabaseTables = LeftDatabase.GetTablesSchemaInfo().ToList();
            var rightDatabaseTables = RightDatabase.GetTablesSchemaInfo().ToList();

            //Tables only in the left database
            var uniqueLeft = leftDatabaseTables.Where(p => rightDatabaseTables.All(p2 => !string.Equals(p2.TableName,p.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueLeft.ForEach(l => AddRightMissingTableNode(tablesTestsNode, l));

            //Tables only in the right database
            var uniqueRight = rightDatabaseTables.Where(p => leftDatabaseTables.All(p2 => !string.Equals(p2.TableName, p.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueRight.ForEach(r => AddLeftMissingTableNode(tablesTestsNode, r));

            var unionListLeftTables = leftDatabaseTables.Where(x => rightDatabaseTables.Any(y => string.Equals(y.TableName, x.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            var unionListRightTables = rightDatabaseTables.Where(x => leftDatabaseTables.Any(y => string.Equals(y.TableName, x.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();

            if (unionListRightTables.Any())
            TestMatchedTables(unionListLeftTables, unionListRightTables, tablesTestsNode);

            SetResultLevel(tablesTestsNode);

            Logger.Info($"End TestTables method.");
            return tablesTestsNode;
        }

        private void TestMatchedTables(List<Table> leftTableList, List<Table> rightTableList, TestNodes tablesTestsNode)
        {
            foreach (var leftTbl in leftTableList)
            {
                var rightTable = rightTableList.First(x => string.Equals(x.TableName, leftTbl.TableName, StringComparison.CurrentCultureIgnoreCase));
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {leftTbl.TableName}");

                AddTestResult($"Testing table L: {leftTbl.TableName} R: {rightTable.TableName}", ErrorTypes.LpresentRpresent, ObjectType.Table, $"Reference table {leftTbl.TableName}" , testNode.Results);
                
                //Test Columns
                var testColumnsNode = TestColumns(leftTbl, rightTable);
                testNode.Nodes.Add(testColumnsNode);

                SetResultLevel(testNode);
                //Append testNode
                tablesTestsNode.Nodes.Add(testNode);
            }
        }

        private void AddRightMissingTableNode(TestNodes tablesTestsNode, Table leftDatabaseTable)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {leftDatabaseTable.TableName}");
            AddTestResult($"Testing table L: {leftDatabaseTable.TableName} R: missing", ErrorTypes.LpresentRmissing, ObjectType.Table, leftDatabaseTable.TableName, testNode.Results);

            SetResultLevel(testNode);
            tablesTestsNode.Nodes.Add(testNode);
        }

        private void AddLeftMissingTableNode(TestNodes tablesTestsNode, Table rightDatabaseTable)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.Table, $"Test for Table {rightDatabaseTable.TableName}");
            AddTestResult($"Testing table L: missing R: {rightDatabaseTable.TableName}", ErrorTypes.LmissingRpresent, ObjectType.Table, rightDatabaseTable.TableName , testNode.Results);

            SetResultLevel(testNode);
            tablesTestsNode.Nodes.Add(testNode);
        }

        private TestNodes TestColumns(Table leftTable, Table rightTable)
        {
            Logger.Info($"Start testing columns for tables L:{leftTable.TableName} R:{rightTable.TableName}");
            var testColumnsNode = CreateTestNode(null, ObjectType.ColumnsTests, "Set of tests for Columns"); 

            leftTable.Columns.ForEach(leftTableColumn => TestLeftDbColumns(rightTable, testColumnsNode, leftTableColumn));
            rightTable.Columns.ForEach(rightTableColumn => TestRightDbColumns(leftTable, testColumnsNode, rightTableColumn));
            
            Logger.Info($"Returning TestNodes for testing columns.");
            SetResultLevel(testColumnsNode);
            return testColumnsNode;
        }

        private void TestRightDbColumns(Table leftTable, TestNodes testColumnsNode, Column rightTableColumn)
        {
            var columnNode = CreateTestNode(new List<TestResult>(), ObjectType.Column,
              $"Column {rightTableColumn.TableName}.{rightTableColumn.ColumnName}");

            var leftTableColumn = leftTable.Columns.FirstOrDefault(l => l.ColumnName == rightTableColumn.ColumnName);
            if (leftTableColumn == null)
            {
                AddTestResult($"Columns L: missing R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", ErrorTypes.LmissingRpresent, ObjectType.Column, rightTableColumn.TableName, columnNode.Results);
            }
            else
            {
                AddTestResult($"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", ErrorTypes.LpresentRpresent, ObjectType.Column, leftTableColumn.ColumnName, columnNode.Results);
                //Check Columns
                CheckColumnType(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check column IsNullable
                CheckColumnIsNullable(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check Identity settings
                CheckColumnIdentity(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check column Collation
                CheckColumnCollation(leftTableColumn, rightTableColumn, columnNode.Results);
            }

            SetResultLevel(columnNode);

            testColumnsNode.Nodes.Add(columnNode);
        }

        private void TestLeftDbColumns(Table rightTable, TestNodes testColumnsNode, Column leftTableColumn)
        {
            var columnNode = CreateTestNode(new List<TestResult>(), ObjectType.Column,
                $"Column {leftTableColumn.TableName}.{leftTableColumn.ColumnName}");
            var rightTableColumn = rightTable.Columns.FirstOrDefault(r => r.ColumnName == leftTableColumn.ColumnName);
            if (rightTableColumn == null)
            {
                AddTestResult($"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: misssing", ErrorTypes.LpresentRmissing, ObjectType.Column, leftTableColumn.ColumnName, columnNode.Results);
            }
            else
            {
                AddTestResult($"Columns L: {leftTableColumn.TableName}.{leftTableColumn.ColumnName} R: {rightTableColumn.TableName}.{rightTableColumn.ColumnName}", ErrorTypes.LpresentRpresent, ObjectType.Column, leftTableColumn.ColumnName , columnNode.Results);
                //Check column DataTypes
                CheckColumnType(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check column IsNullable
                CheckColumnIsNullable(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check Identity settings
                CheckColumnIdentity(leftTableColumn, rightTableColumn, columnNode.Results);
                //Check column Collation
                CheckColumnCollation(leftTableColumn, rightTableColumn, columnNode.Results);
            }
            SetResultLevel(columnNode);

            testColumnsNode.Nodes.Add(columnNode);
        }

        private void CheckColumnIdentity(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            AddTestResult(
                $"Identification setting for tested columns L: {leftTableColumn.ColumnName} = {leftTableColumn.IsIdentification} R: {rightTableColumn.ColumnName} = {rightTableColumn.IsIdentification}",
                leftTableColumn.IsIdentification == rightTableColumn.IsIdentification
                    ? ErrorTypes.IsMatch
                    : ErrorTypes.NotMatch,
                ObjectType.IsIdentification,
                $"L: {leftTableColumn.IsIdentification} R: {rightTableColumn.IsIdentification}",
                testResults);
        }

        private void CheckColumnIsNullable(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            AddTestResult(
                $"IsNullable setting for tested columns L: {leftTableColumn.ColumnName} = {leftTableColumn.IsNullable} R: {rightTableColumn.ColumnName} = {rightTableColumn.IsNullable}",
                leftTableColumn.IsNullable == rightTableColumn.IsNullable ? ErrorTypes.IsMatch : ErrorTypes.NotMatch,
                ObjectType.IsNullable,
                $"L: {leftTableColumn.IsNullable} R: {rightTableColumn.IsNullable}",
                testResults);
        }

        private void CheckColumnType(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            AddTestResult(
                $"DataTypes of tested columns L: {leftTableColumn.ColumnName} = {leftTableColumn.DataType} R: {rightTableColumn.ColumnName} = {rightTableColumn.DataType}",
                leftTableColumn.DataType == rightTableColumn.DataType ? ErrorTypes.IsMatch : ErrorTypes.NotMatch,
                ObjectType.DataType,
                $"L: {leftTableColumn.DataType} R: {rightTableColumn.DataType}"
                , testResults);
        }

        private void CheckColumnCollation(Column leftTableColumn, Column rightTableColumn, List<TestResult> testResults)
        {
            if (leftTableColumn.CollationName == null && rightTableColumn.CollationName == null)
            {
                AddTestResult("Collation for tested columns cannot be overwritten.",ErrorTypes.IsMatch, ObjectType.ColumnCollation, $"L: Type = {leftTableColumn.DataType} R: Type = {rightTableColumn.DataType}", testResults);
            }
            else
            {
               AddTestResult(
               $"Collation for tested columns L: {leftTableColumn.ColumnName} = {leftTableColumn.CollationName} R: {rightTableColumn.ColumnName} = {rightTableColumn.CollationName}",
               leftTableColumn.CollationName == rightTableColumn.CollationName ? ErrorTypes.IsMatch : ErrorTypes.NotMatch,
               ObjectType.ColumnCollation,
               $"L: {leftTableColumn.CollationName} R: {rightTableColumn.CollationName}",
               testResults);
            }
        }

        #endregion

        #region TestProcedures Methods
        public TestNodes TestProcedures()
        {
            Logger.Info("Begin TestProcedures method.");

            var proceduresTestsNode = CreateTestNode(null, ObjectType.StoredProceduresTests, "Set of tests for stored procedures");

            var leftDatabaseSp = LeftDatabase.GetStoredProceduresInfo().ToList();
            var rightDatabaseSp = RightDatabase.GetStoredProceduresInfo().ToList();

            //Procedures only in the left database
            var uniqueLeft = leftDatabaseSp.Where(p => rightDatabaseSp.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueLeft.ForEach(l => AddRightMissingProcedureNode(proceduresTestsNode, l));

            //Procedures only in the right database
            var uniqueRight = rightDatabaseSp.Where(p => leftDatabaseSp.All(p2 => !string.Equals(p2.Name, p.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueRight.ForEach(r => AddLeftMissingProcedureNode(proceduresTestsNode, r));

            var unionListLeftProcedures = leftDatabaseSp.Where(x => rightDatabaseSp.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();
            var unionListRightProcedures = rightDatabaseSp.Where(x => leftDatabaseSp.Any(y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))).ToList();

            if (unionListRightProcedures.Any())
                TestMatchedProcedures(unionListLeftProcedures, unionListRightProcedures, proceduresTestsNode);

            Logger.Info($"End TestProcedures method.");

            SetResultLevel(proceduresTestsNode);

            return proceduresTestsNode;
        }

        private void TestMatchedProcedures(List<StoredProcedure> leftProceduresList, List<StoredProcedure> rightProceduresList, TestNodes proceduresTestsNode)
        {
            foreach (var leftSp in leftProceduresList)
            {
                var rightSp = rightProceduresList.First(x => string.Equals(x.Name, leftSp.Name, StringComparison.CurrentCultureIgnoreCase));
                var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP {leftSp.Name}");
                AddTestResult($"Testing Stored Procedure L: {leftSp.Name} R: {rightSp.Name}", ErrorTypes.LpresentRpresent, ObjectType.StoredProcedure, $"Reference object {leftSp.Name}", testNode.Results);
                var testProceduresBodyNode = TestProceduresBody(leftSp, rightSp);
                testNode.Nodes.Add(testProceduresBodyNode);
                SetResultLevel(testNode);
                proceduresTestsNode.Nodes.Add(testNode);
            }
        }

        private void AddLeftMissingProcedureNode(TestNodes proceduresTestsNode, StoredProcedure storedProcedure)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP: {storedProcedure.Name}");
            AddTestResult($"Testing Stored Procedure L: missing R: {storedProcedure.Name}", ErrorTypes.LmissingRpresent, ObjectType.StoredProcedure, storedProcedure.Name, testNode.Results);
            SetResultLevel(testNode);
            proceduresTestsNode.Nodes.Add(testNode);
        }

        private void AddRightMissingProcedureNode(TestNodes proceduresTestsNode, StoredProcedure storedProcedure)
        {
            var testNode = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, $"Test for SP: {storedProcedure.Name}");
            AddTestResult($"Testing Stored Procedure L: {storedProcedure.Name} R: missing", ErrorTypes.LpresentRmissing, ObjectType.StoredProcedure, storedProcedure.Name, testNode.Results);
            SetResultLevel(testNode);
            proceduresTestsNode.Nodes.Add(testNode);
        }

        private TestNodes TestProceduresBody(StoredProcedure leftSp, StoredProcedure rightSp)
        {
            var bodyTest = CreateTestNode(new List<TestResult>(), ObjectType.StoredProcedure, "Test for SP Content");
            var normalizedLeft = Extensions.Normalize(leftSp.Body).Trim();
            var normalizedRight = Extensions.Normalize(rightSp.Body).Trim();
            Logger.Info($"Testing stored procedures content L: {leftSp.Body} R: {rightSp.Body}");
            AddTestResult($"Testing content of SP L: {leftSp.Name}\n{leftSp.Body} \nR: {rightSp.Name}\n{rightSp.Body}",
                normalizedLeft == normalizedRight ? ErrorTypes.IsMatch : ErrorTypes.NotMatch, ObjectType.StoredProcedure,
                leftSp.Name, bodyTest.Results);
            SetResultLevel(bodyTest);
            return bodyTest;
        }

        #endregion

        #region TestIndexes Methods
        public TestNodes TestIndexes()
        {
            Logger.Info("Begin TestIndexes method.");
            var indexesTestsNode = CreateTestNode(null, ObjectType.IndexTests, "Set of tests for indexes");

            var leftDatabaseIndexes = LeftDatabase.GetIndexesInfo().ToList();
            var rightDatabaseIndexes = RightDatabase.GetIndexesInfo().ToList();

            //Tables only in the left database
            var uniqueLeft = leftDatabaseIndexes.Where(p => rightDatabaseIndexes.All(p2 => !string.Equals(p2.IndexName, p.IndexName, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(p2.TableName, p.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueLeft.ForEach(l => AddRightMissingIndexNode(indexesTestsNode, l));

            //Tables only in the right database
            var uniqueRight = rightDatabaseIndexes.Where(p => leftDatabaseIndexes.All(p2 => !string.Equals(p2.IndexName, p.IndexName, StringComparison.CurrentCultureIgnoreCase) && !string.Equals(p2.TableName,p.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            uniqueRight.ForEach(r => AddLeftMissingIndexNode(indexesTestsNode, r));

            var unionListLeftIndexes = leftDatabaseIndexes.Where(x => rightDatabaseIndexes.Any(y => string.Equals(y.IndexName, x.IndexName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(y.TableName,x.TableName,StringComparison.CurrentCultureIgnoreCase))).ToList();
            var unionListRightIndexes = rightDatabaseIndexes.Where(x => leftDatabaseIndexes.Any(y => string.Equals(y.IndexName, x.IndexName, StringComparison.CurrentCultureIgnoreCase) && string.Equals(y.TableName, x.TableName, StringComparison.CurrentCultureIgnoreCase))).ToList();

            if (unionListRightIndexes.Any())
                TestMatchedIndexes(unionListLeftIndexes, unionListRightIndexes, indexesTestsNode);

            SetResultLevel(indexesTestsNode);

            Logger.Info("End TestTables method.");

            return indexesTestsNode;
        }

        private void TestMatchedIndexes(List<Index> leftIndexesList, List<Index> rightIndexesList, TestNodes indexesTestsNode)
        {
            foreach (var leftIndex in leftIndexesList)
            {
                Logger.Info($"Testing index {leftIndex.TableName}");    

                var rigthIndex = rightIndexesList.First(r => 
                string.Equals(r.IndexName, leftIndex.IndexName, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(r.TableName, leftIndex.TableName, StringComparison.CurrentCultureIgnoreCase));

                var indexNode = CreateTestNode(new List<TestResult>(), ObjectType.Index,
                   $"Index Name {leftIndex.IndexName} applied on {leftIndex.TableName}.{leftIndex.ColumnName}");

                AddTestResult($"Indexes L: {leftIndex.IndexName} R: {rigthIndex.IndexName}", ErrorTypes.LpresentRpresent, ObjectType.Index, leftIndex.IndexName, indexNode.Results);

                SetResultLevel(indexNode);

                indexesTestsNode.Nodes.Add(indexNode);
            }
        }

        private void AddLeftMissingIndexNode(TestNodes indexesTestsNode, Index index)
        {
            var indexNode = CreateTestNode(new List<TestResult>(), ObjectType.Index, $"Index Name {index.IndexName} applied on {index.TableName}.{index.ColumnName}");

            AddTestResult($"Indexes L: missing R: {index.IndexName}", ErrorTypes.LpresentRmissing, ObjectType.Index, index.IndexName, indexNode.Results);

            SetResultLevel(indexNode);

            indexesTestsNode.Nodes.Add(indexNode);
        }

        private void AddRightMissingIndexNode(TestNodes indexesTestsNode, Index index)
        {
            var indexNode = CreateTestNode(new List<TestResult>(), ObjectType.Index, $"Index Name {index.IndexName} applied on {index.TableName}.{index.ColumnName}");
           
            AddTestResult($"Indexes L: {index.IndexName} R: misssing", ErrorTypes.LpresentRmissing, ObjectType.Index, index.IndexName, indexNode.Results);

            SetResultLevel(indexNode);

            indexesTestsNode.Nodes.Add(indexNode);
        }

        #endregion

        #region TestIntegrityConstraints Methods
        private TestNodes TestIntegrityConstraints()
        {
            Logger.Info("Begin TestIntegrityConstraints method");
            var integrityConstraintsTestNode = CreateTestNode(null, ObjectType.IntegrityConstraintsTests, "Set of tests for integrity constraints");

            var primaryKeys = CreateTestNode(null, ObjectType.PrimaryKeysTests, "Set of tests for primary keys.");
            var foreignKeys = CreateTestNode(null, ObjectType.ForeignKeysTests, "Set of tests for foreign keys.");
            var checkConstraints = CreateTestNode(null, ObjectType.CheckTests, "Set of tests for check constraints");
            
            // Primary Keys
            var leftDbPk = LeftDatabase.GetPrimaryKeysInfo().ToList();
            var rightDbPk = RightDatabase.GetPrimaryKeysInfo().ToList();

            leftDbPk.ForEach(leftPkKey => TestLeftDbPrimaryKey(primaryKeys,rightDbPk,leftPkKey));
            rightDbPk.ForEach(rightPkKey => TestRightDbPrimaryKey(primaryKeys, leftDbPk, rightPkKey));
           
            // Foreign Keys
            var leftDbFk = LeftDatabase.GetForeignKeysInfo().ToList();
            var rightDbFk = RightDatabase.GetForeignKeysInfo().ToList();

            leftDbFk.ForEach(leftFk => TestLeftDbForeignKeys(foreignKeys, rightDbFk, leftFk));
            rightDbFk.ForEach(rightFk => TestRigthDbForeignKeys(foreignKeys, rightDbFk, rightFk));

            // Check Constraints
            if (DbType == DatabaseType.SqlServer)
            {
                var leftDbChk = LeftDatabase.GetCheckConstraintsInfo().ToList();
                var rightDbChk = RightDatabase.GetCheckConstraintsInfo().ToList();
                leftDbChk.ForEach(leftCheck => TestLeftDbChecks(checkConstraints, rightDbChk, leftCheck));
                rightDbChk.ForEach(rightCheck => TestRightDbCheck(checkConstraints, leftDbChk, rightCheck));
            } else if (DbType == DatabaseType.MySql)
            {
                var notSupportedNode = CreateTestNode(new List<TestResult>(), ObjectType.Check,
                    "MySql checks not supported");
                AddTestResult($"Testing Check Constraint: Not Supported", ErrorTypes.IsMatch, ObjectType.Check, "MySql engines does not support check constraints.", notSupportedNode.Results);
                SetResultLevel(notSupportedNode);
                checkConstraints.Nodes.Add(notSupportedNode);
            }
            SetResultLevel(primaryKeys);
            SetResultLevel(foreignKeys);
            SetResultLevel(checkConstraints);

            integrityConstraintsTestNode.Nodes.Add(primaryKeys);
            integrityConstraintsTestNode.Nodes.Add(foreignKeys);
            integrityConstraintsTestNode.Nodes.Add(checkConstraints);

            SetResultLevel(integrityConstraintsTestNode);

            Logger.Info("End TestIntegrityConstraints method");
            return integrityConstraintsTestNode;
        }

        private void TestRigthDbForeignKeys(TestNodes foreignKeys, List<ForeignKey> rightDbFk, ForeignKey rightFk)
        {
            var leftFk = rightDbFk.FirstOrDefault(l => string.Equals(l.ConstraintName, rightFk.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
                                                        string.Equals(l.ConstraintTable, rightFk.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));
            var testFkNode = CreateTestNode(new List<TestResult>(), ObjectType.ForeignKey, $"Test for Foreign Key Constraint: {rightFk.ConstraintName} within table: {rightFk.ConstraintTable} on column: {rightFk.ColumnApplied}");

            if (leftFk == null)
            {
                AddTestResult($"Testing Foreign Key Constraint L: missing R: {rightFk.ConstraintName}", 
                    ErrorTypes.LmissingRpresent, 
                    ObjectType.ForeignKey, 
                    rightFk.ConstraintName, 
                    testFkNode.Results);
            }
            else
            {
                AddTestResult($"Testing Foreign Key Constraint L: {leftFk.ConstraintName} within Table: {leftFk.ConstraintTable} \nR: {rightFk.ConstraintName} within Table: {rightFk.ConstraintTable}", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.ForeignKey, 
                    rightFk.ConstraintName, 
                    testFkNode.Results);

                ForeignKeysSubtests(rightFk, leftFk, testFkNode);
            }
            SetResultLevel(testFkNode);
            foreignKeys.Nodes.Add(testFkNode);
        }

        private void ForeignKeysSubtests(ForeignKey rightFk, ForeignKey leftFk, TestNodes testFkNode)
        {
            var testColumnApplied = string.Equals(leftFk.ColumnApplied, rightFk.ColumnApplied, StringComparison.CurrentCultureIgnoreCase);

            var testReferencedTable = string.Equals(leftFk.ReferencedTable, rightFk.ReferencedTable, StringComparison.CurrentCultureIgnoreCase);

            var testReferencedColumn = string.Equals(leftFk.ReferencedColumn, rightFk.ReferencedColumn, StringComparison.CurrentCultureIgnoreCase);

            if (testColumnApplied)
            {
                AddTestResult($"Applied Column L: {leftFk.ColumnApplied} \nR: {leftFk.ColumnApplied} matches.",
                    ErrorTypes.IsMatch,
                    ObjectType.ForeignKey,
                    $"{leftFk.ConstraintName}.{leftFk.ColumnApplied}",
                    testFkNode.Results);
            }
            else
            {
                AddTestResult($"Applied Column L: {leftFk.ColumnApplied} R: {rightFk.ColumnApplied} missmatch.",
                    ErrorTypes.NotMatch,
                    ObjectType.ForeignKey,
                     $"{leftFk.ConstraintName}.{leftFk.ColumnApplied}",
                    testFkNode.Results);
            }

            if (testReferencedTable)
            {
                AddTestResult($"Referenced Table L: {leftFk.ReferencedTable} \nR: {rightFk.ReferencedTable} matches.",
                    ErrorTypes.IsMatch,
                    ObjectType.ForeignKey,
                    $"{leftFk.ConstraintName}.{leftFk.ReferencedTable}",
                    testFkNode.Results);
            }
            else
            {
                AddTestResult($"Referenced Table L: {leftFk.ReferencedTable} \nR: {rightFk.ReferencedTable} missmatch.",
                    ErrorTypes.NotMatch,
                    ObjectType.ForeignKey,
                   $"{leftFk.ConstraintName}.{leftFk.ReferencedTable}",
                    testFkNode.Results);
            }

            if (testReferencedColumn)
            {
                AddTestResult($"Referenced Column L: {leftFk.ReferencedColumn} \nR: {rightFk.ReferencedColumn} matches.",
                    ErrorTypes.IsMatch,
                    ObjectType.ForeignKey,
                    $"{leftFk.ConstraintName}.{leftFk.ReferencedColumn}",
                    testFkNode.Results);
            }
            else
            {
                AddTestResult($"Referenced Column L: {leftFk.ReferencedColumn} \n R: {rightFk.ReferencedColumn} missmatch.",
                    ErrorTypes.NotMatch,
                    ObjectType.ForeignKey,
                    $"{leftFk.ConstraintName}{leftFk.ReferencedColumn}",
                    testFkNode.Results);
            }
        }

        private void TestLeftDbForeignKeys(TestNodes foreignKeys, List<ForeignKey> rightDbFk, ForeignKey leftFk)
        {
            var rightFk = rightDbFk.FirstOrDefault(r => string.Equals(r.ConstraintName, leftFk.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
                                                        string.Equals(r.ConstraintTable, leftFk.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));

            var testFkNode = CreateTestNode(new List<TestResult>(), ObjectType.ForeignKey, $"Test for Foreign Key Constraint: {leftFk.ConstraintName} within table: {leftFk.ConstraintTable}.{leftFk.ColumnApplied}");

            if (rightFk == null)
            {
                AddTestResult($"Testing Foreign Key Constraint L: {leftFk.ConstraintName} R: missing", ErrorTypes.LpresentRmissing, ObjectType.ForeignKey, $"{leftFk.ConstraintName}" , testFkNode.Results);
            }
            else
            {
                AddTestResult($"Testing Foreign Key Constraint L: {leftFk.ConstraintName} within Table: {leftFk.ConstraintTable} \nR: {rightFk.ConstraintName} within Table: {rightFk.ConstraintTable}", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.ForeignKey, leftFk.ConstraintName, 
                    testFkNode.Results);
                ForeignKeysSubtests(rightFk, leftFk, testFkNode);
               
            }
            SetResultLevel(testFkNode);
            foreignKeys.Nodes.Add(testFkNode);
        }

        private void TestRightDbCheck(TestNodes checkConstraints, List<CheckConstraint> leftDbChk, CheckConstraint rightCheck)
        {
            var leftCheck = leftDbChk.FirstOrDefault(l => string.Equals(l.ConstraintName, rightCheck.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
           string.Equals(l.ConstraintTable, rightCheck.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));

            var testCheckNode = CreateTestNode(new List<TestResult>(), ObjectType.Check, $"Test for Check Constraint: {rightCheck.ConstraintName} within table: {rightCheck.ConstraintTable}");
            if (leftCheck == null)
            {
                AddTestResult($"Testing Check Constraint L: missing R: {rightCheck.ConstraintName}", ErrorTypes.LmissingRpresent, ObjectType.Check, rightCheck.ConstraintName , testCheckNode.Results);
            }
            else
            {
                AddTestResult($"Testing Check Constraint L: {leftCheck.ConstraintName} within Table: {leftCheck.ConstraintTable} \nR: {leftCheck.ConstraintName} within Table: {leftCheck.ConstraintTable}", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.Check, leftCheck.ConstraintName,
                    testCheckNode.Results);

                var testCheckBody = string.Equals(rightCheck.ConstraintBody, leftCheck.ConstraintBody, StringComparison.CurrentCultureIgnoreCase);
                var testColumnApplied = string.Equals(rightCheck.ColumnApplied, leftCheck.ColumnApplied, StringComparison.CurrentCultureIgnoreCase);

                if (testCheckBody && testColumnApplied)
                {
                    AddTestResult($"Column and Body L: {leftCheck.ConstraintName} \nR: {leftCheck.ConstraintName} matches",
                        ErrorTypes.IsMatch,
                        ObjectType.Check, 
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }

                if (!testCheckBody)
                {
                    AddTestResult($"Testing Check Body L: {leftCheck.ConstraintName} with body {leftCheck.ConstraintBody} \nR: {leftCheck.ConstraintName} with body {leftCheck.ColumnApplied}",
                        ErrorTypes.NotMatch,
                        ObjectType.Check,
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }
                if (!testColumnApplied)
                {
                    AddTestResult($"Testing Applied Column L: {leftCheck.ConstraintName} on column {leftCheck.ColumnApplied} \nR: {leftCheck.ConstraintName} on column {leftCheck.ColumnApplied}",
                        ErrorTypes.NotMatch,
                        ObjectType.Check,
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }
            }
            SetResultLevel(testCheckNode);
            checkConstraints.Nodes.Add(testCheckNode);
        }

        private void TestLeftDbChecks(TestNodes checkConstraints, List<CheckConstraint> rightDbChk, CheckConstraint leftCheck)
        {
            var rightCheck = rightDbChk.FirstOrDefault(r => string.Equals(r.ConstraintName, leftCheck.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
            string.Equals(r.ConstraintTable, leftCheck.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));
            var testCheckNode = CreateTestNode(new List<TestResult>(), ObjectType.Check, $"Test for Check Constraint: {leftCheck.ConstraintName} within table: {leftCheck.ConstraintTable}");
            if (rightCheck == null)
            {
                AddTestResult($"Testing Check Constraint L: {leftCheck.ConstraintName} R: missing", ErrorTypes.LpresentRmissing, ObjectType.Check, leftCheck.ConstraintName, testCheckNode.Results);
            }
            else
            {
                AddTestResult($"Testing Check Constraint L: {leftCheck.ConstraintName} within Table: {leftCheck.ConstraintTable} \nR: {rightCheck.ConstraintName} within Table: {rightCheck.ConstraintTable}", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.Check, 
                    rightCheck.ConstraintName,
                    testCheckNode.Results);

                var testCheckBody = string.Equals(leftCheck.ConstraintBody, rightCheck.ConstraintBody, StringComparison.CurrentCultureIgnoreCase);
                var testColumnApplied = string.Equals(leftCheck.ColumnApplied, rightCheck.ColumnApplied, StringComparison.CurrentCultureIgnoreCase);

                if (testCheckBody && testColumnApplied)
                {
                    AddTestResult($"Column and Body L: {leftCheck.ConstraintName} \nR: {rightCheck.ConstraintName} matches", 
                        ErrorTypes.IsMatch, 
                        ObjectType.Check, 
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }

                if (!testCheckBody)
                {
                    AddTestResult($"Testing Check Body L: {leftCheck.ConstraintName} with body {leftCheck.ConstraintBody} \nR: {rightCheck.ConstraintName} with body {rightCheck.ColumnApplied}",
                        ErrorTypes.NotMatch,
                        ObjectType.Check,
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }
                if (!testColumnApplied)
                {
                    AddTestResult($"Testing Applied Column L: {leftCheck.ConstraintName} on column {leftCheck.ColumnApplied} \nR: {rightCheck.ConstraintName} on column {rightCheck.ColumnApplied}",
                        ErrorTypes.NotMatch,
                        ObjectType.Check,
                        leftCheck.ConstraintName, 
                        testCheckNode.Results);
                }
            }
            SetResultLevel(testCheckNode);
            checkConstraints.Nodes.Add(testCheckNode);
        }

        private void TestRightDbPrimaryKey(TestNodes primaryKeys, IList<PrimaryKey> leftDbPk, PrimaryKey rightPkKey)
        {
            var pkNode = CreateTestNode(new List<TestResult>(), ObjectType.PrimaryKey, $"Primary Key R: {rightPkKey.ConstraintName} applied on {rightPkKey.ConstraintTable}.{rightPkKey.ColumnApplied}");
            var leftPrimaryKey = leftDbPk.FirstOrDefault(l =>
           string.Equals(l.ConstraintName, rightPkKey.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
           string.Equals(l.ConstraintTable, rightPkKey.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));

            if (leftPrimaryKey == null)
            {
                AddTestResult($"Primary Keys L: misssing R: {rightPkKey.ConstraintName}", ErrorTypes.LmissingRpresent, ObjectType.PrimaryKey,
                    rightPkKey.ConstraintName, pkNode.Results);
            }
            else
            {
                AddTestResult($"Primary Keys L: {leftPrimaryKey.ConstraintName} R: {rightPkKey.ConstraintName}.", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.PrimaryKey,
                          leftPrimaryKey.ConstraintName,
                          pkNode.Results);

                var testColumnApplied = string.Equals(leftPrimaryKey.ColumnApplied,
                    rightPkKey.ColumnApplied, StringComparison.CurrentCultureIgnoreCase);

                var testTableApplied = string.Equals(leftPrimaryKey.ConstraintTable,
                    rightPkKey.ConstraintTable, StringComparison.CurrentCultureIgnoreCase);

                if (testColumnApplied && testTableApplied)
                {
                    AddTestResult($"Primary Keys L: {leftPrimaryKey.ConstraintName} Table.Column:{leftPrimaryKey.ConstraintTable}.{leftPrimaryKey.ColumnApplied} " +
                                  $"\nR: {rightPkKey.ConstraintName} Table.Column:{rightPkKey.ConstraintTable}.{rightPkKey.ColumnApplied}", 
                        ErrorTypes.IsMatch, 
                        ObjectType.PrimaryKey, 
                        leftPrimaryKey.ConstraintName,
                        pkNode.Results);
                }
                if (!testColumnApplied)
                {
                    AddTestResult($"Primary Keys Columns Applied on L: {leftPrimaryKey.ConstraintName} on Column: {leftPrimaryKey.ColumnApplied}  \nR: {rightPkKey.ConstraintName} on Column: {rightPkKey.ColumnApplied}", 
                        ErrorTypes.NotMatch, 
                        ObjectType.PrimaryKey,
                        leftPrimaryKey.ConstraintName,
                        pkNode.Results);
                }
                if (!testTableApplied)
                {
                    AddTestResult($"Primary Keys applied on Tables L: {leftPrimaryKey.ConstraintName} on Table: {leftPrimaryKey.ConstraintTable} \nR: {rightPkKey.ConstraintName} on Table: {rightPkKey.ConstraintTable}", 
                        ErrorTypes.NotMatch, 
                        ObjectType.PrimaryKey,
                        leftPrimaryKey.ConstraintName,
                        pkNode.Results);
                }
            }
            SetResultLevel(pkNode);
            primaryKeys.Nodes.Add(pkNode);
        }

        private void TestLeftDbPrimaryKey(TestNodes primaryKeys, IList<PrimaryKey> rightDbPk, PrimaryKey leftPkKey)
        {
            var pkNode = CreateTestNode(new List<TestResult>(), ObjectType.PrimaryKey,
               $"PrimaryKey Name {leftPkKey.ConstraintName} applied on {leftPkKey.ConstraintTable}.{leftPkKey.ColumnApplied}");
            var rightPrimaryKey = rightDbPk.FirstOrDefault(r =>
            string.Equals(r.ConstraintName, leftPkKey.ConstraintName, StringComparison.CurrentCultureIgnoreCase) &&
           string.Equals(r.ConstraintTable, leftPkKey.ConstraintTable, StringComparison.CurrentCultureIgnoreCase));

            if (rightPrimaryKey == null)
            {
                AddTestResult($"Primary Keys L: {leftPkKey.ConstraintName} R: misssing", ErrorTypes.LpresentRmissing, ObjectType.PrimaryKey, leftPkKey.ConstraintName, pkNode.Results);
            }
            else
            {
                AddTestResult($"Primary Keys L: {leftPkKey.ConstraintName} R: {rightPrimaryKey.ConstraintName}", 
                    ErrorTypes.LpresentRpresent, 
                    ObjectType.PrimaryKey,
                       leftPkKey.ConstraintName,
                       pkNode.Results);

                var testColumnApplied = string.Equals(leftPkKey.ColumnApplied,
                    rightPrimaryKey.ColumnApplied, StringComparison.CurrentCultureIgnoreCase);

                var testTableApplied = string.Equals(leftPkKey.ConstraintTable,
                    rightPrimaryKey.ConstraintTable, StringComparison.CurrentCultureIgnoreCase);

                if (testColumnApplied && testTableApplied)
                {
                    AddTestResult($"Primary Keys L: {leftPkKey.ConstraintName} Table.Column:{leftPkKey.ConstraintTable}.{leftPkKey.ColumnApplied} " +
                                  $"\nR: {rightPrimaryKey.ConstraintName} Table.Column:{rightPrimaryKey.ConstraintTable}.{rightPrimaryKey.ColumnApplied}", 
                        ErrorTypes.IsMatch, 
                        ObjectType.PrimaryKey,
                        leftPkKey.ConstraintName,
                        pkNode.Results);
                }

                if (!testColumnApplied)
                {
                    AddTestResult($"Primary Keys Columns Applied on L: {leftPkKey.ConstraintName} on Column: {leftPkKey.ColumnApplied}  \nR: {rightPrimaryKey.ConstraintName} on Column: {rightPrimaryKey.ColumnApplied}", ErrorTypes.NotMatch, ObjectType.PrimaryKey,
                        $"{leftPkKey.ConstraintName}.{leftPkKey.ColumnApplied}",
                        pkNode.Results);
                }

                if (!testTableApplied)
                {
                    AddTestResult($"Primary Keys applied on Tables L: {leftPkKey.ConstraintName} on Table: {leftPkKey.ConstraintTable} \nR: {rightPrimaryKey.ConstraintName} on Table: {rightPrimaryKey.ConstraintTable}", ErrorTypes.NotMatch, ObjectType.PrimaryKey,
                        $"{leftPkKey.ConstraintName}.{leftPkKey.ConstraintTable}",
                        pkNode.Results);
                }
            }
            SetResultLevel(pkNode);
            primaryKeys.Nodes.Add(pkNode);
        }

        #endregion

        public static TestNodes CreateTestNode(List<TestResult> testResults, ObjectType testType, string description)
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

        public static TestNodes CreateTestNode(List<TestResult> testResults, ObjectType testType, string description, ResultLevel resultLevel)
        {
            var node = new TestNodes
            {
                Nodes = new List<TestNodes>(),
                Results = testResults,
                Description = description,
                NodeType = testType,
                ResultLevel = resultLevel
            };
            return node;
        }

        public static void SetResultLevel(TestNodes testNode)
        { 
            ResultLevel resultLevel = ResultLevel.None;

            if(testNode.Nodes.Any()) { 
                 var resultList = Extensions.DepthFirstTraversal(testNode, col => col.Nodes).ToList();
                resultLevel = resultList.Any(r => r.ResultLevel == ResultLevel.Error) ? ResultLevel.Error : ResultLevel.Success;
            }
            if (testNode.Results != null && testNode.Results.Any() && resultLevel != ResultLevel.Error) { 
                var columnNodeResults = testNode.Results.Any(
                                        r =>
                                        r.ErrorType != ErrorTypes.LpresentRpresent &&
                                        r.ErrorType != ErrorTypes.IsMatch &&
                                        r.ErrorType != ErrorTypes.CreationScriptSuccess);

                resultLevel = columnNodeResults ? ResultLevel.Error : ResultLevel.Success;
            }
            Logger.Debug($"Setting result level to {resultLevel}",testNode);
            testNode.ResultLevel = resultLevel;

        }

        public static void AddTestResult(string description, ErrorTypes errorType, ObjectType objType, string testedObjName, List<TestResult> testResults)
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
