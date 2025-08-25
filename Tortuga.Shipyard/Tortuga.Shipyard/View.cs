using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

public class JoinedViewSource : ViewSource
{
	public JoinedViewSource(string schemaName, string viewName) : base(schemaName, viewName)
	{
		//TODO: This is a view source with a join expression.
		//		Need to reference the parent table/view, type of join, and join expression.
	}
}

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
}

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

public class OutputColumn : ModelBase
{
	public OutputColumn(string? columnName)
	{
		ColumnName = columnName;
	}

	public OutputColumn(string columnName, string expression)
	{
		ColumnName = columnName;
		Expression = expression;
	}

	/// <summary>
	/// Gets or sets the name of the column.
	/// </summary>
	/// <value>Name of the column as it will be exposed by the view.</value>
	[Required]
	public string? ColumnName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the expression. If null, the column name will be used.
	/// </summary>
	/// <value>The expression.</value>
	/// <remarks>Use {0} as a placeholder for the table or view's alias.</remarks>
	public string? Expression { get => Get<string?>(); set => Set(value); }
}
