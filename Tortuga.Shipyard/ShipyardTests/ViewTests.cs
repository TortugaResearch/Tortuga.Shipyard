using System.Data;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ViewTests
{
	[TestMethod]
	public void View_Constructor_Test()
	{
		var view = new View("Reporting", "TestView");
		Assert.AreEqual("Reporting", view.SchemaName);
		Assert.AreEqual("TestView", view.ViewName);
	}

	[TestMethod]
	public void View_Description_Test()
	{
		var view = new View("Reporting", "TestView")
		{
			Description = "This is a test view"
		};

		Assert.AreEqual("This is a test view", view.Description);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void View_Join_EmptyJoinColumn_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));

		var departmentTable = new Table("Storage", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, departmentTable, "");
	}

	[TestMethod]
	public void View_Join_ExcludeDuplicateColumns_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("FirstName", DbType.String, 50));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var departmentTable = new Table("Storage", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));// Duplicate column name
		departmentTable.Columns.Add(new Column("FirstName", DbType.String, 100)); // Duplicate column name
		departmentTable.Columns.Add(new Column("DepartmentName", DbType.String, 100));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");

		Assert.AreEqual(2, view.Sources.Count);
		var joinedSource = view.Sources[1] as JoinedViewSource;
		Assert.IsNotNull(joinedSource);

		// Should only include DepartmentId from the join, FirstName should be excluded
		Assert.AreEqual(1, joinedSource.Outputs.Count);
	}

	[TestMethod]
	public void View_Join_InnerJoin_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("FirstName", DbType.String, 50));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var departmentTable = new Table("Storage", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		departmentTable.Columns.Add(new Column("DepartmentName", DbType.String, 100));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");

		Assert.AreEqual(2, view.Sources.Count);
		var joinedSource = view.Sources[1] as JoinedViewSource;
		Assert.IsNotNull(joinedSource);
		Assert.AreEqual(JoinType.InnerJoin, joinedSource.JoinType);
		Assert.AreEqual("Storage", joinedSource.SchemaName);
		Assert.AreEqual("Department", joinedSource.TableOrViewName);
	}

	[TestMethod]
	public void View_Join_LeftJoin_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("FirstName", DbType.String, 50));
		employeeTable.Columns.Add(new Column("ManagerId", DbType.Int32, true));

		var managerTable = new Table("Storage", "Manager");
		managerTable.Columns.Add(new Column("ManagerId", DbType.Int32));
		managerTable.Columns.Add(new Column("ManagerName", DbType.String, 100));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.LeftJoin, managerTable, "ManagerId");

		Assert.AreEqual(2, view.Sources.Count);
		var joinedSource = view.Sources[1] as JoinedViewSource;
		Assert.IsNotNull(joinedSource);
		Assert.AreEqual(JoinType.LeftJoin, joinedSource.JoinType);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void View_Join_NullTable_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, null!, "SomeColumn");
	}

	[TestMethod]
	public void View_Join_WithPrefixColumnAlias_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("FirstName", DbType.String, 50));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var departmentTable = new Table("Storage", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		departmentTable.Columns.Add(new Column("DepartmentName", DbType.String, 100));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId",
			new JoinRules { PrefixColumnAlias = "Dept_" });

		Assert.AreEqual(2, view.Sources.Count);
		var joinedSource = view.Sources[1] as JoinedViewSource;
		Assert.IsNotNull(joinedSource);

		// When prefix is used, all columns should be included with prefix
		Assert.IsTrue(joinedSource.Outputs.Count >= 1);
	}

	[TestMethod]
	public void View_MultipleJoins_Test()
	{
		var employeeTable = new Table("Storage", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		employeeTable.Columns.Add(new Column("ManagerId", DbType.Int32));

		var departmentTable = new Table("Storage", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		departmentTable.Columns.Add(new Column("DepartmentName", DbType.String, 100));

		var managerTable = new Table("Storage", "Manager");
		managerTable.Columns.Add(new Column("ManagerId", DbType.Int32));
		managerTable.Columns.Add(new Column("ManagerName", DbType.String, 100));

		var view = employeeTable.CreateView("Reporting", "EmployeeView");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");
		view.Join(JoinType.LeftJoin, managerTable, "ManagerId");

		Assert.AreEqual(3, view.Sources.Count);
	}

	[TestMethod]
	public void View_Properties_SetAndGet_Test()
	{
		var view = new View("Reporting", "TestView");
		view.SchemaName = "NewSchema";
		view.ViewName = "NewView";
		view.Description = "Updated description";

		Assert.AreEqual("NewSchema", view.SchemaName);
		Assert.AreEqual("NewView", view.ViewName);
		Assert.AreEqual("Updated description", view.Description);
	}

	[TestMethod]
	public void View_Sources_Test()
	{
		var view = new View("Reporting", "TestView");
		var source = new ViewSource("Storage", "Employee");
		view.Sources.Add(source);

		Assert.AreEqual(1, view.Sources.Count);
		Assert.AreEqual("Storage", view.Sources[0].SchemaName);
		Assert.AreEqual("Employee", view.Sources[0].TableOrViewName);
	}
}
