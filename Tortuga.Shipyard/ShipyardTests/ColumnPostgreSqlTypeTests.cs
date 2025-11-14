using System.Data;
using NpgsqlTypes;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ColumnPostgreSqlTypeTests
{
	[TestMethod]
	public void Column_CalculatePostgreSqlType_AnsiString_Test()
	{
		var column = new Column("Test", DbType.AnsiString);
		Assert.AreEqual(NpgsqlDbType.Varchar, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_AnsiStringFixedLength_Test()
	{
		var column = new Column("Test", DbType.AnsiStringFixedLength);
		Assert.AreEqual(NpgsqlDbType.Char, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Binary_Test()
	{
		var column = new Column("Test", DbType.Binary);
		Assert.AreEqual(NpgsqlDbType.Bytea, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Boolean_Test()
	{
		var column = new Column("Test", DbType.Boolean);
		Assert.AreEqual(NpgsqlDbType.Boolean, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Byte_Test()
	{
		var column = new Column("Test", DbType.Byte);
		Assert.AreEqual(NpgsqlDbType.Bytea, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Currency_Test()
	{
		var column = new Column("Test", DbType.Currency);
		Assert.AreEqual(NpgsqlDbType.Money, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Date_Test()
	{
		var column = new Column("Test", DbType.Date);
		Assert.AreEqual(NpgsqlDbType.Date, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_DateTime_Test()
	{
		var column = new Column("Test", DbType.DateTime);
		Assert.AreEqual(NpgsqlDbType.Timestamp, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_DateTime2_Test()
	{
		var column = new Column("Test", DbType.DateTime2);
		Assert.AreEqual(NpgsqlDbType.Timestamp, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_DateTimeOffset_Test()
	{
		var column = new Column("Test", DbType.DateTimeOffset);
		Assert.AreEqual(NpgsqlDbType.TimestampTz, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Decimal_Test()
	{
		var column = new Column("Test", DbType.Decimal);
		Assert.AreEqual(NpgsqlDbType.Numeric, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Double_Test()
	{
		var column = new Column("Test", DbType.Double);
		Assert.AreEqual(NpgsqlDbType.Double, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Guid_Test()
	{
		var column = new Column("Test", DbType.Guid);
		Assert.AreEqual(NpgsqlDbType.Uuid, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Int16_Test()
	{
		var column = new Column("Test", DbType.Int16);
		Assert.AreEqual(NpgsqlDbType.Smallint, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Int32_Test()
	{
		var column = new Column("Test", DbType.Int32);
		Assert.AreEqual(NpgsqlDbType.Integer, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Int64_Test()
	{
		var column = new Column("Test", DbType.Int64);
		Assert.AreEqual(NpgsqlDbType.Bigint, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Single_Test()
	{
		var column = new Column("Test", DbType.Single);
		Assert.AreEqual(NpgsqlDbType.Real, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_String_Test()
	{
		var column = new Column("Test", DbType.String);
		Assert.AreEqual(NpgsqlDbType.Varchar, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_StringFixedLength_Test()
	{
		var column = new Column("Test", DbType.StringFixedLength);
		Assert.AreEqual(NpgsqlDbType.Char, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Time_Test()
	{
		var column = new Column("Test", DbType.Time);
		Assert.AreEqual(NpgsqlDbType.Time, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_VarNumeric_Test()
	{
		var column = new Column("Test", DbType.VarNumeric);
		Assert.AreEqual(NpgsqlDbType.Numeric, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlType_Xml_Test()
	{
		var column = new Column("Test", DbType.Xml);
		Assert.AreEqual(NpgsqlDbType.Xml, column.CalculatePostgreSqlType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Bigint_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Bigint);
		Assert.AreEqual("bigint", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Bytea_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Bytea);
		Assert.AreEqual("bytea", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Bit_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Bit);
		Assert.AreEqual("bit", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Boolean_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Boolean);
		Assert.AreEqual("boolean", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Char_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Char, 10);
		Assert.AreEqual("char(10)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Date_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Date);
		Assert.AreEqual("date", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_TimestampTz_NoPrecision_Test()
	{
		var column = new Column("Test", NpgsqlDbType.TimestampTz);
		Assert.AreEqual("timestamp with time zone", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_TimestampTz_WithPrecision_Test()
	{
		var column = new Column("Test", NpgsqlDbType.TimestampTz, 6);
		Assert.AreEqual("timestamp(6) with time zone", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Numeric_NoPrecision_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Numeric);
		Assert.AreEqual("numeric()", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Numeric_PrecisionOnly_Test()
	{
		var column = new Column("Test", DbType.Decimal, 10);
		Assert.AreEqual("numeric(10)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Numeric_PrecisionAndScale_Test()
	{
		var column = new Column("Test", DbType.Decimal, 18, 2);
		Assert.AreEqual("numeric(18,2)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Double_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Double);
		Assert.AreEqual("double precision", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Integer_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Integer);
		Assert.AreEqual("integer", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Json_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Json);
		Assert.AreEqual("json", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Jsonb_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Jsonb);
		Assert.AreEqual("jsonb", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Money_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Money);
		Assert.AreEqual("money", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Real_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Real);
		Assert.AreEqual("real", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Smallint_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Smallint);
		Assert.AreEqual("smallint", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Text_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Text);
		Assert.AreEqual("text", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Time_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Time);
		Assert.AreEqual("time", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Timestamp_NoPrecision_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Timestamp);
		Assert.AreEqual("timestamp", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Timestamp_WithPrecision_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Timestamp, 6);
		Assert.AreEqual("timestamp(6)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Uuid_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Uuid);
		Assert.AreEqual("uuid", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Varchar_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Varchar, 100);
		Assert.AreEqual("varchar(100)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Varchar_MaxLength_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Varchar, 20000000);
		Assert.AreEqual("varchar(10485760)", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_CalculatePostgreSqlFullType_Xml_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Xml);
		Assert.AreEqual("xml", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_PostgreSqlType_SetDirectly_Test()
	{
		var column = new Column { PostgreSqlType = NpgsqlDbType.Json };
		Assert.AreEqual(NpgsqlDbType.Json, column.PostgreSqlType);
	}

	[TestMethod]
	public void Column_PostgreSqlOverride_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Varchar, 100)
		{
			PostgreSqlOverride = "text"
		};
		Assert.AreEqual("text", column.CalculatePostgreSqlFullType());
	}

	[TestMethod]
	public void Column_PostgreSqlOverride_CustomType_Test()
	{
		var column = new Column("Test", NpgsqlDbType.Varchar)
		{
			PostgreSqlOverride = "citext"
		};
		Assert.AreEqual("citext", column.CalculatePostgreSqlFullType());
	}
}
