using NpgsqlTypes;
using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed partial class PostgreSqlTableTests : TestsBase
{
	[TestMethod]
	public void CheckConstraint_Test()
	{
		var expected = @"CREATE TABLE ""dbo"".""TestTable""
(
	""Id"" integer NOT NULL,
	""Age"" integer NOT NULL CONSTRAINT CK_Age CHECK ""Age"" >= 0 AND ""Age"" <= 120,
	CONSTRAINT TestTable_pkey PRIMARY KEY (""Id"")
);

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new("Age", NpgsqlDbType.Integer)
		{
			Check = "\"Age\" >= 0 AND \"Age\" <= 120",
			CheckConstraintName = "CK_Age"
		});

		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void CommentWithApostrophe_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

COMMENT ON TABLE hr.employee IS 'This is John''s table';

";

		var table = new Table("HR", "Employee");
		table.Description = "This is John's table";
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void CompoundPrimaryKey_Test()
	{
		var expected = @"CREATE TABLE dbo.test_table
(
	key1 integer NOT NULL,
	key2 integer NOT NULL,
	""data"" varchar(50) NULL,
	CONSTRAINT test_table_pkey PRIMARY KEY (key1, key2)
);

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Key1", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new("Key2", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new("Data", NpgsqlDbType.Varchar, 50, true));

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void DefaultConstraint_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	status varchar(10) NOT NULL DEFAULT 'Active',
	created_count integer NOT NULL DEFAULT 0,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new("Status", NpgsqlDbType.Varchar, 10) { Default = "'Active'" });
		table.Columns.Add(new("CreatedCount", NpgsqlDbType.Integer) { Default = "0" });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void ForeignKey_WithDifferentSchema_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	department_id integer NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key),
	CONSTRAINT employee_department_id_fkey FOREIGN KEY (department_id) REFERENCES organization.department(department_id)
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new("DepartmentId", NpgsqlDbType.Integer)
		{
			ReferencedSchema = "Organization",
			ReferencedTable = "Department",
			ReferencedColumn = "DepartmentId"
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildTable_Null_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.BuildTable(null!);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildView_Null_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.BuildView(null!);
	}

	[TestMethod]
	public void Generator_EscapeAllIdentifiers_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsTrue(output.Contains("\"HR\".\"Employee\""));
		Assert.IsTrue(output.Contains("\"EmployeeKey\""));
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Keyword_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		var result = generator.EscapeIdentifier("select");

		Assert.AreEqual("\"select\"", result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Normal_NoEscape_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("normalcolumn");

		Assert.AreEqual("normalcolumn", result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Null_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.EscapeIdentifier(null);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_WithSpecialChars_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = true;
		var result = generator.EscapeIdentifier("Column Name");

		Assert.AreEqual("\"Column Name\"", result);
	}

	[TestMethod]
	public void Generator_EscapeText_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.EscapeText("Test value");

		Assert.AreEqual("'Test value'", result);
	}

	[TestMethod]
	public void Generator_EscapeText_WithQuotes_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.EscapeText("It's a test");

		Assert.AreEqual("'It''s a test'", result);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_NameConstraints_Null_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.NameConstraints((Table)null!);
	}

	[TestMethod]
	public void Generator_TabSize_Integration_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.TabSize = 2;
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsFalse(output.Contains("\t"));
		Assert.IsTrue(output.Contains("  ")); // 2 spaces
	}

	[TestMethod]
	public void Generator_UseSnakeCase_ColumnName_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new("FirstName", NpgsqlDbType.Varchar, 50));

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsTrue(output.Contains("employee_key"));
		Assert.IsTrue(output.Contains("first_name"));
	}

	[TestMethod]
	public void Generator_UseSnakeCase_TableName_Test()
	{
		var table = new Table("HR", "EmployeeDetails");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsTrue(output.Contains("hr.employee_details"));
	}

	[TestMethod]
	public void IdentitySeed_CustomValue_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

-- Setting the identity seed
SELECT setval(pg_get_serial_sequence('hr.employee', 'employee_key'), (SELECT GREATEST(1000, MAX(employee_key)) FROM hr.employee));

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer)
		{
			IsIdentity = true,
			IsPrimaryKey = true,
			IdentitySeed = 1000
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void No_Columns_Test()
	{
		var table = new Table("dbo", "foo");
		var generator = new PostgreSqlGenerator();
		var results = generator.Validate(table);
		Assert.IsTrue(results.Any(e => e.MemberNames.Any(c => c == "Columns")));
	}

	private static Table CreateEmployeeTable()
	{
		var table = new Table("HR", "Employee");
		table.Description = "This is my table's description";
		table.Columns.Add(new("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true, IdentitySeed = 101 });
		table.Columns.Add(new("FirstName", NpgsqlDbType.Varchar, 50) { Description = "Hello Name" });
		table.Columns.Add(new("MiddleName", NpgsqlDbType.Varchar, 50, true));
		table.Columns.Add(new("LastName", NpgsqlDbType.Varchar, 50) { Description = "You're in touble name." });
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
