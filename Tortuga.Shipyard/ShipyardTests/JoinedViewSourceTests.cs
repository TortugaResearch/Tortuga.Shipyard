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
}
