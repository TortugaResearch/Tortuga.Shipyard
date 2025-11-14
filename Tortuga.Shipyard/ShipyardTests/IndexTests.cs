using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed class IndexTests
{
	[TestMethod]
	public void Index_OrderedColumns_Test()
	{
		var index = new Index();
		index.OrderedColumns.Add("Column1");
		index.OrderedColumns.Add("Column2");

		Assert.AreEqual(2, index.OrderedColumns.Count);
		Assert.AreEqual("Column1", index.OrderedColumns[0]);
		Assert.AreEqual("Column2", index.OrderedColumns[1]);
	}

	[TestMethod]
	public void Index_IncludedColumns_Test()
	{
		var index = new Index();
		index.IncludedColumns.Add("IncludedColumn1");
		index.IncludedColumns.Add("IncludedColumn2");

		Assert.AreEqual(2, index.IncludedColumns.Count);
		Assert.AreEqual("IncludedColumn1", index.IncludedColumns[0]);
		Assert.AreEqual("IncludedColumn2", index.IncludedColumns[1]);
	}

	[TestMethod]
	public void Index_Properties_Test()
	{
		var index = new Index
		{
			IndexName = "IX_TestIndex",
			IsUnique = true,
			IsConstraint = true
		};

		Assert.AreEqual("IX_TestIndex", index.IndexName);
		Assert.IsTrue(index.IsUnique);
		Assert.IsTrue(index.IsConstraint);
	}

	[TestMethod]
	public void Index_IsConstraint_Default_Test()
	{
		var index = new Index();
		Assert.IsFalse(index.IsConstraint);
	}

	[TestMethod]
	public void Index_IsUnique_Default_Test()
	{
		var index = new Index();
		Assert.IsFalse(index.IsUnique);
	}

	[TestMethod]
	public void Index_ComplexScenario_Test()
	{
		var index = new Index
		{
			IndexName = "IX_Composite",
			IsUnique = false,
			IsConstraint = false
		};
		
		index.OrderedColumns.Add("FirstName");
		index.OrderedColumns.Add("LastName");
		index.IncludedColumns.Add("Email");
		index.IncludedColumns.Add("PhoneNumber");

		Assert.AreEqual("IX_Composite", index.IndexName);
		Assert.IsFalse(index.IsUnique);
		Assert.IsFalse(index.IsConstraint);
		Assert.AreEqual(2, index.OrderedColumns.Count);
		Assert.AreEqual(2, index.IncludedColumns.Count);
	}
}
