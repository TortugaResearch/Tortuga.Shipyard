using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents a database table, including its schema, columns, indexes, and primary key information.
/// </summary>
public class Table : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Table" /> class with the specified schema and table name.
	/// </summary>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableName">The table name.</param>
	public Table(string schemaName, string tableName)
	{
		SchemaName = schemaName;
		TableName = tableName;
	}

	/// <summary>
	/// Gets or sets the clustered index.
	/// </summary>
	public Index? ClusteredIndex { get => Get<Index?>(); set => Set(value); }

	/// <summary>
	/// Gets the collection of columns in the table.
	/// </summary>
	/// <value>The columns.</value>
	public ColumnCollection Columns => GetNew<ColumnCollection>();

	/// <summary>
	/// Gets or sets the description of the table.
	/// </summary>
	public string? Description { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets a value indicating whether the table has a compound (multi-column) primary key.
	/// </summary>
	/// <value><c>true</c> if this instance has compound primary key; otherwise, <c>false</c>.</value>
	public bool HasCompoundPrimaryKey => Columns.Count(c => c.IsPrimaryKey) > 1;

	/// <summary>
	/// Gets the collection of indexes defined on the table.
	/// </summary>
	/// <value>The indexes.</value>
	public IndexCollection Indexes => GetNew<IndexCollection>();

	/// <summary>
	/// Gets or sets the name of the primary key constraint.
	/// </summary>
	/// <value>The name of the primary key constraint.</value>
	public string? PrimaryKeyConstraintName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the schema name of the table.
	/// </summary>
	/// <value>The name of the schema.</value>
	public string? SchemaName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the table name.
	/// </summary>
	/// <value>The name of the table.</value>
	public string? TableName { get => Get<string?>(); set => Set(value); }

	///// <summary>
	///// Adds a clustered index.
	///// </summary>
	///// <param name="columnNames">The column names.</param>
	///// <returns>The newly created index.</returns>
	//public Index AddClusteredIndex(params string[] columnNames)
	//{
	//	if (columnNames == null)
	//		throw new ArgumentNullException(nameof(columnNames), $"{nameof(columnNames)} is null.");

	//	var result = new Index();
	//	foreach (var column in columnNames)
	//		result.OrderedColumns.Add(Columns.Single(c => c.ColumnName == column));

	//	return result;
	//}
}
