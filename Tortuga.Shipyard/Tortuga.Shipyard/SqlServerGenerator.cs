using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Tortuga.Anchor;

namespace Tortuga.Shipyard;

/// <summary>
/// Class SqlServerGenerator.
/// Implements the <see cref="Tortuga.Shipyard.Generator" />
/// </summary>
/// <seealso cref="Tortuga.Shipyard.Generator" />
public class SqlServerGenerator : Generator
{
	/// <summary>
	/// The s key words
	/// </summary>
	static readonly ImmutableHashSet<string> s_KeyWords = ImmutableHashSet.Create("ACTION", "ADD", "ADMIN", "AFTER", "AGGREGATE", "ALIAS", "ALL", "ALLOCATE", "ALTER", "AND", "ANY", "ARE", "ARRAY", "AS", "ASC", "ASSERTION", "AT", "AUTHORIZATION", "BACKUP", "BEFORE", "BEGIN", "BETWEEN", "BINARY", "BIT", "BLOB", "BOOLEAN", "BOTH", "BREADTH", "BREAK", "BROWSE", "BSOLUTE", "BULK", "BY", "CALL", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAR", "CHARACTER", "CHECK", "CHECKPOINT", "CLASS", "CLOB", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE", "COLLATION", "COLUMN", "COMMIT", "COMPLETION", "COMPUTE", "CONNECT", "CONNECTION", "CONSTRAINT", "CONSTRAINTS", "CONSTRUCTOR", "CONTAINS", "CONTAINSTABLE", "CONTINUE", "CONVERT", "CORRESPONDING", "CREATE", "CROSS", "CUBE", "CURRENT", "CURRENT_DATE", "CURRENT_PATH", "CURRENT_ROLE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR", "CYCLE", "DATA", "DATABASE", "DATE", "DAY", "DBCC", "DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE", "DEFERRED", "DELETE", "DENY", "DEPTH", "DEREF", "DESC", "DESCRIBE", "DESCRIPTOR", "DESTROY", "DESTRUCTOR", "DETERMINISTIC", "DIAGNOSTICS", "DICTIONARY", "DISCONNECT", "DISK", "DISTINCT", "DISTRIBUTED", "DOMAIN", "DOUBLE", "DROP", "DUMMY", "DUMP", "DYNAMIC", "EACH", "ELSE", "END", "END - EXEC", "EQUALS", "ERRLVL", "ESCAPE", "EVERY", "EXCEPT", "EXCEPTION", "EXEC", "EXECUTE", "EXISTS", "EXIT", "EXTERNAL", "FALSE", "FETCH", "FILE", "FILLFACTOR", "FIRST", "FLOAT", "FOR", "FOREIGN", "FOUND", "FREE", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FUNCTION", "GENERAL", "GET", "GLOBAL", "GO", "GOTO", "GRANT", "GROUP", "GROUPING", "HAVING", "HOLDLOCK", "HOST", "HOUR", "IDENTITY", "IDENTITY_INSERT", "IDENTITYCOL", "IF", "IGNORE", "IMMEDIATE", "IN", "INDEX", "INDICATOR", "INITIALIZE", "INITIALLY", "INNER", "INOUT", "INPUT", "INSERT", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "IS", "ISOLATION", "ITERATE", "JOIN", "KEY", "KILL", "LANGUAGE", "LARGE", "LAST", "LATERAL", "LEADING", "LEFT", "LESS", "LEVEL", "LIKE", "LIMIT", "LINENO", "LOAD", "LOCAL", "LOCALTIME", "LOCALTIMESTAMP", "LOCATOR", "MAP", "MATCH", "MINUTE", "MODIFIES", "MODIFY", "MODULE", "MONTH", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NCLOB", "NEW", "NEXT", "NO", "NOCHECK", "NONCLUSTERED", "NONE", "NOT", "NULL", "NULLIF", "NUMERIC", "OBJECT", "OF", "OFF", "OFFSETS", "OLD", "ON", "ONLY", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML", "OPERATION", "OPTION", "OR", "ORDER", "ORDINALITY", "OUT", "OUTER", "OUTPUT", "OVER", "PAD", "PARAMETER", "PARAMETERS", "PARTIAL", "PATH", "PERCENT", "PLAN", "POSTFIX", "PRECISION", "PREFIX", "PREORDER", "PREPARE", "PRESERVE", "PRIMARY", "PRINT", "PRIOR", "PRIVILEGES", "PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "READ", "READS", "READTEXT", "REAL", "RECONFIGURE", "RECURSIVE", "REF", "REFERENCES", "REFERENCING", "RELATIVE", "REPLICATION", "RESTORE", "RESTRICT", "RESULT", "RETURN", "RETURNS", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROLLUP", "ROUTINE", "ROW", "ROWCOUNT", "ROWGUIDCOL", "ROWS", "RULE", "SAVE", "SAVEPOINT", "SCHEMA", "SCOPE", "SCROLL", "SEARCH", "SECOND", "SECTION", "SELECT", "SEQUENCE", "SESSION", "SESSION_USER", "SET", "SETS", "SETUSER", "SHUTDOWN", "SIZE", "SMALLINT", "SOME", "SPACE", "SPECIFIC", "SPECIFICTYPE", "SQL", "SQLEXCEPTION", "SQLSTATE", "SQLWARNING", "START", "STATE", "STATEMENT", "STATIC", "STATISTICS", "STRUCTURE", "SYSTEM_USER", "TABLE", "TEMPORARY", "TERMINATE", "TEXTSIZE", "THAN", "THEN", "TIME", "TIMESTAMP", "TIMEZONE_HOUR", "TIMEZONE_MINUTE", "TO", "TOP", "TRAILING", "TRAN", "TRANSACTION", "TRANSLATION", "TREAT", "TRIGGER", "TRUE", "TRUNCATE", "TSEQUAL", "UNDER", "UNION", "UNIQUE", "UNKNOWN", "UNNEST", "UPDATE", "UPDATETEXT", "USAGE", "USE", "USER", "USING", "VALUE", "VALUES", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHENEVER", "WHERE", "WHILE", "WITH", "WITHOUT", "WORK", "WRITE", "WRITETEXT", "YEAR", "ZONE");

	/// <summary>
	/// Initializes a new instance of the <see cref="SqlServerGenerator"/> class.
	/// </summary>
	public SqlServerGenerator()
	{
		Keywords.AddRange(s_KeyWords);
	}

	/// <summary>
	/// Gets or sets a value indicating whether to include schema name when generating a constraint name.
	/// </summary>
	public bool IncludeSchemaNameInConstraintNames { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether batch seperator should be used between statements.
	/// </summary>
	public bool UseBatchSeperator { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Builds the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>System.String.</returns>
	/// <exception cref="System.ArgumentNullException">table</exception>
	public override string BuildTable(Table table)
	{
		if (table == null)
			throw new ArgumentNullException(nameof(table), $"{nameof(table)} is null.");

		var output = new StringBuilder();

		output.AppendLine($"CREATE TABLE {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)}");
		output.AppendLine("(");

		foreach (var column in table.Columns)
		{
			string nullString;
			if (column.IsIdentity)
				if (column.IdentityIncrement.HasValue || column.IdentitySeed.HasValue)
					nullString = $"IDENTITY({column.IdentitySeed ?? 1}, {column.IdentityIncrement ?? 1})";
				else
					nullString = "IDENTITY";
			else if (column.IsSparse)
				nullString = "SPARSE";
			else if (column.IsNullable)
				nullString = "NULL";
			else
				nullString = "NOT NULL";

			output.Append($"\t{column.ColumnName} {column.SqlServerFullType} {nullString}");

			if (column.IsPrimaryKey && !table.HasCompoundPrimaryKey)
			{
				if (table.PrimaryKeyConstraintName != null)
					output.Append($" CONSTRAINT {table.PrimaryKeyConstraintName}");
				output.Append(" PRIMARY KEY");
				if (table.ClusteredIndex != null)
					output.Append(" NONCLUSTERED");
			}

			if (column.IsUnique)
			{
				if (column.UniqueConstraintName != null)
					output.Append($" CONSTRAINT {column.UniqueConstraintName}");
				output.Append(" UNIQUE");
			}

			if (column.Default != null)
			{
				if (column.DefaultConstraintName != null)
					output.Append($" CONSTRAINT {column.DefaultConstraintName}");
				output.Append($" DEFAULT {column.Default}");
			}

			if (column.Check != null)
			{
				if (column.CheckConstraintName != null)
					output.Append($" CONSTRAINT {column.CheckConstraintName}");
				output.Append($" CHECK {column.Check}");
			}

			if (column.FKColumnName != null)
			{
				if (column.FKConstraintName != null)
					output.Append($" CONSTRAINT {column.FKConstraintName}");
				output.Append($" REFERENCES {column.FKSchemaName ?? table.SchemaName}.{column.FKTableName}({column.FKColumnName})");
			}

			output.AppendLine(",");
		}

		if (table.HasCompoundPrimaryKey)
		{
			output.Append('\t');
			if (table.PrimaryKeyConstraintName != null)
				output.Append($"CONSTRAINT {table.PrimaryKeyConstraintName}");
			output.AppendLine($" PRIMARY KEY ({string.Join(",", table.Columns.Where(c => c.IsPrimaryKey).Select(c => EscapeIdentifier(c.ColumnName)))}),");
		}

		output.Remove(output.Length - 3, 1); //remove trailing comma
		output.AppendLine(");");

		if (TabSize.HasValue)
			output.Replace("\t", new string(' ', TabSize.Value));

		EndBatch(output);

		if (table.ClusteredIndex != null)
		{
			output.AppendLine($"CREATE CLUSTERED INDEX {table.ClusteredIndex.IndexName} ON {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)} ({string.Join(", ", table.ClusteredIndex.OrderedColumns.Select(c => EscapeIdentifier(c)))});");
			EndBatch(output);
		}

		foreach (var index in table.Indexes.Where(c => c.IsConstraint))
		{
			output.AppendLine($"ALTER TABLE {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)} ADD CONSTRAINT {index.IndexName} UNIQUE ({string.Join(", ", index.OrderedColumns.Select(c => EscapeIdentifier(c)))});");
			EndBatch(output);
		}

		foreach (var index in table.Indexes.Where(c => !c.IsConstraint))
		{
			var indexType = "NONCLUSTERED";
			if (index.IsUnique)
				indexType = "UNIQUE";

			var includePhrase = "";
			if (index.IncludedColumns.Count > 0)
				includePhrase = $" INCLUDE ({string.Join(", ", index.IncludedColumns.Select(c => EscapeIdentifier(c)))})";

			output.AppendLine($"CREATE {indexType} INDEX {index.IndexName} ON {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)} ({string.Join(", ", index.OrderedColumns.Select(c => EscapeIdentifier(c)))}){includePhrase};");
			EndBatch(output);
		}

		if (table.Description != null)
		{
			output.AppendLine($"EXEC sp_addextendedproperty @name = N'MS_Description', @value = {EscapeTextUnicode(table.Description)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = NULL, @level2name = NULL;");
			EndBatch(output);
		}

		foreach (var column in table.Columns)
		{
			if (column.Description != null)
			{
				output.AppendLine($"EXEC sp_addextendedproperty @name = N'MS_Description', @value = {EscapeTextUnicode(column.Description)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
				EndBatch(output);
			}

			foreach (var property in column.Properties)
			{
				output.AppendLine($"EXEC sp_addextendedproperty @name = N'{property.Name}', @value = {EscapeTextUnicode(property.Value)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
				EndBatch(output);
			}
		}

		return output.ToString();
	}

	public override void CalculateAliases(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		foreach (var source in view.Sources.Where(v => v.Alias == null))
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

	public override void CalculateJoinExpressions(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		foreach (var source in view.Sources.OfType<JoinedViewSource>().Where(v => v.JoinExpression == null))
		{
			var express = new List<string>();
			for (int i = 0; i < source.LeftJoinColumns.Count; i++)
			{
				var parentTable = view.Sources.FirstOrDefault(s => s.Outputs.Any(o => o.ColumnName == source.LeftJoinColumns[i]));
				if (parentTable == null)
					throw new InvalidOperationException($"Unable to find a source that contains column {source.LeftJoinColumns[i]}.");
				express.Add($"{EscapeIdentifier(parentTable.Alias ?? parentTable.TableOrViewName)}.{EscapeIdentifier(source.LeftJoinColumns[i])} = {EscapeIdentifier(source.Alias ?? source.TableOrViewName)}.{EscapeIdentifier(source.RightJoinColumns[i])}");
			}
			source.JoinExpression = string.Join(" AND ", express);
		}
	}

	public override string BuildView(View view)
	{
		if (view == null)
			throw new ArgumentNullException(nameof(view), $"{nameof(view)} is null.");

		var output = new StringBuilder();

		output.AppendLine($"CREATE VIEW {EscapeIdentifier(view.SchemaName)}.{EscapeIdentifier(view.ViewName)}");
		output.AppendLine("(");
		output.AppendLine("SELECT");
		foreach (var source in view.Sources)
		{
			foreach (var outputColumn in source.Outputs)
			{
				if (outputColumn.Expression != null)
					output.AppendLine($"\t{string.Format(outputColumn.Expression, EscapeIdentifier(source.Alias ?? source.TableOrViewName))} AS {EscapeIdentifier(outputColumn.ColumnName)},");
				else
					output.AppendLine($"\t{EscapeIdentifier(source.Alias ?? source.TableOrViewName)}.{EscapeIdentifier(outputColumn.ColumnName)},");
			}
		}
		output.Remove(output.Length - 3, 1); //remove trailing comma

		{
			var source = view.Sources[0];
			output.AppendLine($"FROM {EscapeIdentifier(source.Alias ?? source.TableOrViewName)}");
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
			output.AppendLine($"{joinTypeString} {EscapeIdentifier(source.Alias ?? source.TableOrViewName)}");
			if (source.JoinType != JoinType.CrossJoin)
				output.AppendLine($"\tON {source.JoinExpression}");



		}

		output.AppendLine(");");




		return output.ToString();
	}

	/// <summary>
	/// Names the constraints.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <exception cref="System.ArgumentNullException">table</exception>
	public override void NameConstraints(Table table)
	{
		if (table == null)
			throw new ArgumentNullException(nameof(table), $"{nameof(table)} is null.");

		var schemaPart = IncludeSchemaNameInConstraintNames ? $"_{table.SchemaName}" : "";

		if (table.PrimaryKeyConstraintName == null && table.Columns.Any(c => c.IsPrimaryKey))
			table.PrimaryKeyConstraintName = $"PK{schemaPart}_{table.TableName}";

		if (table.ClusteredIndex != null && table.ClusteredIndex.IndexName == null)
			table.ClusteredIndex.IndexName = $"CX{schemaPart}_{table.TableName}";

		foreach (var column in table.Columns)
		{
			if (column.DefaultConstraintName == null && column.Default != null)
				column.DefaultConstraintName = $"D_{column.ColumnName}";

			if (column.CheckConstraintName == null && column.Default != null)
				column.CheckConstraintName = $"C_{column.ColumnName}";

			if (column.UniqueConstraintName == null && column.IsUnique)
				column.UniqueConstraintName = $"UX_{column.ColumnName}";

			if (column.FKConstraintName == null && column.FKColumnName != null)
				column.FKConstraintName = $"FK{schemaPart}_{table.TableName}_{column.ColumnName}";
		}
	}

	/// <summary>
	/// Validates the table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>List&lt;ValidationResult&gt;.</returns>
	/// <exception cref="ArgumentNullException">table</exception>
	public override List<ValidationResult> ValidateTable(Table table)
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

	private void EndBatch(StringBuilder output)
	{
		if (UseBatchSeperator)
		{
			output.AppendLine("GO");
			output.AppendLine();
		}
	}

	/// <summary>
	/// Escapes the identifier.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>System.Nullable&lt;System.String&gt;.</returns>
	[return: NotNullIfNotNull(nameof(identifier))]
	string? EscapeIdentifier(string? identifier)
	{
		if (identifier == null)
			return null;

		if (EscapeAllIdentifiers || Keywords.Contains(identifier))
			return "[" + identifier + "]";
		else
			return identifier;
	}

}
