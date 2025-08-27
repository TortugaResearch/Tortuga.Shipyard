using NpgsqlTypes;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents a column in a database table, including its data type, constraints, and metadata.
/// </summary>
public class Column : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Column" /> class with the specified column name, type, and nullability.
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The general database type of the column.</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	public Column(string columnName, DbType type, bool isNullable = false)
	{
		ColumnName = columnName;
		Type = type;
		IsNullable = isNullable;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Column" /> class with the specified column name, SQL Server type, and nullability.
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The SQL Server-specific type of the column.</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	public Column(string columnName, SqlDbType type, bool isNullable = false)
	{
		ColumnName = columnName;
		SqlServerType = type;
		IsNullable = isNullable;
	}

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
	/// Initializes a new instance of the <see cref="Column" /> class for types that require precision and scale.
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The general database type of the column.</param>
	/// <param name="parameter1">The precision value.</param>
	/// <param name="parameter2">The scale value.</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">type - The data type '{type}' does not support a parameter.</exception>
	public Column(string columnName, DbType type, int parameter1, int parameter2, bool isNullable = false)
	{
		ColumnName = columnName;
		Type = type;
		switch (type)
		{
			case DbType.Decimal:
			case DbType.VarNumeric:
				Precision = parameter1;
				Scale = parameter2;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, $"The data type '{type}' does not support a parameter.");
		}
		IsNullable = isNullable;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Column" /> class for types that require a single parameter (such as max length or precision).
	/// </summary>
	/// <param name="columnName">The name of the column.</param>
	/// <param name="type">The general database type of the column.</param>
	/// <param name="parameter1">The parameter value (max length or precision).</param>
	/// <param name="isNullable">Indicates whether the column allows null values.</param>
	/// <exception cref="System.ArgumentOutOfRangeException">type - The data type '{type}' does not support a parameter.</exception>
	public Column(string columnName, DbType type, int parameter1, bool isNullable = false)
	{
		ColumnName = columnName;
		Type = type;
		switch (type)
		{
			case DbType.AnsiString:
			case DbType.AnsiStringFixedLength:
			case DbType.Binary:
			case DbType.Decimal:
			case DbType.String:
			case DbType.StringFixedLength:
			case DbType.VarNumeric:
				MaxLength = parameter1;
				break;

			case DbType.DateTime2:
				Precision = parameter1;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, $"The data type '{type}' does not support a parameter.");
		}
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
	public Column(string columnName, SqlDbType type, int parameter1, bool isNullable = false)
	{
		ColumnName = columnName;
		SqlServerType = type;
		switch (type)
		{
			case SqlDbType.Binary:
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NVarChar:
			case SqlDbType.VarBinary:
			case SqlDbType.VarChar:
				MaxLength = parameter1;
				break;

			case SqlDbType.DateTime2:
			case SqlDbType.Decimal:
				Precision = parameter1;
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, $"The data type '{type}' does not support a parameter.");
		}
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
	/// Gets or sets the check constraint expression for the column.
	/// </summary>
	/// <value>The check.</value>
	public string? Check { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the check constraint.
	/// </summary>
	/// <value>The name of the check constraint.</value>
	public string? CheckConstraintName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the column.
	/// </summary>
	/// <value>The name of the column.</value>
	public string ColumnName { get => Get<string>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the default value for the column.
	/// </summary>
	/// <value>The default value.</value>
	[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "GetDefault is not a public method.")]
	public string? Default { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the default value constraint.
	/// </summary>
	/// <value>The default name of the value constraint.</value>
	public string? DefaultConstraintName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the description of the column.
	/// </summary>
	/// <value>The description.</value>
	public string? Description { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the foreign key column.
	/// </summary>
	/// <value>The foreign key column.</value>
	public string? FKColumnName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the foreign key constraint.
	/// </summary>
	/// <value>The name of the foreign key constraint.</value>
	public string? FKConstraintName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the foreign key schema.
	/// </summary>
	/// <value>The foreign key schema.</value>
	public string? FKSchemaName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the foreign key table.
	/// </summary>
	/// <value>The foreign key table.</value>
	public string? FKTableName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the identity increment value for identity columns.
	/// </summary>
	/// <value>The identity increment.</value>
	public int? IdentityIncrement { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the identity seed value for identity columns.
	/// </summary>
	/// <value>The identity seed.</value>
	public int? IdentitySeed { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the column is an identity column.
	/// </summary>
	/// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
	public bool IsIdentity { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the column allows null values.
	/// </summary>
	/// <value><c>true</c> if this instance is nullable; otherwise, <c>false</c>.</value>
	public bool IsNullable { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the column is part of the primary key.
	/// </summary>
	/// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
	public bool IsPrimaryKey { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the column is marked as sparse (SQL Server feature).
	/// </summary>
	/// <value><c>true</c> if this instance is sparse; otherwise, <c>false</c>.</value>
	public bool IsSparse { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the column has a unique constraint.
	/// </summary>
	/// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
	public bool IsUnique { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the maximum length for variable-length data types.
	/// </summary>
	/// <value>The maximum length.</value>
	public int? MaxLength { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Gets the full SQL Server type of the column, including length, precision, and scale as appropriate.
	/// </summary>
	/// <value>The full type of the SQL server.</value>
	public string PostgreSqlFullType
	{
		get
		{
			if (PostgreSqlOverride != null)
				return PostgreSqlOverride;

			var typeCode = PostgreSqlType;

			typeCode ??= Type switch
			{
				DbType.AnsiString => NpgsqlDbType.Char,
				DbType.AnsiStringFixedLength => NpgsqlDbType.Char,
				DbType.Binary => NpgsqlDbType.Bytea,
				DbType.Boolean => NpgsqlDbType.Bit,
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

			return typeCode switch
			{
				NpgsqlDbType.Bigint =>
					IsIdentity ? "bigserial" :
					$"bigint",
				NpgsqlDbType.Bytea => $"bytea({MaxLength ?? 1})",
				NpgsqlDbType.Bit => $"bit",
				NpgsqlDbType.Char => $"char({MaxLength})",
				NpgsqlDbType.Date => $"date",
				NpgsqlDbType.TimestampTz =>
					Precision.HasValue ? $"timestamp({Precision}) with time zone" :
					$"timestamp with time zone",
				NpgsqlDbType.Numeric =>
					Precision.HasValue && Scale.HasValue ? $"numeric({Precision},{Scale})" :
					Precision.HasValue ? $"numeric({Precision})" :
					$"numeric()",
				NpgsqlDbType.Double => $"double",
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
				NpgsqlDbType.Varchar => $"varchar({MaxLength})",
				NpgsqlDbType.Xml => $"xml",

				_ => throw new NotSupportedException($"Uknown NpgsqlDbType '{typeCode}'")
			};
		}
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

	/// <summary>
	/// Gets or sets the precision for numeric data types.
	/// </summary>
	/// <value>The precision.</value>
	public int? Precision { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Returns a collection of extended properties
	/// </summary>
	public new ExtendedPropertyCollection Properties => GetNew<ExtendedPropertyCollection>();

	/// <summary>
	/// Gets or sets the scale for numeric data types.
	/// </summary>
	/// <value>The scale.</value>
	public int? Scale { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Gets the full SQL Server type of the column, including length, precision, and scale as appropriate.
	/// </summary>
	/// <value>The full type of the SQL server.</value>
	public string SqlServerFullType
	{
		get
		{
			if (SqlServerTypeOverride != null)
				return SqlServerTypeOverride;

			var typeCode = SqlServerType;

			typeCode ??= Type switch
			{
				DbType.AnsiString => SqlDbType.Char,
				DbType.AnsiStringFixedLength => SqlDbType.Char,
				DbType.Binary => SqlDbType.VarBinary,
				DbType.Boolean => SqlDbType.Bit,
				DbType.Byte => SqlDbType.TinyInt,
				DbType.Currency => SqlDbType.Money,
				DbType.Date => SqlDbType.Date,
				DbType.DateTime => SqlDbType.DateTime,
				DbType.DateTime2 => SqlDbType.DateTime2,
				DbType.DateTimeOffset => SqlDbType.DateTimeOffset,
				DbType.Decimal => SqlDbType.Decimal,
				DbType.Double => SqlDbType.Float,
				DbType.Guid => SqlDbType.UniqueIdentifier,
				DbType.Int16 => SqlDbType.SmallInt,
				DbType.Int32 => SqlDbType.Int,
				DbType.Int64 => SqlDbType.BigInt,
				DbType.Object => SqlDbType.Variant,
				//DbType.SByte => System.Data.SqlDbType  ,
				DbType.Single => SqlDbType.Real,
				DbType.String => SqlDbType.NVarChar,
				DbType.StringFixedLength => SqlDbType.NChar,
				DbType.Time => SqlDbType.Time,
				//DbType.UInt16 => System.Data.SqlDbType  ,
				//DbType.UInt32 => System.Data.SqlDbType  ,
				//DbType.UInt64 => System.Data.SqlDbType  ,
				DbType.VarNumeric => SqlDbType.Decimal,
				DbType.Xml => SqlDbType.Xml,

				_ => throw new NotSupportedException($"Uknown DbType '{Type}'")
			};

			return typeCode switch
			{
				SqlDbType.BigInt => $"BIGINT",
				SqlDbType.Binary => $"BINARY({MaxLength})",
				SqlDbType.Bit => $"BIT",
				SqlDbType.Char => $"CHAR({MaxLength})",
				SqlDbType.Date => $"DATE",
				SqlDbType.DateTime => $"DATETIME",
				SqlDbType.DateTime2 =>
					Precision.HasValue ? $"DATETIME2({Precision})" :
					$"DATETIME2",
				SqlDbType.DateTimeOffset => $"DATETIMEOFFSET",
				SqlDbType.Decimal =>
					Precision.HasValue && Scale.HasValue ? $"DECIMAL({Precision},{Scale})" :
					Precision.HasValue ? $"DECIMAL({Precision})" :
					$"DECIMAL()",
				SqlDbType.Float => $"FLOAT",
				SqlDbType.Image => $"IMAGE",
				SqlDbType.Int => $"INT",
				SqlDbType.Money => $"MONEY",
				SqlDbType.NChar => $"NCHAR({MaxLength})",
				SqlDbType.NText => $"NTEXT",
				SqlDbType.NVarChar => $"NVARCHAR({(MaxLength == -1 ? "MAX" : MaxLength)})",
				SqlDbType.Real => $"REAL",
				SqlDbType.SmallDateTime => $"SMALLDATETIME",
				SqlDbType.SmallInt => $"SMALLINT",
				SqlDbType.SmallMoney => $"SMALLMONEY",
				SqlDbType.Text => $"TEXT",
				SqlDbType.Time => $"TIME",
				SqlDbType.Timestamp => $"",
				SqlDbType.TinyInt => $"TINYINT",
				SqlDbType.UniqueIdentifier => $"UNIQUEIDENTIFIER",
				SqlDbType.VarBinary => $"VARBINARY({MaxLength})",
				SqlDbType.VarChar => $"VARCHAR({(MaxLength == -1 ? "MAX" : MaxLength)})",
				SqlDbType.Variant => $"VARIANT",
				SqlDbType.Xml => $"XML",

				_ => throw new NotSupportedException($"Uknown SqlDbType '{typeCode}'")
			};
		}
	}

	/// <summary>
	/// Gets or sets the SQL Server-specific type of the column.
	/// </summary>
	public SqlDbType? SqlServerType { get => Get<SqlDbType?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the type override. If not null, this overrides the type name calculated from DbType/SqlDbType.
	/// When using this, include any length, precision, or scale values as needed.
	/// </summary>
	/// <value>The SQL server type override.</value>
	public string? SqlServerTypeOverride { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the general database type of the column.
	/// </summary>
	/// <value>The type.</value>
	public DbType? Type { get => Get<DbType?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the unique constraint.
	/// </summary>
	/// <value>The name of the unique constraint.</value>
	public string? UniqueConstraintName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Adds an extended property.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	public Column AddProperty(string name, string value)
	{
		Properties.Add(new(name, value));
		return this;
	}
}
