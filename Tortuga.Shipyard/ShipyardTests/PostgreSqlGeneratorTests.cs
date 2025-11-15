using NpgsqlTypes;
using System.Data;
using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed class PostgreSqlGeneratorTests : TestsBase
{
	[TestMethod]
	public void EscapeIdentifier_EmptyString_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.EscapeIdentifier("");

		Assert.AreEqual("", result);
	}

	[TestMethod]
	public void EscapeIdentifier_Keyword_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("select");

		Assert.AreEqual("\"select\"", result);
	}

	[TestMethod]
	public void EscapeIdentifier_StartsWithNumber_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("1stColumn");

		Assert.AreEqual("\"1stColumn\"", result);
	}

	[TestMethod]
	public void EscapeIdentifier_WithDot_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("schema.table");

		Assert.AreEqual("\"schema.table\"", result);
	}

	[TestMethod]
	public void EscapeIdentifier_WithHyphen_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("column-name");

		Assert.AreEqual("\"column-name\"", result);
	}

	[TestMethod]
	public void EscapeIdentifier_WithSnakeCase_AndEscapeAll_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.EscapeAllIdentifiers = true;
		var result = generator.EscapeIdentifier("EmployeeKey");

		Assert.AreEqual("\"employee_key\"", result);
	}

	[TestMethod]
	public void EscapeIdentifier_WithSnakeCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("EmployeeKey");

		Assert.AreEqual("employee_key", result);
	}

	[TestMethod]
	public void EscapeIdentifier_WithSpace_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.EscapeAllIdentifiers = false;
		var result = generator.EscapeIdentifier("column name");

		Assert.AreEqual("\"column name\"", result);
	}

	[TestMethod]
	public void IncludeSchemaNameInConstraintNames_DefaultFalse_Test()
	{
		var generator = new PostgreSqlGenerator();
		Assert.IsFalse(generator.IncludeSchemaNameInConstraintNames);
	}

	[TestMethod]
	public void IncludeSchemaNameInConstraintNames_SetTrue_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.IncludeSchemaNameInConstraintNames = true;
		Assert.IsTrue(generator.IncludeSchemaNameInConstraintNames);
	}

	[TestMethod]
	public void NameConstraints_CheckConstraint_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Age", NpgsqlDbType.Integer) { Default = "0" });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		Assert.AreEqual("employee_age_check", table.Columns[1].CheckConstraintName);
	}

	[TestMethod]
	public void NameConstraints_ClusteredIndex_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.ClusteredIndex = new Index();

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		Assert.AreEqual("employee_ckey", table.ClusteredIndex.IndexName);
	}

	[TestMethod]
	public void NameConstraints_ExistingConstraintNames_NotOverwritten_Test()
	{
		var table = new Table("HR", "Employee");
		table.PrimaryKeyConstraintName = "CustomPK";
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		Assert.AreEqual("CustomPK", table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	public void NameConstraints_ForeignKeyConstraint_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("DepartmentId", NpgsqlDbType.Integer)
		{
			ReferencedSchema = "HR",
			ReferencedTable = "Department",
			ReferencedColumn = "DepartmentId"
		});

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		Assert.AreEqual("employee_department_id_fkey", table.Columns[1].FKConstraintName);
	}

	[TestMethod]
	public void NameConstraints_LongName_LimitTo64_Test()
	{
		var longTableName = new string('A', 100);
		var table = new Table("HR", longTableName);
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		// Should be null because it exceeds 64 characters
		Assert.IsNull(table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	public void NameConstraints_UniqueConstraint_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Email", NpgsqlDbType.Varchar, 100) { IsUnique = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.NameConstraints(table);

		Assert.AreEqual("employee_email_key", table.Columns[1].UniqueConstraintName);
	}

	[TestMethod]
	public void NameConstraints_WithoutSchemaName_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.IncludeSchemaNameInConstraintNames = false;
		generator.NameConstraints(table);

		Assert.AreEqual("employee_pkey", table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	public void NameConstraints_WithSchemaName_Test()
	{
		var table = new Table("HR", "Employee");
		table.Columns.Add(new Column("EmployeeKey", NpgsqlDbType.Integer) { IsPrimaryKey = true });

		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		generator.IncludeSchemaNameInConstraintNames = true;
		generator.NameConstraints(table);

		Assert.AreEqual("hr_employee_pkey", table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	public void PostgreSqlGenerator_Constructor_Test()
	{
		var generator = new PostgreSqlGenerator();

		Assert.IsNotNull(generator);
		Assert.IsNotNull(generator.Keywords);
		Assert.IsTrue(generator.Keywords.Count > 0);
		Assert.IsTrue(generator.Keywords.Contains("SELECT"));
		Assert.IsTrue(generator.Keywords.Contains("FROM"));
	}

	[TestMethod]
	public void SnakeCaseIdentifier_AllLowerCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("employee");

		Assert.AreEqual("employee", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_AllUpperCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("ID");

		Assert.AreEqual("id", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_DisabledSnakeCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = false;
		var result = generator.SnakeCaseIdentifier("EmployeeKey");

		Assert.AreEqual("EmployeeKey", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_EmptyString_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.SnakeCaseIdentifier("");

		Assert.AreEqual("", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_MultipleWords_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("EmployeeFirstName");

		Assert.AreEqual("employee_first_name", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_Null_Test()
	{
		var generator = new PostgreSqlGenerator();
		var result = generator.SnakeCaseIdentifier(null);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_SingleCharacter_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("A");

		Assert.AreEqual("a", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_SingleWord_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("Employee");

		Assert.AreEqual("employee", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_StartsWithLowerCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("employeeKey");

		Assert.AreEqual("employee_key", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_TwoWords_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("EmployeeKey");

		Assert.AreEqual("employee_key", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_WithAcronym_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("HTTPRequest");

		Assert.AreEqual("http_request", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_WithMixedCase_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("EmployeeIDValue");

		Assert.AreEqual("employee_id_value", result);
	}

	[TestMethod]
	public void SnakeCaseIdentifier_WithNumbers_Test()
	{
		var generator = new PostgreSqlGenerator();
		generator.UseSnakeCase = true;
		var result = generator.SnakeCaseIdentifier("Employee123");

		Assert.AreEqual("employee123", result);
	}
}
