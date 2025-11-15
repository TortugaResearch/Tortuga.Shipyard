namespace Tortuga.Shipyard;

public class JoinedViewSource : ViewSource
{
	public JoinedViewSource(string schemaName, string viewName, JoinType joinType, string leftJoinColumn, string rightJoinColumn) : base(schemaName, viewName)
	{
		LeftJoinColumns = [leftJoinColumn];
		RightJoinColumns = [rightJoinColumn];
		JoinType = joinType;
	}

	public JoinedViewSource(string schemaName, string viewName, JoinType joinType, IReadOnlyList<string> leftJoinColumns, IReadOnlyList<string> rightJoinColumns) : base(schemaName, viewName)
	{
		LeftJoinColumns = leftJoinColumns;
		RightJoinColumns = rightJoinColumns;
		JoinType = joinType;
	}

	public JoinedViewSource(string schemaName, string viewName, JoinType joinType, IReadOnlyList<string> joinColumns) : base(schemaName, viewName)
	{
		LeftJoinColumns = joinColumns;
		RightJoinColumns = joinColumns;
		JoinType = joinType;
	}

	public JoinedViewSource(string schemaName, string viewName, JoinType joinType, string joinColumn) : base(schemaName, viewName)
	{
		LeftJoinColumns = [joinColumn];
		RightJoinColumns = [joinColumn];
		JoinType = joinType;
	}

	public string? JoinExpression { get => Get<string?>(); set => Set(value); }
	public JoinType JoinType { get => Get<JoinType>(); init => Set(value); }

	public IReadOnlyList<string> LeftJoinColumns { get => Get<IReadOnlyList<string>>(); init => Set(value); }
	public IReadOnlyList<string> RightJoinColumns { get => Get<IReadOnlyList<string>>(); init => Set(value); }
}
