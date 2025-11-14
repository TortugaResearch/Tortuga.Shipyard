using System.Data;
using NpgsqlTypes;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ColumnSqlServerTypeTests
{
	[TestMethod]
	public void Column_CalculateSqlServerFullType_BigInt_Test()
	{
		var column = new Column("Test", DbType.Int64);
		Assert.AreEqual("BIGINT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Binary_Test()
	{
		var column = new Column("Test", DbType.Binary, 50);
		Assert.AreEqual("VARBINARY(50)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Bit_Test()
	{
		var column = new Column("Test", DbType.Boolean);
		Assert.AreEqual("BIT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Char_Test()
	{
		var column = new Column("Test", DbType.AnsiStringFixedLength, 10);
		Assert.AreEqual("CHAR(10)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Date_Test()
	{
		var column = new Column("Test", DbType.Date);
		Assert.AreEqual("DATE", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_DateTime_Test()
	{
		var column = new Column("Test", DbType.DateTime);
		Assert.AreEqual("DATETIME", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_DateTime2_NoPrecision_Test()
	{
		var column = new Column("Test", DbType.DateTime2);
		Assert.AreEqual("DATETIME2", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_DateTimeOffset_Test()
	{
		var column = new Column("Test", DbType.DateTimeOffset);
		Assert.AreEqual("DATETIMEOFFSET", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Decimal_NoPrecision_Test()
	{
		var column = new Column("Test", DbType.Decimal);
		Assert.AreEqual("DECIMAL()", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Decimal_PrecisionOnly_Test()
	{
		var column = new Column("Test", DbType.Decimal, 10);
		Assert.AreEqual("DECIMAL(10)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Float_Test()
	{
		var column = new Column("Test", DbType.Double);
		Assert.AreEqual("FLOAT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Image_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.Image };
		Assert.AreEqual("IMAGE", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Money_Test()
	{
		var column = new Column("Test", DbType.Currency);
		Assert.AreEqual("MONEY", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_NChar_Test()
	{
		var column = new Column("Test", DbType.StringFixedLength, 10);
		Assert.AreEqual("NCHAR(10)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_NText_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.NText };
		Assert.AreEqual("NTEXT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Real_Test()
	{
		var column = new Column("Test", DbType.Single);
		Assert.AreEqual("REAL", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_SmallDateTime_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.SmallDateTime };
		Assert.AreEqual("SMALLDATETIME", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_SmallInt_Test()
	{
		var column = new Column("Test", DbType.Int16);
		Assert.AreEqual("SMALLINT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_SmallMoney_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.SmallMoney };
		Assert.AreEqual("SMALLMONEY", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Text_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.Text };
		Assert.AreEqual("TEXT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Time_Test()
	{
		var column = new Column("Test", DbType.Time);
		Assert.AreEqual("TIME", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Timestamp_Test()
	{
		var column = new Column { SqlServerType = SqlDbType.Timestamp };
		Assert.AreEqual("", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_TinyInt_Test()
	{
		var column = new Column("Test", DbType.Byte);
		Assert.AreEqual("TINYINT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_UniqueIdentifier_Test()
	{
		var column = new Column("Test", DbType.Guid);
		Assert.AreEqual("UNIQUEIDENTIFIER", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_VarBinary_Test()
	{
		var column = new Column("Test", DbType.Binary, 100);
		Assert.AreEqual("VARBINARY(100)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_VarBinaryMax_Test()
	{
		var column = new Column("Test", DbType.Binary, -1);
		Assert.AreEqual("VARBINARY(MAX)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_VarChar_Test()
	{
		var column = new Column("Test", DbType.AnsiString, 100);
		Assert.AreEqual("VARCHAR(100)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_VarCharMax_Test()
	{
		var column = new Column("Test", DbType.AnsiString, -1);
		Assert.AreEqual("VARCHAR(MAX)", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Variant_Test()
	{
		var column = new Column("Test", DbType.Object);
		Assert.AreEqual("VARIANT", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerFullType_Xml_Test()
	{
		var column = new Column("Test", DbType.Xml);
		Assert.AreEqual("XML", column.CalculateSqlServerFullType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_AnsiString_Test()
	{
		var column = new Column("Test", DbType.AnsiString);
		Assert.AreEqual(SqlDbType.VarChar, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_AnsiStringFixedLength_Test()
	{
		var column = new Column("Test", DbType.AnsiStringFixedLength);
		Assert.AreEqual(SqlDbType.Char, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Binary_Test()
	{
		var column = new Column("Test", DbType.Binary);
		Assert.AreEqual(SqlDbType.VarBinary, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Boolean_Test()
	{
		var column = new Column("Test", DbType.Boolean);
		Assert.AreEqual(SqlDbType.Bit, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Byte_Test()
	{
		var column = new Column("Test", DbType.Byte);
		Assert.AreEqual(SqlDbType.TinyInt, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Currency_Test()
	{
		var column = new Column("Test", DbType.Currency);
		Assert.AreEqual(SqlDbType.Money, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Date_Test()
	{
		var column = new Column("Test", DbType.Date);
		Assert.AreEqual(SqlDbType.Date, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_DateTime_Test()
	{
		var column = new Column("Test", DbType.DateTime);
		Assert.AreEqual(SqlDbType.DateTime, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_DateTime2_Test()
	{
		var column = new Column("Test", DbType.DateTime2);
		Assert.AreEqual(SqlDbType.DateTime2, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_DateTimeOffset_Test()
	{
		var column = new Column("Test", DbType.DateTimeOffset);
		Assert.AreEqual(SqlDbType.DateTimeOffset, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Decimal_Test()
	{
		var column = new Column("Test", DbType.Decimal);
		Assert.AreEqual(SqlDbType.Decimal, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Double_Test()
	{
		var column = new Column("Test", DbType.Double);
		Assert.AreEqual(SqlDbType.Float, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Guid_Test()
	{
		var column = new Column("Test", DbType.Guid);
		Assert.AreEqual(SqlDbType.UniqueIdentifier, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Int16_Test()
	{
		var column = new Column("Test", DbType.Int16);
		Assert.AreEqual(SqlDbType.SmallInt, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Int32_Test()
	{
		var column = new Column("Test", DbType.Int32);
		Assert.AreEqual(SqlDbType.Int, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Int64_Test()
	{
		var column = new Column("Test", DbType.Int64);
		Assert.AreEqual(SqlDbType.BigInt, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Object_Test()
	{
		var column = new Column("Test", DbType.Object);
		Assert.AreEqual(SqlDbType.Variant, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Single_Test()
	{
		var column = new Column("Test", DbType.Single);
		Assert.AreEqual(SqlDbType.Real, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_String_Test()
	{
		var column = new Column("Test", DbType.String);
		Assert.AreEqual(SqlDbType.NVarChar, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_StringFixedLength_Test()
	{
		var column = new Column("Test", DbType.StringFixedLength);
		Assert.AreEqual(SqlDbType.NChar, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Time_Test()
	{
		var column = new Column("Test", DbType.Time);
		Assert.AreEqual(SqlDbType.Time, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_VarNumeric_Test()
	{
		var column = new Column("Test", DbType.VarNumeric);
		Assert.AreEqual(SqlDbType.Decimal, column.CalculateSqlServerType());
	}

	[TestMethod]
	public void Column_CalculateSqlServerType_Xml_Test()
	{
		var column = new Column("Test", DbType.Xml);
		Assert.AreEqual(SqlDbType.Xml, column.CalculateSqlServerType());
	}
}
