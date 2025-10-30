using NpgsqlTypes;
using System.Data;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

partial class Column : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Column" /> class with the specified column name, SQL Server type, and nullability.
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The SQL Server-specific type of the column.</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	public Column(string columnName, NpgsqlDbType type, bool isNullable = false)
	{
		ColumnName = columnName;
		PostgreSqlType = type;
		IsNullable = isNullable;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Column" /> class for SQL Server types that require a single parameter (such as max length or precision).
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The SQL Server-specific type of the column.</param>
	/// <param name="parameter1">The parameter value (max length or precision).</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">type - The data type '{type}' does not support a parameter.</exception>
	public Column(string columnName, NpgsqlDbType type, int parameter1, bool isNullable = false)
	{
		ColumnName = columnName;
		PostgreSqlType = type;
		switch (type)
		{
			case NpgsqlDbType.Char:
			case NpgsqlDbType.Bytea:
			case NpgsqlDbType.Varchar:
				MaxLength = parameter1;
				break;

			case NpgsqlDbType.Timestamp:
			case NpgsqlDbType.TimestampTz:
				Precision = parameter1;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, $"The data type '{type}' does not support a parameter.");
		}
		IsNullable = isNullable;
	}

	/// <summary>
	/// Gets the SqlDbType for PostgreSQL, converting from DbType if necessary.
	/// </summary>

	public NpgsqlDbType CalculatePostgreSqlType()
	{
		return PostgreSqlType ?? Type switch
		{
			DbType.AnsiString => NpgsqlDbType.Varchar,
			DbType.AnsiStringFixedLength => NpgsqlDbType.Char,
			DbType.Binary => NpgsqlDbType.Bytea,
			DbType.Boolean => NpgsqlDbType.Boolean,
			DbType.Byte => NpgsqlDbType.Bytea,
			DbType.Currency => NpgsqlDbType.Money,
			DbType.Date => NpgsqlDbType.Date,
			DbType.DateTime => NpgsqlDbType.Timestamp,
			DbType.DateTime2 => NpgsqlDbType.Timestamp,
			DbType.DateTimeOffset => NpgsqlDbType.TimestampTz,
			DbType.Decimal => NpgsqlDbType.Numeric,
			DbType.Double => NpgsqlDbType.Double,
			DbType.Guid => NpgsqlDbType.Uuid,
			DbType.Int16 => NpgsqlDbType.Smallint,
			DbType.Int32 => NpgsqlDbType.Integer,
			DbType.Int64 => NpgsqlDbType.Bigint,
			//DbType.Object => NpgsqlDbType,
			//DbType.SByte => System.Data.NpgsqlDbType  ,
			DbType.Single => NpgsqlDbType.Real,
			DbType.String => NpgsqlDbType.Varchar,
			DbType.StringFixedLength => NpgsqlDbType.Char,
			DbType.Time => NpgsqlDbType.Time,
			//DbType.UInt16 => System.Data.NpgsqlDbType  ,
			//DbType.UInt32 => System.Data.NpgsqlDbType  ,
			//DbType.UInt64 => System.Data.NpgsqlDbType  ,
			DbType.VarNumeric => NpgsqlDbType.Numeric,
			DbType.Xml => NpgsqlDbType.Xml,

			_ => throw new NotSupportedException($"Uknown DbType '{Type}'")
		};
	}

	/// <summary>
	/// Gets the full PostgreSQL type of the column, including length, precision, and scale as appropriate.
	/// </summary>
	/// <returns>The full type of the SQL server.</returns>
	public string CalculatePostgreSqlFullType()
	{
		if (PostgreSqlOverride != null)
			return PostgreSqlOverride;

		var typeCode = CalculatePostgreSqlType();

		return typeCode switch
		{
			NpgsqlDbType.Bigint =>
				IsIdentity ? "bigserial" :
				$"bigint",
			NpgsqlDbType.Bytea => $"bytea",
			NpgsqlDbType.Bit => $"bit",
			NpgsqlDbType.Boolean => $"boolean",
			NpgsqlDbType.Char => $"char({MaxLength})",
			NpgsqlDbType.Date => $"date",
			NpgsqlDbType.TimestampTz =>
				Precision.HasValue ? $"timestamp({Precision}) with time zone" :
				$"timestamp with time zone",
			NpgsqlDbType.Numeric =>
				Precision.HasValue && Scale.HasValue ? $"numeric({Precision},{Scale})" :
				Precision.HasValue ? $"numeric({Precision})" :
				$"numeric()",
			NpgsqlDbType.Double => $"double precision",
			NpgsqlDbType.Integer =>
				IsIdentity ? "serial" :
				$"integer",
			NpgsqlDbType.Json => $"json",
			NpgsqlDbType.Jsonb => $"jsonb",
			NpgsqlDbType.Money => $"money",
			NpgsqlDbType.Real => $"real",
			NpgsqlDbType.Smallint => $"smallint",
			NpgsqlDbType.Text => $"text",
			NpgsqlDbType.Time => $"time",
			NpgsqlDbType.Timestamp =>
				Precision.HasValue ? $"timestamp({Precision})" :
				$"timestamp",
			NpgsqlDbType.Uuid => $"uuid",
			NpgsqlDbType.Varchar => $"varchar({(MaxLength > 10485760 ? 10485760 : MaxLength)})",
			NpgsqlDbType.Xml => $"xml",

			_ => throw new NotSupportedException($"Uknown NpgsqlDbType '{typeCode}'")
		};
	}

	/// <summary>
	/// Gets or sets the type override. If not null, this overrides the type name calculated from DbType/PostgreSqlDbType.
	/// When using this, include any length, precision, or scale values as needed.
	/// </summary>
	/// <value>The PostgreSQL type override.</value>
	public string? PostgreSqlOverride { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the PostgreSQL-specific type of the column.
	/// </summary>
	public NpgsqlDbType? PostgreSqlType { get => Get<NpgsqlDbType?>(); set => Set(value); }
}