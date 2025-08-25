namespace Tortuga.Shipyard;

public record struct JoinRules
{
	/// <summary>
	/// Gets or sets the prefix to use for column aliases in join operations.
	/// </summary>
	public string? PrefixColumnAlias { get; set; }
}
