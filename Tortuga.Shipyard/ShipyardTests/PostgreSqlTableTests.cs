using NpgsqlTypes;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed partial class PostgreSqlTableTests : TestsBase
{
	[TestMethod]
	public void No_Columns_Test()
	{
		var table = new Table("dbo", "foo");
		var generator = new PostgreSqlGenerator();
		var results = generator.ValidateTable(table);
		Assert.IsTrue(results.Any(e => e.MemberNames.Any(c => c == "Columns")));
	}

	[TestMethod]
	public void Scenario_1()
	{
		var expected = @"CREATE TABLE ""HR"".""Employee""
(
	""EmployeeKey"" serial,
	""FirstName"" varchar(50) NOT NULL,
	""MiddleName"" varchar(50) NULL,
	""LastName"" varchar(50) NOT NULL,
	""Title"" varchar(100) NULL,
	""ManagerKey"" integer NULL,
	""OfficePhone"" varchar(15) NULL,
	""CellPhone"" varchar(15) NULL,
	""CreatedDate"" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	""UpdatedDate"" timestamp NULL,
	""EmployeeId"" varchar(50) NOT NULL,
	""Gender"" char(1) NOT NULL,
	""Status"" char(1) NULL,
	CONSTRAINT Employee_pkey PRIMARY KEY (""EmployeeKey""),
	CONSTRAINT Employee_ManagerKey_fkey FOREIGN KEY (""ManagerKey"") REFERENCES ""HR"".""Employee""(""EmployeeKey"")
);

";
		var table = CreateEmployeeTable();

		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_2()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key serial,
	first_name varchar(50) NOT NULL,
	middle_name varchar(50) NULL,
	last_name varchar(50) NOT NULL,
	title varchar(100) NULL,
	manager_key integer NULL,
	office_phone varchar(15) NULL,
	cell_phone varchar(15) NULL,
	created_date timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_date timestamp NULL,
	employee_id varchar(50) NOT NULL,
	gender char(1) NOT NULL,
	status char(1) NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key),
	CONSTRAINT employee_manager_key_fkey FOREIGN KEY (manager_key) REFERENCES hr.employee(employee_key)
);

";
		var table = CreateEmployeeTable();

		var generator = new PostgreSqlGenerator();
		generator.SnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	private static Table CreateEmployeeTable()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new("MiddleName", NpgsqlDbType.Varchar, 50, true));
		table.Columns.Add(new("LastName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new("Title", NpgsqlDbType.Varchar, 100, true));
		table.Columns.Add(new("ManagerKey", NpgsqlDbType.Integer, true) { ReferencedSchema = "HR", ReferencedTable = "Employee", ReferencedColumn = "EmployeeKey" });
		table.Columns.Add(new("OfficePhone", NpgsqlDbType.Varchar, 15, true));
		table.Columns.Add(new("CellPhone", NpgsqlDbType.Varchar, 15, true));
		table.Columns.Add(new("CreatedDate", NpgsqlDbType.Timestamp) { Default = "CURRENT_TIMESTAMP" });
		table.Columns.Add(new("UpdatedDate", NpgsqlDbType.Timestamp, true));
		table.Columns.Add(new("EmployeeId", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new("Gender", NpgsqlDbType.Char, 1));
		table.Columns.Add(new("Status", NpgsqlDbType.Char, 1, true));
		return table;
	}
}
