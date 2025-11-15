using NpgsqlTypes;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class PostgreSqlViewTests : TestsBase
{
	[TestMethod]
	public void PostgreSql_View_LeftJoin_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_managers
AS
SELECT
	e.employee_key,
	e.first_name,
	e.last_name,
	e.department_id,
	e.manager_key,
	m.manager_name
FROM hr.employee e
LEFT JOIN hr.manager m
	ON e.manager_key = m.manager_key;

";

		var employeeTable = CreateEmployeeTable();
		var managerTable = CreateManagerTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeManagers");
		view.Join(JoinType.LeftJoin, managerTable, "ManagerKey");

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void PostgreSql_View_Scenario_1()
	{
		var expected = @"CREATE VIEW ""Reporting"".""EmployeeDepartments""
AS
SELECT
	e.""EmployeeKey"",
	e.""FirstName"",
	e.""LastName"",
	e.""DepartmentId"",
	e.""ManagerKey"",
	d.""DepartmentName""
FROM ""HR"".""Employee"" e
INNER JOIN ""HR"".""Department"" d
	ON e.""DepartmentId"" = d.""DepartmentId"";

";

		var employeeTable = CreateEmployeeTable();
		var departmentTable = CreateDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId");

		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void PostgreSql_View_Scenario_2_SnakeCase()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.last_name,
	e.department_id,
	e.manager_key,
	d.department_name
FROM hr.employee e
INNER JOIN hr.department d
	ON e.department_id = d.department_id;

";

		var employeeTable = CreateEmployeeTable();
		var departmentTable = CreateDepartmentTable();

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
	public void PostgreSql_View_WithColumnPrefix_Test()
	{
		var expected = @"CREATE VIEW reporting.employee_departments
AS
SELECT
	e.employee_key,
	e.first_name,
	e.last_name,
	e.department_id,
	e.manager_key,
	d.department_id AS dept_department_id,
	d.department_name AS dept_department_name
FROM hr.employee e
INNER JOIN hr.department d
	ON e.department_id = d.department_id;

";

		var employeeTable = CreateEmployeeTable();
		var departmentTable = CreateDepartmentTable();

		var view = employeeTable.CreateView("Reporting", "EmployeeDepartments");
		view.Join(JoinType.InnerJoin, departmentTable, "DepartmentId",
			new JoinRules { PrefixColumnAlias = "Dept_" });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	private static Table CreateDepartmentTable()
	{
		var table = new Table("HR", "Department");
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("DepartmentName", NpgsqlDbType.Varchar, 100));
		return table;
	}

	private static Table CreateEmployeeTable()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer));
		table.Columns.Add(new Column("ManagerKey", NpgsqlDbType.Integer, true));
		return table;
	}

	private static Table CreateManagerTable()
	{
		var table = new Table("HR", "Manager");
		table.Columns.Add(new Column("ManagerKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("ManagerName", NpgsqlDbType.Varchar, 100));
		return table;
	}
}
