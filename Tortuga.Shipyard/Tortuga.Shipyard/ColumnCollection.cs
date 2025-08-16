using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Class ColumnCollection.
/// </summary>
public class ColumnCollection : ModelCollection<Column>
{
	/// <summary>
	/// Adds the specified item.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <returns>Column.</returns>
	public new Column Add(Column item)
	{
		base.Add(item);
		return item;
	}
}
