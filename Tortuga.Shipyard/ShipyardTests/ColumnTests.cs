using System.Data;
using NpgsqlTypes;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ColumnTests
{
	[TestMethod]
	public void Column_Constructor_DbType_Test()
	{
		var column = new Column("TestColumn", DbType.String, true);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(DbType.String, column.Type);
		Assert.IsTrue(column.IsNullable);
	}

	[TestMethod]
	public void Column_Constructor_WithLength_Test()
	{
		var column = new Column("TestColumn", DbType.String, 100, false);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(DbType.String, column.Type);
		Assert.AreEqual(100, column.MaxLength);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	public void Column_Constructor_WithPrecision_Test()
	{
		var column = new Column("TestColumn", DbType.DateTime2, 7, false);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(DbType.DateTime2, column.Type);
		Assert.AreEqual(7, column.Precision);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	public void Column_Constructor_WithPrecisionAndScale_Test()
	{
		var column = new Column("TestColumn", DbType.Decimal, 18, 2, false);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(DbType.Decimal, column.Type);
		Assert.AreEqual(18, column.Precision);
		Assert.AreEqual(2, column.Scale);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void Column_Constructor_InvalidTypeWithParameter_Test()
	{
		_ = new Column("TestColumn", DbType.Boolean, 100, false);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void Column_Constructor_InvalidTypeWithTwoParameters_Test()
	{
		_ = new Column("TestColumn", DbType.Boolean, 18, 2, false);
	}

	[TestMethod]
	public void Column_AddProperty_Test()
	{
		var column = new Column("TestColumn", DbType.String);
		column.AddProperty("TestName", "TestValue");
		
		Assert.AreEqual(1, column.Properties.Count);
		Assert.AreEqual("TestName", column.Properties[0].Name);
		Assert.AreEqual("TestValue", column.Properties[0].Value);
	}

	[TestMethod]
	public void Column_AddProperty_Chaining_Test()
	{
		var column = new Column("TestColumn", DbType.String)
			.AddProperty("Name1", "Value1")
			.AddProperty("Name2", "Value2");

		Assert.AreEqual(2, column.Properties.Count);
	}

	[TestMethod]
	public void Column_SqlServerType_Constructor_Test()
	{
		var column = new Column("TestColumn", SqlDbType.NVarChar, true);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(SqlDbType.NVarChar, column.SqlServerType);
		Assert.IsTrue(column.IsNullable);
	}

	[TestMethod]
	public void Column_PostgreSqlType_Constructor_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Varchar, true);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(NpgsqlDbType.Varchar, column.PostgreSqlType);
		Assert.IsTrue(column.IsNullable);
	}

	[TestMethod]
	public void Column_PostgreSqlType_WithLength_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Varchar, 100, false);
		Assert.AreEqual("TestColumn", column.ColumnName);
		Assert.AreEqual(NpgsqlDbType.Varchar, column.PostgreSqlType);
		Assert.AreEqual(100, column.MaxLength);
		Assert.IsFalse(column.IsNullable);
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_FromDbType_Test()
	{
		var column = new Column("TestColumn", DbType.String);
		var sqlType = column.CalculateSqlServerType();
		Assert.AreEqual(SqlDbType.NVarChar, sqlType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Int_Test()
	{
		var column = new Column("TestColumn", DbType.Int32);
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("INT", fullType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Varchar_Test()
	{
		var column = new Column("TestColumn", DbType.String, 100);
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("NVARCHAR(100)", fullType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_VarcharMax_Test()
	{
		var column = new Column("TestColumn", DbType.String, -1);
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("NVARCHAR(MAX)", fullType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Decimal_Test()
	{
		var column = new Column("TestColumn", DbType.Decimal, 18, 2);
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("DECIMAL(18,2)", fullType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_DateTime2_Test()
	{
		var column = new Column("TestColumn", DbType.DateTime2, 7);
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("DATETIME2(7)", fullType);
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Override_Test()
	{
		var column = new Column("TestColumn", DbType.String) 
		{ 
			SqlServerTypeOverride = "VARCHAR(MAX)" 
		};
		var fullType = column.CalculateSqlServerFullType();
		Assert.AreEqual("VARCHAR(MAX)", fullType);
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_FromDbType_Test()
	{
		var column = new Column("TestColumn", DbType.String);
		var pgType = column.CalculatePostgreSqlType();
		Assert.AreEqual(NpgsqlDbType.Varchar, pgType);
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Integer_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Integer);
		var fullType = column.CalculatePostgreSqlFullType();
		Assert.AreEqual("integer", fullType);
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Varchar_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Varchar, 100);
		var fullType = column.CalculatePostgreSqlFullType();
		Assert.AreEqual("varchar(100)", fullType);
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Timestamp_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Timestamp, 6);
		var fullType = column.CalculatePostgreSqlFullType();
		Assert.AreEqual("timestamp(6)", fullType);
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Override_Test()
	{
		var column = new Column("TestColumn", DbType.String) 
		{ 
			PostgreSqlOverride = "text" 
		};
		var fullType = column.CalculatePostgreSqlFullType();
		Assert.AreEqual("text", fullType);
	}

	[TestMethod]
	public void Column_CloneForHistory_Test()
	{
		// This test has been removed because CloneForHistory is an internal method
		// and is already tested through Table.CreateHistoryTable tests
	}

	[TestMethod]
	public void Column_Properties_SetAndGet_Test()
	{
		var column = new Column();
		
		column.ColumnName = "TestCol";
		column.Check = "TestCol > 0";
		column.CheckConstraintName = "CK_TestCol";
		column.Default = "0";
		column.DefaultLocalTime = true;
		column.DefaultUtcTime = false;
		column.DefaultConstraintName = "DF_TestCol";
		column.Description = "Test description";
		column.FKConstraintName = "FK_TestCol";
		column.IdentityIncrement = 1;
		column.IdentitySeed = 100;
		column.IsIdentity = true;
		column.IsNullable = true;
		column.IsPrimaryKey = true;
		column.IsSparse = true;
		column.IsUnique = true;
		column.MaxLength = 50;
		column.Precision = 18;
		column.Scale = 2;
		column.ReferencedColumn = "Id";
		column.ReferencedSchema = "dbo";
		column.ReferencedTable = "RefTable";
		column.Type = DbType.String;
		column.UniqueConstraintName = "UQ_TestCol";
		column.IsRowStart = true;
		column.IsRowEnd = true;
		column.IsHidden = true;

		Assert.AreEqual("TestCol", column.ColumnName);
		Assert.AreEqual("TestCol > 0", column.Check);
		Assert.AreEqual("CK_TestCol", column.CheckConstraintName);
		Assert.AreEqual("0", column.Default);
		Assert.IsTrue(column.DefaultLocalTime);
		Assert.IsFalse(column.DefaultUtcTime);
		Assert.AreEqual("DF_TestCol", column.DefaultConstraintName);
		Assert.AreEqual("Test description", column.Description);
		Assert.AreEqual("FK_TestCol", column.FKConstraintName);
		Assert.AreEqual(1, column.IdentityIncrement);
		Assert.AreEqual(100, column.IdentitySeed);
		Assert.IsTrue(column.IsIdentity);
		Assert.IsTrue(column.IsNullable);
		Assert.IsTrue(column.IsPrimaryKey);
		Assert.IsTrue(column.IsSparse);
		Assert.IsTrue(column.IsUnique);
		Assert.AreEqual(50, column.MaxLength);
		Assert.AreEqual(18, column.Precision);
		Assert.AreEqual(2, column.Scale);
		Assert.AreEqual("Id", column.ReferencedColumn);
		Assert.AreEqual("dbo", column.ReferencedSchema);
		Assert.AreEqual("RefTable", column.ReferencedTable);
		Assert.AreEqual(DbType.String, column.Type);
		Assert.AreEqual("UQ_TestCol", column.UniqueConstraintName);
		Assert.IsTrue(column.IsRowStart);
		Assert.IsTrue(column.IsRowEnd);
		Assert.IsTrue(column.IsHidden);
	}

	[TestMethod]
	public void Column_PostgreSqlType_WithChar_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Char, 10);
		Assert.AreEqual(NpgsqlDbType.Char, column.PostgreSqlType);
		Assert.AreEqual(10, column.MaxLength);
	}

	[TestMethod]
	public void Column_PostgreSqlType_WithBytea_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.Bytea, 100);
		Assert.AreEqual(NpgsqlDbType.Bytea, column.PostgreSqlType);
		Assert.AreEqual(100, column.MaxLength);
	}

	[TestMethod]
	public void Column_PostgreSqlType_WithTimestampTz_Test()
	{
		var column = new Column("TestColumn", NpgsqlDbType.TimestampTz, 6);
		Assert.AreEqual(NpgsqlDbType.TimestampTz, column.PostgreSqlType);
		Assert.AreEqual(6, column.Precision);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void Column_PostgreSqlType_InvalidTypeWithParameter_Test()
	{
		_ = new Column("TestColumn", NpgsqlDbType.Boolean, 100);
	}

	[TestMethod]
	public void Column_HasDefault_WithDefault_Test()
	{
		var column = new Column("TestColumn", DbType.String) { Default = "'TestValue'" };
		Assert.IsTrue(column.HasDefault);
	}

	[TestMethod]
	public void Column_HasDefault_WithDefaultLocalTime_Test()
	{
		var column = new Column("TestColumn", DbType.DateTime) { DefaultLocalTime = true };
		Assert.IsTrue(column.HasDefault);
	}

	[TestMethod]
	public void Column_HasDefault_WithDefaultUtcTime_Test()
	{
		var column = new Column("TestColumn", DbType.DateTime) { DefaultUtcTime = true };
		Assert.IsTrue(column.HasDefault);
	}

	[TestMethod]
	public void Column_HasDefault_NoDefault_Test()
	{
		var column = new Column("TestColumn", DbType.String);
		Assert.IsFalse(column.HasDefault);
	}
}
