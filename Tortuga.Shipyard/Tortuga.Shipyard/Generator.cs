using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Tortuga.Anchor;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Class Generator.
/// </summary>
/// <seealso cref="ModelBase" />
public abstract class Generator
{
	/// <summary>
	/// Gets or sets a value indicating whether [escape all identifiers].
	/// </summary>
	/// <value><c>true</c> if [escape all identifiers]; otherwise, <c>false</c>.</value>
	public bool EscapeAllIdentifiers { get; set; }

	/// <summary>
	/// Gets the keywords.
	/// </summary>
	/// <value>The keywords.</value>
	public HashSet<string> Keywords { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Gets or sets the number of spaced to use per logical tab. If null, an actual tab will be used.
	/// </summary>
	/// <value>The size of the tab.</value>
	public int? TabSize { get; set; }

	/// <summary>
	/// If true, identifiers will be converted into snake_case.
	/// An underscore will be added between each lowercase-uppercase pair, then all uppercases will be converted into lowercase.
	/// </summary>
	public bool UseSnakeCase { get; set; }

	/// <summary>
	/// Builds the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>System.String.</returns>
	public abstract string BuildTable(Table table);

	public IEnumerable<string> BuildTables(IEnumerable<Table> tables)
	{
		if (tables == null)
			throw new ArgumentNullException(nameof(tables), $"{nameof(tables)} is null.");

		foreach (var item in tables)
			yield return BuildTable(item);
	}

	/// <summary>
	/// Generates the SQL statement for creating the specified view.
	/// </summary>
	/// <param name="view">The view to generate SQL for.</param>
	/// <returns>The SQL CREATE VIEW statement.</returns>
	public string BuildView(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		var output = new StringBuilder();

		output.AppendLine($"CREATE VIEW {EscapeIdentifier(view.SchemaName)}.{EscapeIdentifier(view.ViewName)}");
		output.AppendLine("AS");
		output.AppendLine("SELECT");
		foreach (var source in view.Sources)
			foreach (var outputColumn in source.Outputs)
				if (outputColumn is ViewColumn vc)
					if (vc.OutputColumnName.IsNullOrEmpty())
						output.AppendLine($"\t{source.Alias ?? EscapeIdentifier(source.TableOrViewName)}.{EscapeIdentifier(vc.ColumnName)},");
					else
						output.AppendLine($"\t{source.Alias ?? EscapeIdentifier(source.TableOrViewName)}.{EscapeIdentifier(vc.ColumnName)} AS {EscapeIdentifier(vc.OutputColumnName)},");
				else if (outputColumn is ExpressionColumn ec)
					output.AppendLine($"\t{ec.Expression} AS {EscapeIdentifier(ec.OutputColumnName)},");

		output.Remove(output.Length - 3, 1); //remove trailing comma

		{
			var source = view.Sources[0];
			output.AppendLine($"FROM {EscapeIdentifier(source.SchemaName)}.{EscapeIdentifier(source.TableOrViewName)} {source.Alias}");
		}

		foreach (JoinedViewSource source in view.Sources.Skip(1))
		{
			var joinTypeString = source.JoinType switch
			{
				JoinType.InnerJoin => "INNER JOIN",
				JoinType.LeftJoin => "LEFT JOIN",
				JoinType.RightJoin => "RIGHT JOIN",
				JoinType.FullJoin => "FULL OUTER JOIN",
				JoinType.CrossJoin => "CROSS JOIN",
				_ => throw new NotSupportedException($"Join type {source.JoinType} is not supported."),
			};
			output.AppendLine($"{joinTypeString} {EscapeIdentifier(source.SchemaName)}.{EscapeIdentifier(source.TableOrViewName)} {source.Alias}");
			if (source.JoinType != JoinType.CrossJoin)
				output.AppendLine($"\tON {source.JoinExpression}");
		}
		output.Remove(output.Length - 2, 2); //remove trailing line break
		output.AppendLine(";");
		output.AppendLine();

		return output.ToString();
	}

	/// <summary>
	/// Builds the views.
	/// </summary>
	/// <param name="views">The views.</param>
	/// <returns>IEnumerable&lt;System.String&gt;.</returns>
	/// <exception cref="System.ArgumentNullException">views</exception>
	public IEnumerable<string> BuildViews(IEnumerable<View> views)
	{
		if (views == null)
			throw new ArgumentNullException(nameof(views), $"{nameof(views)} is null.");

		foreach (var item in views)
			yield return BuildView(item);
	}

	/// <summary>
	/// Calculates and assigns aliases for sources in the specified view.
	/// </summary>
	/// <param name="view">The view for which to calculate aliases.</param>
	public virtual void CalculateAliases(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		foreach (var source in view.Sources.Where(v => v.Alias.IsNullOrEmpty()))
		{
			// Convert baseAlias to a string
			var baseAlias = new string(source.TableOrViewName!.Where(c => char.IsUpper(c)).Select(c => char.ToLower(c, CultureInfo.InvariantCulture)).ToArray());
			if (baseAlias.Length == 0)
				baseAlias = source.TableOrViewName!.Substring(0, 1).ToLowerInvariant();

			var alias = baseAlias;
			int counter = 1;
			while (view.Sources.Any(s => s != source && s.Alias == alias))
			{
				alias = baseAlias + counter.ToString();
				counter++;
			}
			source.Alias = alias;
		}
	}

	/// <summary>
	/// Calculates the aliases.
	/// </summary>
	/// <param name="views">The views.</param>
	/// <exception cref="System.ArgumentNullException">views</exception>
	public void CalculateAliases(IEnumerable<View> views)
	{
		if (views == null)
			throw new ArgumentNullException(nameof(views), $"{nameof(views)} is null.");

		foreach (var item in views)
			CalculateAliases(item);
	}

	/// <summary>
	/// Calculates the join expressions.
	/// </summary>
	/// <param name="views">The views.</param>
	/// <exception cref="System.ArgumentNullException">views</exception>
	public void CalculateJoinExpressions(IEnumerable<View> views)
	{
		if (views == null)
			throw new ArgumentNullException(nameof(views), $"{nameof(views)} is null.");

		foreach (var view in views)
			CalculateJoinExpressions(view);
	}

	/// <summary>
	/// Calculates and assigns join expressions for joined sources in the specified view.
	/// </summary>
	/// <param name="view">The view for which to calculate join expressions.</param>
	public virtual void CalculateJoinExpressions(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		foreach (var source in view.Sources.OfType<JoinedViewSource>().Where(v => v.JoinExpression == null))
		{
			var express = new List<string>();
			for (int i = 0; i < source.LeftJoinColumns.Count; i++)
			{
				var parentTable = view.Sources.FirstOrDefault(s => s.Outputs.OfType<ViewColumn>().Any(o => o.ColumnName == source.LeftJoinColumns[i]));
				if (parentTable == null)
					throw new InvalidOperationException($"Unable to find a source that contains column {source.LeftJoinColumns[i]}.");
				express.Add($"{parentTable.Alias ?? EscapeIdentifier(parentTable.TableOrViewName)}.{EscapeIdentifier(source.LeftJoinColumns[i])} = {source.Alias ?? EscapeIdentifier(source.TableOrViewName)}.{EscapeIdentifier(source.RightJoinColumns[i])}");
			}
			source.JoinExpression = string.Join(" AND ", express);
		}
	}

	[return: NotNullIfNotNull(nameof(identifier))]
	public abstract string? EscapeIdentifier(string? identifier);

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
		return EscapeText(text); //Default to only one style of string.
	}

	/// <summary>
	/// Names the constraints.
	/// </summary>
	/// <param name="table">The table.</param>
	public abstract void NameConstraints(Table table);

	/// <summary>
	/// Names the constraints.
	/// </summary>
	/// <param name="tables">The tables.</param>
	/// <exception cref="System.ArgumentNullException">tables</exception>
	public void NameConstraints(IEnumerable<Table> tables)
	{
		if (tables == null)
			throw new ArgumentNullException(nameof(tables), $"{nameof(tables)} is null.");

		foreach (var item in tables)
			NameConstraints(item);
	}

	/// <summary>
	/// Validates the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>List&lt;ValidationResult&gt;.</returns>
	/// <exception cref="ArgumentNullException">table</exception>
	public virtual List<ValidationResult> Validate(Table table)
	{
		if (table == null)
			throw new ArgumentNullException(nameof(table), $"{nameof(table)} is null.");

		table.Validate();
		var result = new List<ValidationResult>();
		result.AddRange(table.GetAllErrors());

		foreach (var column in table.Columns)
		{
			column.Validate();
			result.AddRange(column.GetAllErrors());
		}

		if (table.ClusteredIndex != null)
		{
			table.ClusteredIndex.Validate();
			result.AddRange(table.GetAllErrors());
		}

		foreach (var index in table.Indexes)
		{
			index.Validate();
			result.AddRange(index.GetAllErrors());
		}

		return result;
	}
}
