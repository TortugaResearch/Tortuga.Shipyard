using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Class Generator.
/// Implements the <see cref="ModelBase" />
/// </summary>
/// <seealso cref="ModelBase" />
public abstract class Generator : ModelBase
{
	/// <summary>
	/// Gets or sets a value indicating whether [escape all identifiers].
	/// </summary>
	/// <value><c>true</c> if [escape all identifiers]; otherwise, <c>false</c>.</value>
	public bool EscapeAllIdentifiers { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets the keywords.
	/// </summary>
	/// <value>The keywords.</value>
	public HashSet<string> Keywords { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Gets or sets the number of spaced to use per logical tab. If null, an actual tab will be used.
	/// </summary>
	/// <value>The size of the tab.</value>
	public int? TabSize { get => Get<int?>(); set => Set(value); }

	/// <summary>
	/// Builds the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>System.String.</returns>
	public abstract string BuildTable(Table table);

	/// <summary>
	/// Escapes text for use as a string in SQL.
	/// </summary>
	/// <param name="text">The text.</param>
	[return: NotNullIfNotNull(nameof(text))]
	public virtual string? EscapeText(string? text)
	{
		if (text == null)
			return null;

		return "'" + text.Replace("'", "''", StringComparison.InvariantCulture) + "'";
	}

	/// <summary>
	/// Escapes text for use as a string in SQL.
	/// </summary>
	/// <param name="text">The text.</param>
	[return: NotNullIfNotNull(nameof(text))]
	public virtual string? EscapeTextUnicode(string? text)
	{
		if (text == null)
			return null;

		return "N'" + text.Replace("'", "''", StringComparison.InvariantCulture) + "'";
	}

	/// <summary>
	/// Names the constraints.
	/// </summary>
	/// <param name="table">The table.</param>
	public abstract void NameConstraints(Table table);

	/// <summary>
	/// Validates the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>List&lt;ValidationResult&gt;.</returns>
	public abstract List<ValidationResult> ValidateTable(Table table);

	/// <summary>
	/// Calculates and assigns aliases for sources in the specified view.
	/// </summary>
	/// <param name="view">The view for which to calculate aliases.</param>
	public abstract void CalculateAliases(View view);

	/// <summary>
	/// Calculates and assigns join expressions for joined sources in the specified view.
	/// </summary>
	/// <param name="view">The view for which to calculate join expressions.</param>
	public abstract void CalculateJoinExpressions(View view);

	/// <summary>
	/// Generates the SQL statement for creating the specified view.
	/// </summary>
	/// <param name="view">The view to generate SQL for.</param>
	/// <returns>The SQL CREATE VIEW statement.</returns>
	public abstract string BuildView(View view);
}
