using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Class Index.
/// Implements the <see cref="ModelBase" />
/// </summary>
/// <seealso cref="ModelBase" />
public class Index : ModelBase
{
	/// <summary>
	/// Gets the included columns.
	/// </summary>
	/// <value>The included columns.</value>
	public List<string> IncludedColumns => GetNew<List<string>>();

	/// <summary>
	/// Gets or sets the name of the index.
	/// </summary>
	/// <value>The name of the index.</value>
	public string? IndexName { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the index is a constraint.
	/// </summary>
	public bool IsConstraint { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets or sets a value indicating whether the index is unique.
	/// </summary>
	public bool IsUnique { get => Get<bool>(); set => Set(value); }

	/// <summary>
	/// Gets the ordered columns.
	/// </summary>
	/// <value>The ordered columns.</value>
	public List<string> OrderedColumns => GetNew<List<string>>();
}
