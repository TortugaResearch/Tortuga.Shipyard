using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class JoinRulesTests
{
	[TestMethod]
	public void JoinRules_DefaultConstructor_Test()
	{
		var joinRules = new JoinRules();

		Assert.IsNull(joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_SetValue_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = "tbl_"
		};

		Assert.AreEqual("tbl_", joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_ChangeValue_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = "initial_"
		};

		joinRules.PrefixColumnAlias = "changed_";

		Assert.AreEqual("changed_", joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_SetToNull_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = "prefix_"
		};

		joinRules.PrefixColumnAlias = null;

		Assert.IsNull(joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_EmptyString_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = ""
		};

		Assert.AreEqual("", joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_WithUnderscore_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = "Employee_"
		};

		Assert.AreEqual("Employee_", joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_PrefixColumnAlias_WithMultipleWords_Test()
	{
		var joinRules = new JoinRules
		{
			PrefixColumnAlias = "EmployeeDepartment_"
		};

		Assert.AreEqual("EmployeeDepartment_", joinRules.PrefixColumnAlias);
	}

	[TestMethod]
	public void JoinRules_RecordEquality_SameValues_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = "test_" };
		var joinRules2 = new JoinRules { PrefixColumnAlias = "test_" };

		Assert.AreEqual(joinRules1, joinRules2);
	}

	[TestMethod]
	public void JoinRules_RecordEquality_DifferentValues_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = "test_" };
		var joinRules2 = new JoinRules { PrefixColumnAlias = "other_" };

		Assert.AreNotEqual(joinRules1, joinRules2);
	}

	[TestMethod]
	public void JoinRules_RecordEquality_BothNull_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = null };
		var joinRules2 = new JoinRules { PrefixColumnAlias = null };

		Assert.AreEqual(joinRules1, joinRules2);
	}

	[TestMethod]
	public void JoinRules_RecordEquality_OneNull_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = "test_" };
		var joinRules2 = new JoinRules { PrefixColumnAlias = null };

		Assert.AreNotEqual(joinRules1, joinRules2);
	}

	[TestMethod]
	public void JoinRules_GetHashCode_SameValues_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = "test_" };
		var joinRules2 = new JoinRules { PrefixColumnAlias = "test_" };

		Assert.AreEqual(joinRules1.GetHashCode(), joinRules2.GetHashCode());
	}

	[TestMethod]
	public void JoinRules_GetHashCode_DifferentValues_Test()
	{
		var joinRules1 = new JoinRules { PrefixColumnAlias = "test_" };
		var joinRules2 = new JoinRules { PrefixColumnAlias = "other_" };

		Assert.AreNotEqual(joinRules1.GetHashCode(), joinRules2.GetHashCode());
	}

	[TestMethod]
	public void JoinRules_WithExpression_Test()
	{
		var joinRules = new JoinRules { PrefixColumnAlias = "original_" };

		var newRules = joinRules with { PrefixColumnAlias = "modified_" };

		Assert.AreEqual("original_", joinRules.PrefixColumnAlias);
		Assert.AreEqual("modified_", newRules.PrefixColumnAlias);
	}
}
