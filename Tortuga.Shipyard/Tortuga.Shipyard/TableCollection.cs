using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents a collection of <see cref="Table" /> objects.
/// </summary>
public class TableCollection : ModelCollection<Table>
{
	/// <summary>
	/// Sorts this colelction by foreign key constraints.
	/// </summary>
	public void SortByForeignKeyConstraints()
	{
		var sorted = this.Where(t => !t.HasForeignKeyConstraints).OrderBy(t => t.TableName).ToList();

		var source = this.Where(t => t.HasForeignKeyConstraints).OrderByDescending(t => t.TableName).ToList();

		var madeProgress = true; //Stop trying if we don't make any promotions for a whole loop.
		while (madeProgress && source.Count > 0)
		{
			madeProgress = false;
			for (int i = source.Count - 1; i >= 0; i--)
			{
				//Promote a table if it no longer references any table in source, other than itself.
				//It's ok if the table references a table that was already promoted.
				if (!source.Where(t => t != source[i]).Any(t => source[i].ReferencesTable(t)))
				{
					sorted.Add(source[i]);
					source.RemoveAt(i);
					madeProgress = true;
				}
			}
		}
		sorted.AddRange(source); //ideally this is zero

		Clear();
		AddRange(sorted);
	}
}
