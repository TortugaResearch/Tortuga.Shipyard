using System.Data;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents a column in a database table, including its data type, constraints, and metadata.
/// </summary>
public partial class Column : ModelBase
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
			case DbType.String:
			case DbType.StringFixedLength:
				MaxLength = parameter1;
				break;

			case DbType.VarNumeric:
			case DbType.Decimal:
			case DbType.DateTime2:
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
	/// If true, default this column to the database's local server time.
	/// </summary>
	public bool DefaultLocalTime { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// If true, default this column to the database's time in UTC.
	/// </summary>
	public bool DefaultUtcTime { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the description of the column.
	/// </summary>
	/// <value>The description.</value>
	public string? Description { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the name of the foreign key constraint.
	/// </summary>
	/// <value>The name of the foreign key constraint.</value>
	public string? FKConstraintName { get => Get<string?>(); set => Set(value); }

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
	/// Gets or sets the precision for numeric data types.
	/// </summary>
	/// <value>The precision.</value>
	public int? Precision { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Returns a collection of extended properties
	/// </summary>
	public new ExtendedPropertyCollection Properties => GetNew<ExtendedPropertyCollection>();

	/// <summary>
	/// Gets or sets the foreign key column.
	/// </summary>
	/// <value>The foreign key column.</value>
	public string? ReferencedColumn { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the foreign key schema.
	/// </summary>
	/// <value>The foreign key schema.</value>
	public string? ReferencedSchema { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the foreign key table.
	/// </summary>
	/// <value>The foreign key table.</value>
	public string? ReferencedTable { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the scale for numeric data types.
	/// </summary>
	/// <value>The scale.</value>
	public int? Scale { get => Get<int?>(); set => Set(value); }

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
