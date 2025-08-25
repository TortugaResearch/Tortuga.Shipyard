using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents a collection of <see cref="Column" /> objects.
/// </summary>
public class ColumnCollection : ModelCollection<Column>
{
    /// <summary>
    /// Adds the specified item to the collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>The added <see cref="Column" />.</returns>
    public new Column Add(Column item)
    {
        base.Add(item);
        return item;
    }
}
