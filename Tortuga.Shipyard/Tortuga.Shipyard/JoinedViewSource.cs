namespace Tortuga.Shipyard;

public class JoinedViewSource : ViewSource
{
	public JoinedViewSource(string schemaName, string viewName, JoinType joinType, IReadOnlyList<string> leftJoinColumns, IReadOnlyList<string> rightJoinColumns) : base(schemaName, viewName)
	{
		LeftJoinColumns = leftJoinColumns;
		RightJoinColumns = rightJoinColumns;
		JoinType = joinType;
	}
	public JoinType JoinType { get => Get<JoinType>(); init => Set(value); }

	public IReadOnlyList<string> RightJoinColumns { get => Get<IReadOnlyList<string>>(); init => Set(value); }
	public IReadOnlyList<string> LeftJoinColumns { get => Get<IReadOnlyList<string>>(); init => Set(value); }

	public string? JoinExpression { get => Get<string?>(); set => Set(value); }

}
