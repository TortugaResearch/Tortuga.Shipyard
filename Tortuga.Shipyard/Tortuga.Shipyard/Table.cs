using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor;
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
	[Length(minimumLength: 1, maximumLength: int.MaxValue)]
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
	/// Gets a value indicating whether this table has any foreign key constraints.
	/// </summary>
	/// <value><c>true</c> if this instance has foreign key constraints; otherwise, <c>false</c>.</value>
	public bool HasForeignKeyConstraints => Columns.Any(c => !c.ReferencedColumn.IsNullOrEmpty());

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
	public string SchemaName { get => Get<string>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the table name.
	/// </summary>
	/// <value>The name of the table.</value>
	public string TableName { get => Get<string>(); set => Set(value); }

	/// <summary>
	/// Creates a view.
	/// </summary>
	/// <param name="schemaName">Name of the schema for the new view.</param>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="columnNames">The columns to include. If null/empty, all columns will be used.</param>
	public View CreateView(string schemaName, string viewName, params string[]? columnNames)
	{
		var result = new View(schemaName, viewName);
		var source = new ViewSource(SchemaName, TableName);
		if (columnNames == null || columnNames.Length == 0)
		{
			foreach (var column in Columns)
				source.AddColumn(column.ColumnName);
		}
		else
		{
			foreach (var column in columnNames)
				source.AddColumn(column);
		}
		result.Sources.Add(source);

		return result;
	}

	public bool ReferencesTable(Table t)
	{
		return Columns.Any(c => t.SchemaName == c.ReferencedSchema && t.TableName == c.ReferencedTable);
	}
}
