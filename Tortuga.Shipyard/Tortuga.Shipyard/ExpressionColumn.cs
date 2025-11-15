using System.ComponentModel.DataAnnotations;

namespace Tortuga.Shipyard;

public class ExpressionColumn : OutputColumnBase
{
	public ExpressionColumn(string expression, string outputColumnName)
	{
		Expression = expression;
		OutputColumnName = outputColumnName;
	}

	/// <summary>
	/// Gets or sets the name of the column.
	/// </summary>
	/// <value>Name of the column as it exists in the source table or view.</value>
	[Required]
	public string Expression { get => Get<string>(); set => Set(value); }

	/// <summary>
	/// If this is blank, the column name will be used.
	/// </summary>
	public string? OutputColumnName { get => Get<string?>(); set => Set(value); }
}

/*
public class OutputColumn : ModelBase
{
public OutputColumn(string? columnName)
{
	ColumnName = columnName;
}

public OutputColumn(string columnName, string expression, bool isExpression)
{
	IsExpression = isExpression;
	ColumnName = columnName;
	ExpressionOrSourceColumn = expression;
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
public string? ExpressionOrSourceColumn { get => Get<string?>(); set => Set(value); }

/// <summary>
/// If false, the identifier is treated as a source column name and will undergo escaping. If true, it is treated as an expression and is used literally.
/// </summary>
public bool IsExpression { get => Get<bool>(); set => Set(value); }
}
*/
