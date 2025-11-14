using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

public class View : ModelBase
{
	public View(string schemaName, string viewName)
	{
		SchemaName = schemaName;
		ViewName = viewName;
	}

	/// <summary>
	/// Gets or sets the description of the view.
	/// </summary>
	public string? Description { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the schema name of the view.
	/// </summary>
	/// <value>The name of the schema.</value>
	public string? SchemaName { get => Get<string?>(); set => Set(value); }

	public List<ViewSource> Sources => GetNew<List<ViewSource>>();

	/// <summary>
	/// Gets or sets the view name.
	/// </summary>
	/// <value>The name of the view.</value>
	public string? ViewName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Adds a join to the view using the specified join type, table, join column, and optional join rules.
	/// </summary>
	/// <param name="joinType">The type of join to use (e.g., InnerJoin, LeftJoin).</param>
	/// <param name="nameTable">The table to join with the view.</param>
	/// <param name="joinColumn">The column name on which to perform the join.</param>
	/// <param name="joinRules">Optional rules for the join, such as prefixing column aliases.</param>
	public void Join(JoinType joinType, Table nameTable, string joinColumn, JoinRules joinRules = new())
	{
		if (nameTable == null)
			throw new ArgumentNullException(nameof(nameTable), $"{nameof(nameTable)} is null.");
		if (string.IsNullOrEmpty(joinColumn))
			throw new ArgumentException($"{nameof(joinColumn)} is null or empty.", nameof(joinColumn));

		var result = new JoinedViewSource(nameTable.SchemaName, nameTable.TableName, joinType, [joinColumn], [joinColumn]);

		if (joinRules.PrefixColumnAlias == null)
			foreach (var column in nameTable.Columns.Where(c => c.ColumnName != joinColumn && !ContainsColumn(c.ColumnName)))
				result.AddColumn(column.ColumnName);
		else
			foreach (var column in nameTable.Columns)
			{
				var columnName = joinRules.PrefixColumnAlias + column.ColumnName;
				if (!ContainsColumn(columnName))
					result.AddColumn(column.ColumnName, columnName);
			}

		Sources.Add(result);
	}

	bool ContainsColumn(string columnName)
	{
		return Sources.Any(s => s.Outputs.OfType<ViewColumn>().Any(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase)));
	}
}
