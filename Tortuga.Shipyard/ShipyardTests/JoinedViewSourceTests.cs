using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class JoinedViewSourceTests
{
	[TestMethod]
	public void JoinedViewSource_ComplexJoin_Test()
	{
		var leftCols = new List<string> { "EmployeeId", "DepartmentId" };
		var rightCols = new List<string> { "EmployeeId", "DepartmentId" };

		var joinedSource = new JoinedViewSource("HR", "EmployeeDepartment", JoinType.LeftJoin, leftCols, rightCols)
		{
			Alias = "ed",
			JoinExpression = "e.EmployeeId = ed.EmployeeId AND e.DepartmentId = ed.DepartmentId"
		};

		joinedSource.AddColumn("AssignmentDate");
		joinedSource.AddExpression("Status", "CASE WHEN EndDate IS NULL THEN 'Active' ELSE 'Inactive' END");

		Assert.AreEqual(2, joinedSource.LeftJoinColumns.Count);
		Assert.AreEqual(2, joinedSource.RightJoinColumns.Count);
		Assert.AreEqual("ed", joinedSource.Alias);
		Assert.IsNotNull(joinedSource.JoinExpression);
		Assert.AreEqual(2, joinedSource.Outputs.Count);
	}

	[TestMethod]
	public void JoinedViewSource_Constructor_Test()
	{
		var leftCols = new List<string> { "DepartmentId" };
		var rightCols = new List<string> { "DepartmentId" };

		var joinedSource = new JoinedViewSource("HR", "Department", JoinType.InnerJoin, leftCols, rightCols);

		Assert.AreEqual("HR", joinedSource.SchemaName);
		Assert.AreEqual("Department", joinedSource.TableOrViewName);
		Assert.AreEqual(JoinType.InnerJoin, joinedSource.JoinType);
		Assert.AreEqual(1, joinedSource.LeftJoinColumns.Count);
		Assert.AreEqual(1, joinedSource.RightJoinColumns.Count);
		Assert.AreEqual("DepartmentId", joinedSource.LeftJoinColumns[0]);
		Assert.AreEqual("DepartmentId", joinedSource.RightJoinColumns[0]);
		Assert.IsNull(joinedSource.JoinExpression);
	}

	[TestMethod]
	public void JoinedViewSource_FullJoinType_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.FullJoin, leftCols, rightCols);

		Assert.AreEqual(JoinType.FullJoin, joinedSource.JoinType);
	}

	[TestMethod]
	public void JoinedViewSource_InnerJoinType_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);

		Assert.AreEqual(JoinType.InnerJoin, joinedSource.JoinType);
	}

	[TestMethod]
	public void JoinedViewSource_JoinExpression_Test()
	{
		var leftCols = new List<string> { "DepartmentId" };
		var rightCols = new List<string> { "DepartmentId" };

		var joinedSource = new JoinedViewSource("HR", "Department", JoinType.InnerJoin, leftCols, rightCols);
		joinedSource.JoinExpression = "e.DepartmentId = d.DepartmentId";

		Assert.AreEqual("e.DepartmentId = d.DepartmentId", joinedSource.JoinExpression);
	}

	[TestMethod]
	public void JoinedViewSource_LeftJoinType_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.LeftJoin, leftCols, rightCols);

		Assert.AreEqual(JoinType.LeftJoin, joinedSource.JoinType);
	}

	[TestMethod]
	public void JoinedViewSource_MultipleJoinColumns_Test()
	{
		var leftCols = new List<string> { "Column1", "Column2" };
		var rightCols = new List<string> { "Column1", "Column2" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.LeftJoin, leftCols, rightCols);

		Assert.AreEqual(2, joinedSource.LeftJoinColumns.Count);
		Assert.AreEqual(2, joinedSource.RightJoinColumns.Count);
		Assert.AreEqual("Column1", joinedSource.LeftJoinColumns[0]);
		Assert.AreEqual("Column2", joinedSource.LeftJoinColumns[1]);
		Assert.AreEqual("Column1", joinedSource.RightJoinColumns[0]);
		Assert.AreEqual("Column2", joinedSource.RightJoinColumns[1]);
	}

	[TestMethod]
	public void JoinedViewSource_RightJoinType_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.RightJoin, leftCols, rightCols);

		Assert.AreEqual(JoinType.RightJoin, joinedSource.JoinType);
	}

	[TestMethod]
	public void JoinedViewSource_WithAlias_Test()
	{
		var leftCols = new List<string> { "DepartmentId" };
		var rightCols = new List<string> { "DepartmentId" };

		var joinedSource = new JoinedViewSource("HR", "Department", JoinType.InnerJoin, leftCols, rightCols)
		{
			Alias = "dept"
		};

		Assert.AreEqual("dept", joinedSource.Alias);
	}

	[TestMethod]
	public void JoinedViewSource_WithOutputs_Test()
	{
		var leftCols = new List<string> { "DepartmentId" };
		var rightCols = new List<string> { "DepartmentId" };

		var joinedSource = new JoinedViewSource("HR", "Department", JoinType.InnerJoin, leftCols, rightCols);
		joinedSource.AddColumn("DepartmentName");
		joinedSource.AddColumn("ManagerId");

		Assert.AreEqual(2, joinedSource.Outputs.Count);
	}

	[TestMethod]
	public void JoinedViewSource_AddColumn_WithOutputName_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		joinedSource.AddColumn("ColumnName", "AliasName");

		Assert.AreEqual(1, joinedSource.Outputs.Count);
		var viewColumn = joinedSource.Outputs[0] as ViewColumn;
		Assert.IsNotNull(viewColumn);
		Assert.AreEqual("ColumnName", viewColumn.ColumnName);
		Assert.AreEqual("AliasName", viewColumn.OutputColumnName);
	}

	[TestMethod]
	public void JoinedViewSource_MixedOutputTypes_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		joinedSource.AddColumn("Name");
		joinedSource.AddExpression("FullName", "FirstName + ' ' + LastName");
		joinedSource.AddColumn("Email", "EmailAddress");

		Assert.AreEqual(3, joinedSource.Outputs.Count);
		Assert.IsInstanceOfType(joinedSource.Outputs[0], typeof(ViewColumn));
		Assert.IsInstanceOfType(joinedSource.Outputs[1], typeof(ExpressionColumn));
		Assert.IsInstanceOfType(joinedSource.Outputs[2], typeof(ViewColumn));
	}

	[TestMethod]
	public void JoinedViewSource_EmptyJoinColumns_Test()
	{
		var leftCols = new List<string>();
		var rightCols = new List<string>();

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);

		Assert.AreEqual(0, joinedSource.LeftJoinColumns.Count);
		Assert.AreEqual(0, joinedSource.RightJoinColumns.Count);
	}

	[TestMethod]
	public void JoinedViewSource_PropertyChanges_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		
		joinedSource.SchemaName = "HR";
		joinedSource.TableOrViewName = "Employee";
		joinedSource.Alias = "emp";
		joinedSource.JoinExpression = "custom.expression";

		Assert.AreEqual("HR", joinedSource.SchemaName);
		Assert.AreEqual("Employee", joinedSource.TableOrViewName);
		Assert.AreEqual("emp", joinedSource.Alias);
		Assert.AreEqual("custom.expression", joinedSource.JoinExpression);
	}

	[TestMethod]
	public void JoinedViewSource_NullAlias_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols)
		{
			Alias = "test"
		};
		
		joinedSource.Alias = null;

		Assert.IsNull(joinedSource.Alias);
	}

	[TestMethod]
	public void JoinedViewSource_NullJoinExpression_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols)
		{
			JoinExpression = "test.expression"
		};
		
		joinedSource.JoinExpression = null;

		Assert.IsNull(joinedSource.JoinExpression);
	}

	[TestMethod]
	public void JoinedViewSource_ChainingAddColumn_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols)
			.AddColumn("Col1")
			.AddColumn("Col2")
			.AddExpression("Expr1", "Expression1");

		Assert.AreEqual(3, joinedSource.Outputs.Count);
	}

	[TestMethod]
	public void JoinedViewSource_AddColumnReturnsThis_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		var result = joinedSource.AddColumn("TestColumn");

		Assert.AreSame(joinedSource, result);
	}

	[TestMethod]
	public void JoinedViewSource_AddExpressionReturnsThis_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		var result = joinedSource.AddExpression("OutputName", "TestExpression");

		Assert.AreSame(joinedSource, result);
	}

	[TestMethod]
	public void JoinedViewSource_DifferentSchemas_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("CustomSchema", "CustomTable", JoinType.LeftJoin, leftCols, rightCols);

		Assert.AreEqual("CustomSchema", joinedSource.SchemaName);
		Assert.AreEqual("CustomTable", joinedSource.TableOrViewName);
	}

	[TestMethod]
	public void JoinedViewSource_LongTableName_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };
		var longName = new string('A', 128);

		var joinedSource = new JoinedViewSource("dbo", longName, JoinType.InnerJoin, leftCols, rightCols);

		Assert.AreEqual(longName, joinedSource.TableOrViewName);
	}

	[TestMethod]
	public void JoinedViewSource_MultipleExpressions_Test()
	{
		var leftCols = new List<string> { "Id" };
		var rightCols = new List<string> { "Id" };

		var joinedSource = new JoinedViewSource("dbo", "Table1", JoinType.InnerJoin, leftCols, rightCols);
		joinedSource.AddExpression("Expr1", "Expression1");
		joinedSource.AddExpression("Expr2", "Expression2");
		joinedSource.AddExpression("Expr3", "Expression3");

		Assert.AreEqual(3, joinedSource.Outputs.Count);
		Assert.IsInstanceOfType(joinedSource.Outputs[0], typeof(ExpressionColumn));
		Assert.IsInstanceOfType(joinedSource.Outputs[1], typeof(ExpressionColumn));
		Assert.IsInstanceOfType(joinedSource.Outputs[2], typeof(ExpressionColumn));
	}
}
