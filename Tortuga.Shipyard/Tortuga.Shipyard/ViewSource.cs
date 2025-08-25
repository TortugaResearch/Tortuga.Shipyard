using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

public class ViewSource : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Table" /> class with the specified schema and table name.
	/// </summary>
	/// <param name="schemaName">The schema name.</param>
	/// <param name="tableOrViewName">The table or view being refernced.</param>
	public ViewSource(string schemaName, string tableOrViewName)
	{
		SchemaName = schemaName;
		TableOrViewName = tableOrViewName;
	}

	/// <summary>
	/// Gets or sets the alias used when referencing columns.
	/// </summary>
	public string? Alias { get => Get<string?>(); set => Set(value); }

	public List<OutputColumn> Outputs => GetNew<List<OutputColumn>>();

	/// <summary>
	/// Gets or sets the schema name of the table.
	/// </summary>
	/// <value>The name of the schema.</value>
	public string? SchemaName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the table or view name.
	/// </summary>
	/// <value>The name of the table or view.</value>
	public string? TableOrViewName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Adds an output column based on the source table or view..
	/// </summary>
	/// <param name="columnName">The column name.</param>
	public ViewSource AddColumn(string columnName)
	{
		Outputs.Add(new(columnName));
		return this;
	}

	/// <summary>
	/// Adds an output column based on an expression.
	/// </summary>
	/// <param name="columnName">Name of the column as it will be exposed by the view.</param>
	/// <param name="expression">The expression.</param>
	/// <remarks>Use {0} as a placeholder for the table or view's alias.</remarks>
	public ViewSource AddExpression(string columnName, string expression)
	{
		Outputs.Add(new(columnName, expression));
		return this;
	}
}
