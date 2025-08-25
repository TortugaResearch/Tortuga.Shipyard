using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

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
