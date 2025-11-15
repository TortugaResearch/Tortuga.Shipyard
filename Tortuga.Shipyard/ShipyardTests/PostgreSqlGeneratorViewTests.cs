using NpgsqlTypes;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class PostgreSqlGeneratorViewTests : TestsBase
{
	[TestMethod]
	public void BuildView_MultipleJoins_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_details
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id,
	e.manager_id,
	d.department_name,
	m.manager_name
FROM hr.employee e
INNER JOIN hr.department d
	ON e.department_id = d.department_id
LEFT JOIN hr.manager m
	ON e.manager_id = m.manager_id;

";

		var employeeTable = CreateEmployeeWithDeptAndManager();
		var departmentTable = CreateSimpleDepartmentTable();
		var managerTable = CreateSimpleManagerTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDetails");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");
		view.Join(JoinType.LeftJoin, managerTable, "ManagerId");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void BuildView_Null_ThrowsException_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.BuildView(null!);
	}

	[TestMethod]
	public void BuildView_SimpleView_NoJoins_Test()
	{
		var expected = @"CREATE VIEW reporting.employees
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id
FROM hr.employee e;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var view = employeeTable.CreateView("Reporting", "Employees");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	[ExpectedException(typeof(NotSupportedException))]
	public void BuildView_UnsupportedJoinType_ThrowsException_Test()
	{
		var employeeTable = CreateSimpleEmployeeTable();
		var view = employeeTable.CreateView("Reporting", "Employees");

		// Manually create a joined source with an invalid join type
		var joinedSource = new JoinedViewSource("HR", "Department", (JoinType)999, new List<string>(), new List<string>())
		{
			Alias = "d",
			JoinExpression = "e.department_id = d.department_id"
		};
		joinedSource.AddColumn("DepartmentName");
		view.Sources.Add(joinedSource);

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.BuildView(view);
	}

	[TestMethod]
	public void BuildView_WithColumnAlias_Test()
	{
		var expected = @"CREATE VIEW reporting.employees
AS
SELECT
	e.employee_key AS emp_id,
	e.first_name AS f_name
FROM hr.employee e;

";

		var employeeTable = new Table("HR", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		employeeTable.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));

		var view = new View("Reporting", "Employees");
		var source = new ViewSource("HR", "Employee") { Alias = "e" };
		source.AddColumn("EmployeeKey", "EmpId");
		source.AddColumn("FirstName", "FName");
		view.Sources.Add(source);

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithCrossJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id,
	d.department_name
FROM hr.employee e
CROSS JOIN hr.department d;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var departmentTable = CreateSimpleDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Sources.Add(new JoinedViewSource("HR", "Department", JoinType.CrossJoin, new List<string>(), new List<string>()) { Alias = "d" });
		view.Sources[1].AddColumn("DepartmentName");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithEscapeAllIdentifiers_Test()
	{
		var expected = @"CREATE VIEW ""Reporting"".""Employees""
AS
SELECT
	e.""EmployeeKey"",
	e.""FirstName"",
	e.""DepartmentId""
FROM ""HR"".""Employee"" e;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var view = employeeTable.CreateView("Reporting", "Employees");

		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		generator.CalculateAliases(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithExpressionColumn_Test()
	{
		var expected = @"CREATE VIEW reporting.employees
AS
SELECT
	e.employee_key,
	e.first_name,
	e.last_name,
	e.first_name || ' ' || e.last_name AS full_name
FROM hr.employee e;

";

		var employeeTable = CreateSimpleEmployeeTableWithLastName();
		var view = employeeTable.CreateView("Reporting", "Employees");
		view.Sources[0].AddExpression("e.first_name || ' ' || e.last_name", "FullName");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithFullJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id,
	d.department_name
FROM hr.employee e
FULL OUTER JOIN hr.department d
	ON e.department_id = d.department_id;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var departmentTable = CreateSimpleDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Join(JoinType.FullJoin, departmentTable, "DepartmentId");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithInnerJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id,
	d.department_name
FROM hr.employee e
INNER JOIN hr.department d
	ON e.department_id = d.department_id;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var departmentTable = CreateSimpleDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithLeftJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_managers
AS
SELECT
	e.employee_key,
	e.first_name,
	e.manager_id,
	m.manager_name
FROM hr.employee e
LEFT JOIN hr.manager m
	ON e.manager_id = m.manager_id;

";

		var employeeTable = CreateEmployeeWithManagerTable();
		var managerTable = CreateSimpleManagerTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeManagers");
		view.Join(JoinType.LeftJoin, managerTable, "ManagerId");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildView_WithRightJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.department_id,
	d.department_name
FROM hr.employee e
RIGHT JOIN hr.department d
	ON e.department_id = d.department_id;

";

		var employeeTable = CreateSimpleEmployeeTable();
		var departmentTable = CreateSimpleDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Join(JoinType.RightJoin, departmentTable, "DepartmentId");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	private static Table CreateEmployeeWithDeptAndManager()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer));
		table.Columns.Add(new Column("ManagerId", NpgsqlDbType.Integer, true));
		return table;
	}

	private static Table CreateEmployeeWithManagerTable()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("ManagerId", NpgsqlDbType.Integer, true));
		return table;
	}

	private static Table CreateSimpleDepartmentTable()
	{
		var table = new Table("HR", "Department");
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("DepartmentName", NpgsqlDbType.Varchar, 100));
		return table;
	}

	private static Table CreateSimpleEmployeeTable()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer));
		return table;
	}

	private static Table CreateSimpleEmployeeTableWithLastName()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));
		return table;
	}

	private static Table CreateSimpleManagerTable()
	{
		var table = new Table("HR", "Manager");
		table.Columns.Add(new Column("ManagerId", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("ManagerName", NpgsqlDbType.Varchar, 100));
		return table;
	}
}
