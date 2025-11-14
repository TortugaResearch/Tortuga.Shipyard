using System.Data;
using NpgsqlTypes;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ColumnEdgeCasesTests
{
	[TestMethod]
	public void Column_SqlServerType_Constructor_Test()
	{
		var column = new Column("Test", SqlDbType.VarChar, false);
		Assert.AreEqual(SqlDbType.VarChar, column.SqlServerType);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	public void Column_SqlServerType_Constructor_Nullable_Test()
	{
		var column = new Column("Test", SqlDbType.NVarChar, true);
		Assert.AreEqual(SqlDbType.NVarChar, column.SqlServerType);
		Assert.IsTrue(column.IsNullable);
	}

	[TestMethod]
	public void Column_IsRowStart_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.DateTime2) { IsRowStart = true };
		Assert.IsTrue(column.IsRowStart);
	}

	[TestMethod]
	public void Column_IsRowEnd_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.DateTime2) { IsRowEnd = true };
		Assert.IsTrue(column.IsRowEnd);
	}

	[TestMethod]
	public void Column_IsHidden_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.Int32) { IsHidden = true };
		Assert.IsTrue(column.IsHidden);
	}

	[TestMethod]
	public void Column_DefaultConstructor_AllPropertiesNull_Test()
	{
		var column = new Column();

		Assert.IsNull(column.ColumnName);
		Assert.IsNull(column.Type);
		Assert.IsNull(column.SqlServerType);
		Assert.IsNull(column.PostgreSqlType);
		Assert.IsNull(column.MaxLength);
		Assert.IsNull(column.Precision);
		Assert.IsNull(column.Scale);
		Assert.IsFalse(column.IsNullable);
		Assert.IsFalse(column.IsPrimaryKey);
		Assert.IsFalse(column.IsIdentity);
		Assert.IsFalse(column.IsUnique);
		Assert.IsFalse(column.IsSparse);
		Assert.IsFalse(column.IsRowStart);
		Assert.IsFalse(column.IsRowEnd);
		Assert.IsFalse(column.IsHidden);
		Assert.IsFalse(column.DefaultLocalTime);
		Assert.IsFalse(column.DefaultUtcTime);
	}

	[TestMethod]
	public void Column_HasDefault_EmptyStringDefault_Test()
	{
		var column = new Column("Test", DbType.String) { Default = "" };
		Assert.IsFalse(column.HasDefault);
	}

	[TestMethod]
	public void Column_HasDefault_WhitespaceDefault_Test()
	{
		var column = new Column("Test", DbType.String) { Default = "   " };
		Assert.IsTrue(column.HasDefault);
	}

	[TestMethod]
	public void Column_HasDefault_MultipleDefaults_Test()
	{
		var column = new Column("Test", DbType.DateTime)
		{
			Default = "GETDATE()",
			DefaultLocalTime = true,
			DefaultUtcTime = true
		};
		Assert.IsTrue(column.HasDefault);
	}

	[TestMethod]
	public void Column_SqlServerTypeOverride_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.String)
		{
			SqlServerTypeOverride = "VARCHAR(MAX) COLLATE Latin1_General_CI_AS"
		};
		Assert.AreEqual("VARCHAR(MAX) COLLATE Latin1_General_CI_AS", column.SqlServerTypeOverride);
		Assert.AreEqual("VARCHAR(MAX) COLLATE Latin1_General_CI_AS", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_PostgreSqlOverride_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.String)
		{
			PostgreSqlOverride = "varchar(100) COLLATE \"en_US\""
		};
		Assert.AreEqual("varchar(100) COLLATE \"en_US\"", column.PostgreSqlOverride);
		Assert.AreEqual("varchar(100) COLLATE \"en_US\"", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_ForeignKeyConstraint_AllProperties_Test()
	{
		var column = new Column("DepartmentId", DbType.Int32)
		{
			ReferencedSchema = "dbo",
			ReferencedTable = "Department",
			ReferencedColumn = "DepartmentId",
			FKConstraintName = "FK_Employee_Department"
		};

		Assert.AreEqual("dbo", column.ReferencedSchema);
		Assert.AreEqual("Department", column.ReferencedTable);
		Assert.AreEqual("DepartmentId", column.ReferencedColumn);
		Assert.AreEqual("FK_Employee_Department", column.FKConstraintName);
	}

	[TestMethod]
	public void Column_CheckConstraint_AllProperties_Test()
	{
		var column = new Column("Age", DbType.Int32)
		{
			Check = "Age >= 0 AND Age <= 150",
			CheckConstraintName = "CK_Age_Range"
		};

		Assert.AreEqual("Age >= 0 AND Age <= 150", column.Check);
		Assert.AreEqual("CK_Age_Range", column.CheckConstraintName);
	}

	[TestMethod]
	public void Column_UniqueConstraint_AllProperties_Test()
	{
		var column = new Column("Email", DbType.String, 255)
		{
			IsUnique = true,
			UniqueConstraintName = "UQ_Employee_Email"
		};

		Assert.IsTrue(column.IsUnique);
		Assert.AreEqual("UQ_Employee_Email", column.UniqueConstraintName);
	}

	[TestMethod]
	public void Column_DefaultConstraint_AllProperties_Test()
	{
		var column = new Column("Status", DbType.String, 10)
		{
			Default = "'Active'",
			DefaultConstraintName = "DF_Employee_Status"
		};

		Assert.AreEqual("'Active'", column.Default);
		Assert.AreEqual("DF_Employee_Status", column.DefaultConstraintName);
	}

	[TestMethod]
	public void Column_Identity_AllProperties_Test()
	{
		var column = new Column("EmployeeId", DbType.Int32)
		{
			IsIdentity = true,
			IdentitySeed = 1000,
			IdentityIncrement = 10
		};

		Assert.IsTrue(column.IsIdentity);
		Assert.AreEqual(1000, column.IdentitySeed);
		Assert.AreEqual(10, column.IdentityIncrement);
	}

	[TestMethod]
	public void Column_Description_SetAndGet_Test()
	{
		var column = new Column("Test", DbType.String)
		{
			Description = "This is a test column description"
		};
		Assert.AreEqual("This is a test column description", column.Description);
	}

	[TestMethod]
	public void Column_IsSparse_SetAndGet_Test()
	{
		var column = new Column("OptionalData", DbType.String, 1000)
		{
			IsSparse = true
		};
		Assert.IsTrue(column.IsSparse);
	}

	[TestMethod]
	public void Column_Properties_EmptyCollection_Test()
	{
		var column = new Column("Test", DbType.String);
		Assert.AreEqual(0, column.Properties.Count);
	}

	[TestMethod]
	public void Column_Properties_MultipleItems_Test()
	{
		var column = new Column("Test", DbType.String);
		column.AddProperty("Prop1", "Value1");
		column.AddProperty("Prop2", "Value2");
		column.AddProperty("Prop3", "Value3");

		Assert.AreEqual(3, column.Properties.Count);
	}

	[TestMethod]
	public void Column_AddProperty_ReturnsColumn_Test()
	{
		var column = new Column("Test", DbType.String);
		var result = column.AddProperty("TestProp", "TestValue");

		Assert.AreSame(column, result);
	}

	[TestMethod]
	public void Column_MaxLength_LargeValue_Test()
	{
		var column = new Column("Test", DbType.String, 8000);
		Assert.AreEqual(8000, column.MaxLength);
	}

	[TestMethod]
	public void Column_Precision_MaxValue_Test()
	{
		var column = new Column("Test", DbType.Decimal, 38, 10);
		Assert.AreEqual(38, column.Precision);
		Assert.AreEqual(10, column.Scale);
	}

	[TestMethod]
	public void Column_NullableBoolean_DefaultFalse_Test()
	{
		var column = new Column("Test", DbType.Boolean);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	public void Column_NullableBoolean_ExplicitTrue_Test()
	{
		var column = new Column("Test", DbType.Boolean, true);
		Assert.IsTrue(column.IsNullable);
	}

	[TestMethod]
	public void Column_SetAllPropertiesToNull_Test()
	{
		var column = new Column("Test", DbType.String, 100);
		column.ColumnName = null!;
		column.Type = null;
		column.SqlServerType = null;
		column.PostgreSqlType = null;
		column.MaxLength = null;
		column.Precision = null;
		column.Scale = null;
		column.Default = null;
		column.Check = null;
		column.Description = null;
		column.ReferencedSchema = null;
		column.ReferencedTable = null;
		column.ReferencedColumn = null;

		Assert.IsNull(column.ColumnName);
		Assert.IsNull(column.Type);
		Assert.IsNull(column.SqlServerType);
		Assert.IsNull(column.PostgreSqlType);
		Assert.IsNull(column.MaxLength);
		Assert.IsNull(column.Precision);
		Assert.IsNull(column.Scale);
		Assert.IsNull(column.Default);
		Assert.IsNull(column.Check);
		Assert.IsNull(column.Description);
		Assert.IsNull(column.ReferencedSchema);
		Assert.IsNull(column.ReferencedTable);
		Assert.IsNull(column.ReferencedColumn);
	}

	[TestMethod]
	public void Column_HasDefault_NullDefault_Test()
	{
		var column = new Column("Test", DbType.String);
		column.Default = null;
		column.DefaultLocalTime = false;
		column.DefaultUtcTime = false;

		Assert.IsFalse(column.HasDefault);
	}

	[TestMethod]
	public void Column_TemporalTable_BothStartAndEnd_Test()
	{
		var startColumn = new Column("SysStartTime", DbType.DateTime2) { IsRowStart = true, IsHidden = true };
		var endColumn = new Column("SysEndTime", DbType.DateTime2) { IsRowEnd = true, IsHidden = true };

		Assert.IsTrue(startColumn.IsRowStart);
		Assert.IsTrue(startColumn.IsHidden);
		Assert.IsTrue(endColumn.IsRowEnd);
		Assert.IsTrue(endColumn.IsHidden);
	}

	[TestMethod]
	public void Column_AllConstraintNames_Set_Test()
	{
		var column = new Column("Test", DbType.Int32)
		{
			CheckConstraintName = "CK_Test",
			DefaultConstraintName = "DF_Test",
			FKConstraintName = "FK_Test",
			UniqueConstraintName = "UQ_Test"
		};

		Assert.AreEqual("CK_Test", column.CheckConstraintName);
		Assert.AreEqual("DF_Test", column.DefaultConstraintName);
		Assert.AreEqual("FK_Test", column.FKConstraintName);
		Assert.AreEqual("UQ_Test", column.UniqueConstraintName);
	}

	[TestMethod]
	public void Column_IdentitySeed_NegativeValue_Test()
	{
		var column = new Column("Test", DbType.Int32)
		{
			IsIdentity = true,
			IdentitySeed = -100,
			IdentityIncrement = 1
		};

		Assert.AreEqual(-100, column.IdentitySeed);
	}

	[TestMethod]
	public void Column_IdentityIncrement_NegativeValue_Test()
	{
		var column = new Column("Test", DbType.Int32)
		{
			IsIdentity = true,
			IdentitySeed = 1,
			IdentityIncrement = -1
		};

		Assert.AreEqual(-1, column.IdentityIncrement);
	}
}
