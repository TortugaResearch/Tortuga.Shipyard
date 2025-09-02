using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tortuga.Anchor;

namespace Tortuga.Shipyard;

/// <summary>
/// Class PostgreSqlGenerator.
/// Implements the <see cref="Tortuga.Shipyard.Generator" />
/// </summary>
/// <seealso cref="Tortuga.Shipyard.Generator" />
public class PostgreSqlGenerator : Generator
{
	/// <summary>
	/// The s key words
	/// </summary>
	static readonly ImmutableHashSet<string> s_KeyWords = ImmutableHashSet.Create("ABORT", "ABSENT", "ABSOLUTE", "ACCESS", "ACTION", "ADD", "ADMIN", "AFTER", "AGGREGATE", "ALL", "ALSO", "ALTER", "ALWAYS", "ANALYSE", "ANALYZE", "AND", "ANY", "ARRAY", "AS", "ASC", "ASENSITIVE", "ASSERTION", "ASSIGNMENT", "ASYMMETRIC", "AT", "ATOMIC", "ATTACH", "ATTRIBUTE", "AUTHORIZATION", "BACKWARD", "BEFORE", "BEGIN", "BETWEEN", "BIGINT", "BINARY", "BIT", "BOOLEAN", "BOTH", "BREADTH", "BY", "CACHE", "CALL", "CALLED", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAIN", "CHAR", "CHARACTER", "CHARACTERISTICS", "CHECK", "CHECKPOINT", "CLASS", "CLOSE", "CLUSTER", "COALESCE", "COLLATE", "COLLATION", "COLUMN", "COLUMNS", "COMMENT", "COMMENTS", "COMMIT", "COMMITTED", "COMPRESSION", "CONCURRENTLY", "CONDITIONAL", "CONFIGURATION", "CONFLICT", "CONNECTION", "CONSTRAINT", "CONSTRAINTS", "CONTENT", "CONTINUE", "CONVERSION", "COPY", "COST", "CREATE", "CROSS", "CSV", "CUBE", "CURRENT", "CURRENT_CATALOG", "CURRENT_DATE", "CURRENT_ROLE", "CURRENT_SCHEMA", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR", "CYCLE", "DATA", "DATABASE", "DAY", "DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFAULTS", "DEFERRABLE", "DEFERRED", "DEFINER", "DELETE", "DELIMITER", "DELIMITERS", "DEPENDS", "DEPTH", "DESC", "DETACH", "DICTIONARY", "DISABLE", "DISCARD", "DISTINCT", "DO", "DOCUMENT", "DOMAIN", "DOUBLE", "DROP", "EACH", "ELSE", "EMPTY", "ENABLE", "ENCODING", "ENCRYPTED", "END", "ENUM", "ERROR", "ESCAPE", "EVENT", "EXCEPT", "EXCLUDE", "EXCLUDING", "EXCLUSIVE", "EXECUTE", "EXISTS", "EXPLAIN", "EXPRESSION", "EXTENSION", "EXTERNAL", "EXTRACT", "FALSE", "FAMILY", "FETCH", "FILTER", "FINALIZE", "FIRST", "FLOAT", "FOLLOWING", "FOR", "FORCE", "FOREIGN", "FORMAT", "FORWARD", "FREEZE", "FROM", "FULL", "FUNCTION", "FUNCTIONS", "GENERATED", "GLOBAL", "GRANT", "GRANTED", "GREATEST", "GROUP", "GROUPING", "GROUPS", "HANDLER", "HAVING", "HEADER", "HOLD", "HOUR", "IDENTITY", "IF", "ILIKE", "IMMEDIATE", "IMMUTABLE", "IMPLICIT", "IMPORT", "IN", "INCLUDE", "INCLUDING", "INCREMENT", "INDENT", "INDEX", "INDEXES", "INHERIT", "INHERITS", "INITIALLY", "INLINE", "INNER", "INOUT", "INPUT", "INSENSITIVE", "INSERT", "INSTEAD", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "INVOKER", "IS", "ISNULL", "ISOLATION", "JOIN", "JSON", "JSON_ARRAY", "JSON_ARRAYAGG", "JSON_EXISTS", "JSON_OBJECT", "JSON_OBJECTAGG", "JSON_QUERY", "JSON_SCALAR", "JSON_SERIALIZE", "JSON_TABLE", "JSON_VALUE", "KEEP", "KEY", "KEYS", "LABEL", "LANGUAGE", "LARGE", "LAST", "LATERAL", "LEADING", "LEAKPROOF", "LEAST", "LEFT", "LEVEL", "LIKE", "LIMIT", "LISTEN", "LOAD", "LOCAL", "LOCALTIME", "LOCALTIMESTAMP", "LOCATION", "LOCK", "LOCKED", "LOGGED", "MAPPING", "MATCH", "MATCHED", "MATERIALIZED", "MAXVALUE", "MERGE", "MERGE_ACTION", "METHOD", "MINUTE", "MINVALUE", "MODE", "MONTH", "MOVE", "NAME", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NESTED", "NEW", "NEXT", "NFC", "NFD", "NFKC", "NFKD", "NO", "NONE", "NORMALIZE", "NORMALIZED", "NOT", "NOTHING", "NOTIFY", "NOTNULL", "NOWAIT", "NULL", "NULLIF", "NULLS", "NUMERIC", "OBJECT", "OF", "OFF", "OFFSET", "OIDS", "OLD", "OMIT", "ON", "ONLY", "OPERATOR", "OPTION", "OPTIONS", "OR", "ORDER", "ORDINALITY", "OTHERS", "OUT", "OUTER", "OVER", "OVERLAPS", "OVERLAY", "OVERRIDING", "OWNED", "OWNER", "PARALLEL", "PARAMETER", "PARSER", "PARTIAL", "PARTITION", "PASSING", "PASSWORD", "PATH", "PLACING", "PLAN", "PLANS", "POLICY", "POSITION", "PRECEDING", "PRECISION", "PREPARE", "PREPARED", "PRESERVE", "PRIMARY", "PRIOR", "PRIVILEGES", "PROCEDURAL", "PROCEDURE", "PROCEDURES", "PROGRAM", "PUBLICATION", "QUOTE", "QUOTES", "RANGE", "READ", "REAL", "REASSIGN", "RECHECK", "RECURSIVE", "REF", "REFERENCES", "REFERENCING", "REFRESH", "REINDEX", "RELATIVE", "RELEASE", "RENAME", "REPEATABLE", "REPLACE", "REPLICA", "RESET", "RESTART", "RESTRICT", "RETURN", "RETURNING", "RETURNS", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROLLUP", "ROUTINE", "ROUTINES", "ROW", "ROWS", "RULE", "SAVEPOINT", "SCALAR", "SCHEMA", "SCHEMAS", "SCROLL", "SEARCH", "SECOND", "SECURITY", "SELECT", "SEQUENCE", "SEQUENCES", "SERIALIZABLE", "SERVER", "SESSION", "SESSION_USER", "SET", "SETOF", "SETS", "SHARE", "SHOW", "SIMILAR", "SIMPLE", "SKIP", "SMALLINT", "SNAPSHOT", "SOME", "SOURCE", "SQL", "STABLE", "STANDALONE", "START", "STATEMENT", "STATISTICS", "STDIN", "STDOUT", "STORAGE", "STORED", "STRICT", "STRING", "STRIP", "SUBSCRIPTION", "SUBSTRING", "SUPPORT", "SYMMETRIC", "SYSID", "SYSTEM", "SYSTEM_USER", "TABLE", "TABLES", "TABLESAMPLE", "TABLESPACE", "TARGET", "TEMP", "TEMPLATE", "TEMPORARY", "TEXT", "THEN", "TIES", "TIME", "TIMESTAMP", "TO", "TRAILING", "TRANSACTION", "TRANSFORM", "TREAT", "TRIGGER", "TRIM", "TRUE", "TRUNCATE", "TRUSTED", "TYPE", "TYPES", "UESCAPE", "UNBOUNDED", "UNCOMMITTED", "UNCONDITIONAL", "UNENCRYPTED", "UNION", "UNIQUE", "UNKNOWN", "UNLISTEN", "UNLOGGED", "UNTIL", "UPDATE", "USER", "USING", "VACUUM", "VALID", "VALIDATE", "VALIDATOR", "VALUE", "VALUES", "VARCHAR", "VARIADIC", "VARYING", "VERBOSE", "VERSION", "VIEW", "VIEWS", "VOLATILE", "WHEN", "WHERE", "WHITESPACE", "WINDOW", "WITH", "WITHIN", "WITHOUT", "WORK", "WRAPPER", "WRITE", "XML", "XMLATTRIBUTES", "XMLCONCAT", "XMLELEMENT", "XMLEXISTS", "XMLFOREST", "XMLNAMESPACES", "XMLPARSE", "XMLPI", "XMLROOT", "XMLSERIALIZE", "XMLTABLE", "YEAR", "YES", "ZONE");

	/// <summary>
	/// Initializes a new instance of the <see cref="PostgreSqlGenerator"/> class.
	/// </summary>
	public PostgreSqlGenerator()
	{
		Keywords.AddRange(s_KeyWords);
	}

	/// <summary>
	/// Gets or sets a value indicating whether to include schema name when generating a constraint name.
	/// </summary>
	public bool IncludeSchemaNameInConstraintNames { get; set; }

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
				nullString = ""; //identity is handled in the data type
			else if (column.IsNullable)
				nullString = "NULL";
			else
				nullString = "NOT NULL";

			output.Append($"\t{EscapeIdentifier(column.ColumnName)} {column.CalculatePostgreSqlFullType()} {nullString}".TrimEnd());

			//if (column.IsPrimaryKey && !table.HasCompoundPrimaryKey)
			//{
			//	if (table.PrimaryKeyConstraintName != null)
			//		output.Append($" CONSTRAINT {table.PrimaryKeyConstraintName}");
			//	output.Append(" PRIMARY KEY");
			//	if (table.ClusteredIndex != null)
			//		output.Append(" NONCLUSTERED");
			//}

			if (column.IsUnique)
			{
				if (!column.UniqueConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.UniqueConstraintName}");
				output.Append(" UNIQUE");
			}

			if (!column.Default.IsNullOrEmpty())
			{
				if (!column.DefaultConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.DefaultConstraintName}");
				output.Append($" DEFAULT {column.Default}");
			}

			if (!column.Check.IsNullOrEmpty())
			{
				if (!column.CheckConstraintName.IsNullOrEmpty())
					output.Append($" CONSTRAINT {column.CheckConstraintName}");
				output.Append($" CHECK {column.Check}");
			}

			//if (column.FKColumnName != null)
			//{
			//	if (column.FKConstraintName != null)
			//		output.Append($" CONSTRAINT {column.FKConstraintName}");
			//	output.Append($" REFERENCES {column.FKSchemaName ?? table.SchemaName}.{column.FKTableName}({column.FKColumnName})");
			//}

			output.AppendLine(",");
		}

		if (table.Columns.Any(c => c.IsPrimaryKey))
		{
			output.Append('\t');
			if (!table.PrimaryKeyConstraintName.IsNullOrEmpty())
				output.Append($"CONSTRAINT {table.PrimaryKeyConstraintName} ");
			output.AppendLine($"PRIMARY KEY ({string.Join(",", table.Columns.Where(c => c.IsPrimaryKey).Select(c => EscapeIdentifier(c.ColumnName)))}),");
		}

		foreach (var column in table.Columns)
		{
			if (!column.ReferencedColumn.IsNullOrEmpty())
			{
				output.Append('\t');
				if (!column.FKConstraintName.IsNullOrEmpty())
					output.Append($"CONSTRAINT {column.FKConstraintName} ");
				output.AppendLine($"FOREIGN KEY ({EscapeIdentifier(column.ColumnName)}) REFERENCES {EscapeIdentifier(column.ReferencedSchema ?? table.SchemaName)}.{EscapeIdentifier(column.ReferencedTable)}({EscapeIdentifier(column.ReferencedColumn)}),");
			}
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
		}

		//if (table.Description != null)
		//{
		//	output.AppendLine($"EXEC sp_addextendedproperty @name = N'MS_Description', @value = {EscapeTextUnicode(table.Description)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = NULL, @level2name = NULL;");
		//	EndBatch(output);
		//}

		//foreach (var column in table.Columns)
		//{
		//	if (column.Description != null)
		//	{
		//		output.AppendLine($"EXEC sp_addextendedproperty @name = N'MS_Description', @value = {EscapeTextUnicode(column.Description)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
		//		EndBatch(output);
		//	}

		//	foreach (var property in column.Properties)
		//	{
		//		output.AppendLine($"EXEC sp_addextendedproperty @name = N'{property.Name}', @value = {EscapeTextUnicode(property.Value)}, @level0type = N'SCHEMA', @level0name = N'{table.SchemaName}', @level1type = N'TABLE', @level1name = N'{table.TableName}', @level2type = N'COLUMN', @level2name = N'{column.ColumnName}';");
		//		EndBatch(output);
		//	}
		//}

		return output.ToString();
	}

	/// <summary>
	/// Generates the SQL statement for creating the specified view.
	/// </summary>
	/// <param name="view">The view to generate SQL for.</param>
	/// <returns>The SQL CREATE VIEW statement.</returns>
	/// <exception cref="System.ArgumentNullException">view</exception>
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
				if (!outputColumn.Expression.IsNullOrEmpty())
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

		string? LimitSize(string name) => name.Length < 64 ? name : null;

		var schemaPart = IncludeSchemaNameInConstraintNames ? $"{SnakeCaseIdentifier(table.SchemaName)}_" : "";
		var tablePart = SnakeCaseIdentifier(table.TableName);

		if (table.PrimaryKeyConstraintName.IsNullOrEmpty() && table.Columns.Any(c => c.IsPrimaryKey))
			table.PrimaryKeyConstraintName = LimitSize($"{schemaPart}{tablePart}_pkey");

		if (table.ClusteredIndex != null && table.ClusteredIndex.IndexName.IsNullOrEmpty())
			table.ClusteredIndex.IndexName = LimitSize($"{schemaPart}{tablePart}_ckey");

		//ref: https://stackoverflow.com/questions/4107915/postgresql-default-constraint-names
		foreach (var column in table.Columns)
		{
			var columnPart = SnakeCaseIdentifier(column.ColumnName);

			//if (column.DefaultConstraintName == null && column.Default != null)
			//	column.DefaultConstraintName = $"D_{columnPart}";

			if (column.CheckConstraintName.IsNullOrEmpty() && !column.Default.IsNullOrEmpty())
				column.CheckConstraintName = LimitSize($"{schemaPart}{tablePart}_{columnPart}_check");

			if (column.UniqueConstraintName.IsNullOrEmpty() && column.IsUnique)
				column.UniqueConstraintName = LimitSize($"{schemaPart}{tablePart}_{columnPart}_key");

			if (column.FKConstraintName.IsNullOrEmpty() && !column.ReferencedColumn.IsNullOrEmpty())
				column.FKConstraintName = LimitSize($"{schemaPart}{tablePart}_{columnPart}_fkey");
		}
	}

	/// <summary>
	/// Escapes the identifier.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <returns>System.Nullable&lt;System.String&gt;.</returns>
	[return: NotNullIfNotNull(nameof(identifier))]
	protected override string? EscapeIdentifier(string? identifier)
	{
		if (identifier.IsNullOrEmpty())
			return identifier;

		var result = SnakeCaseIdentifier(identifier);

		if (EscapeAllIdentifiers
				|| Keywords.Contains(result)
				|| result.Contains('.', StringComparison.Ordinal)
				|| result.Contains('-', StringComparison.Ordinal)
				|| result.Contains(' ', StringComparison.Ordinal)
				|| char.IsNumber(result[0])
			)
			return '"' + result + '"';
		else
			return result;
	}

	static void EndBatch(StringBuilder output)
	{
		output.AppendLine();
	}

	[return: NotNullIfNotNull(nameof(identifier))]
	string? SnakeCaseIdentifier(string? identifier)
	{
		if (identifier == null || identifier.Length == 0)
			return identifier;

		string result;
		if (SnakeCase)
		{
			result = char.ToLowerInvariant(identifier[0]).ToString();
			for (int i = 1; i < identifier.Length; i++)
			{
				if (char.IsUpper(identifier[i]) && char.IsLower(identifier[i - 1]))
					result += '_';
				result += char.ToLowerInvariant(identifier[i]);
			}
		}
		else
			result = identifier;
		return result;
	}
}
