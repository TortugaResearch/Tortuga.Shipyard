using System.ComponentModel.DataAnnotations;

namespace Tortuga.Shipyard;

public class ViewColumn : OutputColumnBase
{
	public ViewColumn(string columnName, string outputColumnName)
	{
		ColumnName = columnName;
		OutputColumnName = outputColumnName;
	}

	public ViewColumn(string columnName)
	{
		ColumnName = columnName;
	}

	/// <summary>
	/// Gets or sets the name of the column.
	/// </summary>
	/// <value>Name of the column as it exists in the source table or view.</value>
	[Required]
	public string ColumnName { get => Get<string>(); set => Set(value); }

	/// <summary>
	/// If this is blank, the column name will be used.
	/// </summary>
	public string? OutputColumnName { get => Get<string?>(); set => Set(value); }
}
