using System.Collections.Frozen;
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

	readonly FrozenSet<SqlDbType> s_IdentityColumnTypes = [SqlDbType.SmallInt, SqlDbType.Int, SqlDbType.BigInt];

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
	public bool IncludeSchemaNameInConstraintNames { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether batch seperator should be used between statements.
	/// </summary>
	public bool UseBatchSeperator { get; set; }

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
			var hiddenString = (column.IsHidden) ? " HIDDEN" : "";

			string nullString;
			if (column.IsIdentity)
				if (column.IdentityIncrement.HasValue || column.IdentitySeed.HasValue)
					nullString = $"IDENTITY({column.IdentitySeed ?? 1}, {column.IdentityIncrement ?? 1})" + hiddenString;
				else
					nullString = "IDENTITY" + hiddenString;
			else if (column.IsSparse)
				nullString = "SPARSE" + hiddenString;
			else if (column.IsRowStart)
				nullString = $"GENERATED ALWAYS AS ROW START{hiddenString} NOT NULL";
			else if (column.IsRowEnd)
				nullString = $"GENERATED ALWAYS AS ROW END{hiddenString} NOT NULL";
			else if (column.IsNullable)
				nullString = "NULL" + hiddenString;
			else
				nullString = "NOT NULL" + hiddenString;

			output.Append($"\t{EscapeIdentifier(column.ColumnName)} {column.CalculateSqlServerFullType()} {nullString}");

			if (column.IsPrimaryKey && !table.HasCompoundPrimaryKey)
			{
				if (!table.PrimaryKeyConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {table.PrimaryKeyConstraintName}");
				output.Append(" PRIMARY KEY");
				if (table.ClusteredIndex != null)
					output.Append(" NONCLUSTERED");
			}

			if (column.IsUnique && !column.IsNullable)
			{
				if (!column.UniqueConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.UniqueConstraintName}");
				output.Append(" UNIQUE");
			}

			var defaultValue = column.Default;
			if (defaultValue.IsNullOrEmpty())
			{
				if (column.DefaultUtcTime)
					defaultValue = "SYSUTCDATETIME()";
				else if (column.DefaultLocalTime)
					defaultValue = "SYSDATETIME()";
			}

			if (!defaultValue.IsNullOrEmpty())
			{
				if (!column.DefaultConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.DefaultConstraintName}");
				output.Append($" DEFAULT ({defaultValue})");
			}

			if (!column.Check.IsNullOrEmpty())
			{
				if (!column.CheckConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.CheckConstraintName}");
				output.Append($" CHECK ({column.Check})");
			}

			if (!column.ReferencedColumn.IsNullOrEmpty())
			{
				if (!column.FKConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.FKConstraintName}");
				output.Append($" REFERENCES {EscapeIdentifier(column.ReferencedSchema ?? table.SchemaName)}.{EscapeIdentifier(column.ReferencedTable)}({EscapeIdentifier(column.ReferencedColumn)})");
			}

			output.AppendLine(",");
		}

		if (table.HasCompoundPrimaryKey)
		{
			output.Append('\t');
			if (!table.PrimaryKeyConstraintName.IsNullOrEmpty())
				output.Append($"CONSTRAINT {table.PrimaryKeyConstraintName} ");
			output.AppendLine($"PRIMARY KEY ({string.Join(", ", table.Columns.Where(c => c.IsPrimaryKey).Select(c => EscapeIdentifier(c.ColumnName)))}),");
		}

		var rowStart = table.Columns.FirstOrDefault(c => c.IsRowStart);
		var rowEnd = table.Columns.FirstOrDefault(c => c.IsRowEnd);
		if (rowStart != null && rowEnd != null)
		{
			output.Append('\t');
			output.AppendLine($"PERIOD FOR SYSTEM_TIME ({EscapeIdentifier(rowStart.ColumnName)}, {EscapeIdentifier(rowEnd.ColumnName)}),");
		}

		output.Remove(output.Length - 3, 1); //remove trailing comma
		output.AppendLine(")");

		if (TabSize.HasValue)
			output.Replace("\t", new string(' ', TabSize.Value));

		if (table.HistoryTableName != null)
			output.AppendLine($"WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {EscapeIdentifier(table.HistorySchemaName)}.{EscapeIdentifier(table.HistoryTableName)}))");

		output.Remove(output.Length - 2, 2); //remove trailing line break
		output.AppendLine(";");

		EndBatch(output);

		if (table.ClusteredIndex != null)
		{
			output.AppendLine($"CREATE CLUSTERED INDEX {table.ClusteredIndex.IndexName} ON {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)} ({string.Join(", ", table.ClusteredIndex.OrderedColumns.Select(c => EscapeIdentifier(c)))});");
			EndBatch(output);
		}

		foreach (var column in table.Columns.Where(c => c.IsUnique && c.IsNullable))
		{
			output.AppendLine($"CREATE UNIQUE INDEX {column.UniqueConstraintName} ON {EscapeIdentifier(table.SchemaName)}.{EscapeIdentifier(table.TableName)}({EscapeIdentifier(column.ColumnName)}) WHERE {EscapeIdentifier(column.ColumnName)} IS NOT NULL;");
			EndBatch(output);
		}
		//TODO: Support nullable unique columns
		/*
		*/

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

		if (!table.Description.IsNullOrEmpty())
		{
			output.AppendLine($"EXEC sp_addextendedproperty @name = N'MS_Description', @value = {EscapeTextUnicode(table.Description)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = NULL, @level2name = NULL;");
			EndBatch(output);
		}

		foreach (var property in table.Properties)
		{
			output.AppendLine($"EXEC sp_addextendedproperty @name = N'{property.Name}', @value = {EscapeTextUnicode(property.Value)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = NULL, @level2name = NULL;");
			EndBatch(output);
		}

		foreach (var column in table.Columns)
		{
			if (!column.Description.IsNullOrEmpty())
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

		foreach (var source in view.Sources.Where(v => v.Alias.IsNullOrEmpty()))
		{
			// Convert baseAlias to a string
			var baseAlias = new string(source.TableOrViewName!.Where(c => char.IsUpper(c)).Select(c => char.ToLower(c, CultureInfo.InvariantCulture)).ToArray());
			if (baseAlias.Length == 0)
				baseAlias = source.TableOrViewName![..1].ToLowerInvariant();

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

		foreach (var source in view.Sources.OfType<JoinedViewSource>().Where(v => v.JoinExpression.IsNullOrEmpty()))
		{
			var express = new List<string>();
			for (int i = 0; i < source.LeftJoinColumns.Count; i++)
			{
				var parentTable = view.Sources.FirstOrDefault(s => s.Outputs.OfType<ViewColumn>().Any(o => o.ColumnName == source.LeftJoinColumns[i]));
				if (parentTable == null)
					throw new InvalidOperationException($"Unable to find a source that contains column {source.LeftJoinColumns[i]}.");
				express.Add($"{EscapeIdentifier(parentTable.Alias ?? parentTable.TableOrViewName)}.{EscapeIdentifier(source.LeftJoinColumns[i])} = {EscapeIdentifier(source.Alias ?? source.TableOrViewName)}.{EscapeIdentifier(source.RightJoinColumns[i])}");
			}
			source.JoinExpression = string.Join(" AND ", express);
		}
	}

	/// <summary>
	/// Escapes the identifier.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>System.Nullable&lt;System.String&gt;.</returns>
	[return: NotNullIfNotNull(nameof(identifier))]
	public override string? EscapeIdentifier(string? identifier)
	{
		if (identifier.IsNullOrEmpty())
			return identifier;

		if (EscapeAllIdentifiers
				|| Keywords.Contains(identifier)
				|| Keywords.Contains(identifier)
				|| identifier.Any(c => !char.IsLetterOrDigit(c) && c != '_')
				|| char.IsNumber(identifier[0])
			)
			return '[' + identifier + ']';
		else
			return identifier;
	}

	/// <summary>
	/// Escapes text for use as a string in SQL.
	/// </summary>
	/// <param name="text">The text.</param>
	[return: NotNullIfNotNull(nameof(text))]
	public override string? EscapeTextUnicode(string? text)
	{
		if (text == null)
			return null;

		return "N'" + text.Replace("'", "''", StringComparison.InvariantCulture) + "'";
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

		var schemaPart = IncludeSchemaNameInConstraintNames ? $"{table.SchemaName}_" : "";

		if (table.PrimaryKeyConstraintName.IsNullOrEmpty() && table.Columns.Any(c => c.IsPrimaryKey))
			table.PrimaryKeyConstraintName = $"PK_{schemaPart}{table.TableName}";

		if (table.ClusteredIndex != null && table.ClusteredIndex.IndexName.IsNullOrEmpty())
			table.ClusteredIndex.IndexName = $"CX_{schemaPart}{table.TableName}";

		foreach (var column in table.Columns)
		{
			if (column.DefaultConstraintName.IsNullOrEmpty() && column.HasDefault)
				column.DefaultConstraintName = $"D_{schemaPart}{table.TableName}_{column.ColumnName}";

			if (column.CheckConstraintName.IsNullOrEmpty() && !column.Check.IsNullOrEmpty())
				column.CheckConstraintName = $"C_{schemaPart}{table.TableName}_{column.ColumnName}";

			if (column.UniqueConstraintName.IsNullOrEmpty() && column.IsUnique)
				column.UniqueConstraintName = $"UX_{schemaPart}{table.TableName}_{column.ColumnName}";

			if (column.FKConstraintName.IsNullOrEmpty() && !column.ReferencedColumn.IsNullOrEmpty())
				column.FKConstraintName = $"FK_{schemaPart}{table.TableName}_{column.ColumnName}";
		}
	}

	public override List<ValidationResult> Validate(Table table)
	{
		if (table == null)
			throw new ArgumentNullException(nameof(table), $"{nameof(table)} is null.");

		var result = base.Validate(table);

		foreach (var column in table.Columns)
		{
			var type = column.CalculateSqlServerType();
			if (column.IsIdentity && !s_IdentityColumnTypes.Contains(type))
				result.Add(new($"Identity column {column.ColumnName} cannot have data type {type}.", ["IsIdentity", "SqlServerType", "Type"]));
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
		else
			output.AppendLine();
	}
}
