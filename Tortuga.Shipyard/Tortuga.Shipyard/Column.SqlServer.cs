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
	public Column(string columnName, SqlDbType type, bool isNullable = false)
	{
		ColumnName = columnName;
		SqlServerType = type;
		IsNullable = isNullable;
	}

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
}
