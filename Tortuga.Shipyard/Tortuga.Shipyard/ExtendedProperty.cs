using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tortuga.Shipyard;

/// <summary>
/// Class ExtendedProperty.
/// Implements the <see cref="ModelBase" />
/// </summary>
/// <seealso cref="ModelBase" />
public class ExtendedProperty : ModelBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExtendedProperty"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="value">The value.</param>
	public ExtendedProperty(string? name, string? value)
	{
		Name = name;
		Value = value;
	}

	/// <summary>
	/// Creates a model using the default property bag implementation..
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
	/// Gets or sets the value.
	/// </summary>
	[Required]
	public string? Value { get => Get<string?>(); set => Set(value); }
}
