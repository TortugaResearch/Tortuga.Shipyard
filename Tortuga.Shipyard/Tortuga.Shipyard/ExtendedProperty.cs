using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Represents an extended property for a database object.
/// </summary>
/// <seealso cref="ModelBase" />
public class ExtendedProperty : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExtendedProperty"/> class.
	/// </summary>
	/// <param name="name">The name of the property.</param>
	/// <param name="value">The value of the property.</param>
	public ExtendedProperty(string? name, string? value)
	{
		Name = name;
		Value = value;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExtendedProperty"/> class.
	/// </summary>
	public ExtendedProperty()
	{
	}

	/// <summary>
	/// Gets or sets the name of the property.
	/// </summary>
	[Required]
	public string? Name { get => Get<string?>(); set => Set(value); }

	/// <summary>
	/// Gets or sets the value of the property.
	/// </summary>
	[Required]
	public string? Value { get => Get<string?>(); set => Set(value); }
}
