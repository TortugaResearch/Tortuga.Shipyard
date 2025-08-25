namespace Tortuga.Shipyard;


/// <summary>
/// Specifies the type of SQL join to be used in a view or query.
/// </summary>
public enum JoinType
{
	/// <summary>
	/// Represents an INNER JOIN, which returns rows when there is a match in both tables.
	/// </summary>
	InnerJoin,
	/// <summary>
	/// Represents a LEFT JOIN, which returns all rows from the left table and matched rows from the right table.
	/// </summary>
	LeftJoin,
	/// <summary>
	/// Represents a RIGHT JOIN, which returns all rows from the right table and matched rows from the left table.
	/// </summary>
	RightJoin,
	/// <summary>
	/// Represents a FULL JOIN, which returns rows when there is a match in one of the tables.
	/// </summary>
	FullJoin,
	/// <summary>
	/// Represents a CROSS JOIN, which returns the Cartesian product of both tables.
	/// </summary>
	CrossJoin
}