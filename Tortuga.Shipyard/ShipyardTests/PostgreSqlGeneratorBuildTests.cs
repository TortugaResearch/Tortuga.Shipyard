using NpgsqlTypes;
using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed class PostgreSqlGeneratorBuildTests : TestsBase
{
	[TestMethod]
	public void BuildTable_WithClusteredIndex_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

CREATE CLUSTERED INDEX idx_employee ON hr.employee (employee_key);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.ClusteredIndex = new Index
		{
			IndexName = "idx_employee",
			OrderedColumns = { "EmployeeKey" }
		};

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_WithIndexAsConstraint_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	email varchar(100) NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

ALTER TABLE hr.employee ADD CONSTRAINT uq_email UNIQUE (email);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("Email", NpgsqlDbType.Varchar, 100));
		table.Indexes.Add(new Index
		{
			IndexName = "uq_email",
			IsConstraint = true,
			OrderedColumns = { "Email" }
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_WithUniqueIndex_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	email varchar(100) NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

CREATE UNIQUE INDEX idx_email ON hr.employee (email);
";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("Email", NpgsqlDbType.Varchar, 100));
		table.Indexes.Add(new Index
		{
			IndexName = "idx_email",
			IsUnique = true,
			OrderedColumns = { "Email" }
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_WithNonClusteredIndex_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	last_name varchar(50) NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

CREATE NONCLUSTERED INDEX idx_lastname ON hr.employee (last_name);
";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));
		table.Indexes.Add(new Index
		{
			IndexName = "idx_lastname",
			IsUnique = false,
			OrderedColumns = { "LastName" }
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_WithIndexIncludedColumns_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	last_name varchar(50) NOT NULL,
	first_name varchar(50) NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

CREATE NONCLUSTERED INDEX idx_lastname ON hr.employee (last_name) INCLUDE (first_name);
";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Indexes.Add(new Index
		{
			IndexName = "idx_lastname",
			OrderedColumns = { "LastName" },
			IncludedColumns = { "FirstName" }
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_WithColumnDescription_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	first_name varchar(50) NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

COMMENT ON COLUMN hr.employee.first_name IS 'Employee first name';

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50) { Description = "Employee first name" });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_NoPrimaryKey_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	first_name varchar(50) NOT NULL,
	last_name varchar(50) NOT NULL
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("FirstName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_NullableColumn_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	middle_name varchar(50) NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key)
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("MiddleName", NpgsqlDbType.Varchar, 50, true));

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_IdentityWithoutPrimaryKey_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_ForeignKeyWithSameSchema_Test()
	{
		var expected = @"CREATE TABLE hr.employee
(
	employee_key integer GENERATED ALWAYS AS IDENTITY,
	manager_id integer NOT NULL,
	CONSTRAINT employee_pkey PRIMARY KEY (employee_key),
	CONSTRAINT employee_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES hr.manager(manager_id)
);

";

		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("ManagerId", NpgsqlDbType.Integer)
		{
			ReferencedTable = "Manager",
			ReferencedColumn = "ManagerId"
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void BuildTable_MultipleIndexes_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new Column("LastName", NpgsqlDbType.Varchar, 50));
		table.Columns.Add(new Column("Email", NpgsqlDbType.Varchar, 100));
		table.Indexes.Add(new Index
		{
			IndexName = "idx_lastname",
			OrderedColumns = { "LastName" }
		});
		table.Indexes.Add(new Index
		{
			IndexName = "idx_email",
			IsUnique = true,
			OrderedColumns = { "Email" }
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsTrue(output.Contains("CREATE NONCLUSTERED INDEX idx_lastname"));
		Assert.IsTrue(output.Contains("CREATE UNIQUE INDEX idx_email"));
	}

	[TestMethod]
	public void BuildTable_WithTabSize_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsIdentity = true, IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.TabSize = 4;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsFalse(output.Contains("\t"));
		Assert.IsTrue(output.Contains("    ")); // 4 spaces
	}
}
