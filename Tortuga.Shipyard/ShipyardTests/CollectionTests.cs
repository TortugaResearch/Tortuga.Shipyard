using System.Data;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class CollectionTests
{
	[TestMethod]
	public void ColumnCollection_Add_Test()
	{
		var collection = new ColumnCollection();
		var column = new Column("TestColumn", DbType.String);
		
		var result = collection.Add(column);
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreSame(column, result);
		Assert.AreSame(column, collection[0]);
	}

	[TestMethod]
	public void ColumnCollection_AddMultiple_Test()
	{
		var collection = new ColumnCollection();
		var col1 = collection.Add(new Column("Column1", DbType.Int32));
		var col2 = collection.Add(new Column("Column2", DbType.String));
		var col3 = collection.Add(new Column("Column3", DbType.Boolean));
		
		Assert.AreEqual(3, collection.Count);
		Assert.AreEqual("Column1", collection[0].ColumnName);
		Assert.AreEqual("Column2", collection[1].ColumnName);
		Assert.AreEqual("Column3", collection[2].ColumnName);
	}

	[TestMethod]
	public void ColumnCollection_Remove_Test()
	{
		var collection = new ColumnCollection();
		var column = new Column("TestColumn", DbType.String);
		collection.Add(column);
		
		collection.Remove(column);
		
		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod]
	public void ColumnCollection_Clear_Test()
	{
		var collection = new ColumnCollection();
		collection.Add(new Column("Column1", DbType.Int32));
		collection.Add(new Column("Column2", DbType.String));
		
		collection.Clear();
		
		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod]
	public void ColumnCollection_Indexer_Test()
	{
		var collection = new ColumnCollection();
		var col1 = new Column("Column1", DbType.Int32);
		var col2 = new Column("Column2", DbType.String);
		collection.Add(col1);
		collection.Add(col2);
		
		Assert.AreSame(col1, collection[0]);
		Assert.AreSame(col2, collection[1]);
	}

	[TestMethod]
	public void TableCollection_Add_Test()
	{
		var collection = new TableCollection();
		var table = new Table("dbo", "TestTable");
		
		collection.Add(table);
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreSame(table, collection[0]);
	}

	[TestMethod]
	public void TableCollection_SortByForeignKeyConstraints_NoConstraints_Test()
	{
		var collection = new TableCollection();
		var table1 = new Table("dbo", "Table1");
		table1.Columns.Add(new Column("Id", DbType.Int32));
		
		var table2 = new Table("dbo", "Table2");
		table2.Columns.Add(new Column("Id", DbType.Int32));
		
		var table3 = new Table("dbo", "Table3");
		table3.Columns.Add(new Column("Id", DbType.Int32));
		
		collection.Add(table3);
		collection.Add(table1);
		collection.Add(table2);
		
		collection.SortByForeignKeyConstraints();
		
		Assert.AreEqual(3, collection.Count);
		// Should be sorted alphabetically when no constraints
		Assert.AreEqual("Table1", collection[0].TableName);
		Assert.AreEqual("Table2", collection[1].TableName);
		Assert.AreEqual("Table3", collection[2].TableName);
	}

	[TestMethod]
	public void TableCollection_SortByForeignKeyConstraints_SimpleChain_Test()
	{
		var collection = new TableCollection();
		
		// Table3 references Table2, Table2 references Table1
		var table1 = new Table("dbo", "Table1");
		table1.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		
		var table2 = new Table("dbo", "Table2");
		table2.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table2.Columns.Add(new Column("Table1Id", DbType.Int32) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "Table1", 
			ReferencedColumn = "Id" 
		});
		
		var table3 = new Table("dbo", "Table3");
		table3.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table3.Columns.Add(new Column("Table2Id", DbType.Int32) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "Table2", 
			ReferencedColumn = "Id" 
		});
		
		// Add in reverse order
		collection.Add(table3);
		collection.Add(table2);
		collection.Add(table1);
		
		collection.SortByForeignKeyConstraints();
		
		Assert.AreEqual(3, collection.Count);
		// Table1 should be first (no dependencies), then Table2, then Table3
		Assert.AreEqual("Table1", collection[0].TableName);
		Assert.AreEqual("Table2", collection[1].TableName);
		Assert.AreEqual("Table3", collection[2].TableName);
	}

	[TestMethod]
	public void TableCollection_SortByForeignKeyConstraints_SelfReference_Test()
	{
		var collection = new TableCollection();
		
		var table1 = new Table("dbo", "Employee");
		table1.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table1.Columns.Add(new Column("ManagerId", DbType.Int32, true) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "Employee", 
			ReferencedColumn = "Id" 
		});
		
		collection.Add(table1);
		
		collection.SortByForeignKeyConstraints();
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual("Employee", collection[0].TableName);
	}

	[TestMethod]
	public void TableCollection_SortByForeignKeyConstraints_MixedTables_Test()
	{
		var collection = new TableCollection();
		
		// Independent table
		var independent = new Table("dbo", "Independent");
		independent.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		
		// Referenced table
		var parent = new Table("dbo", "Parent");
		parent.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		
		// Referencing table
		var child = new Table("dbo", "Child");
		child.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		child.Columns.Add(new Column("ParentId", DbType.Int32) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "Parent", 
			ReferencedColumn = "Id" 
		});
		
		// Add in random order
		collection.Add(child);
		collection.Add(independent);
		collection.Add(parent);
		
		collection.SortByForeignKeyConstraints();
		
		Assert.AreEqual(3, collection.Count);
		// Independent and Parent should come before Child
		Assert.IsTrue(collection[0].TableName == "Independent" || collection[0].TableName == "Parent");
		Assert.IsTrue(collection[1].TableName == "Independent" || collection[1].TableName == "Parent");
		Assert.AreEqual("Child", collection[2].TableName);
	}

	[TestMethod]
	public void IndexCollection_Add_Test()
	{
		var collection = new IndexCollection();
		var index = new Tortuga.Shipyard.Index { IndexName = "IX_Test" };
		
		collection.Add(index);
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreSame(index, collection[0]);
	}

	[TestMethod]
	public void IndexCollection_Multiple_Test()
	{
		var collection = new IndexCollection();
		var idx1 = new Tortuga.Shipyard.Index { IndexName = "IX_Test1" };
		var idx2 = new Tortuga.Shipyard.Index { IndexName = "IX_Test2" };
		
		collection.Add(idx1);
		collection.Add(idx2);
		
		Assert.AreEqual(2, collection.Count);
		Assert.AreEqual("IX_Test1", collection[0].IndexName);
		Assert.AreEqual("IX_Test2", collection[1].IndexName);
	}

	[TestMethod]
	public void ViewCollection_Add_Test()
	{
		var collection = new ViewCollection();
		var view = new View("dbo", "TestView");
		
		collection.Add(view);
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreSame(view, collection[0]);
	}

	[TestMethod]
	public void ViewCollection_Multiple_Test()
	{
		var collection = new ViewCollection();
		var view1 = new View("dbo", "View1");
		var view2 = new View("dbo", "View2");
		
		collection.Add(view1);
		collection.Add(view2);
		
		Assert.AreEqual(2, collection.Count);
		Assert.AreEqual("View1", collection[0].ViewName);
		Assert.AreEqual("View2", collection[1].ViewName);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Count_Test()
	{
		var collection = new ExtendedPropertyCollection();
		
		Assert.AreEqual(0, collection.Count);
		
		collection.Add(new ExtendedProperty("Name", "Value"));
		
		Assert.AreEqual(1, collection.Count);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Enumeration_Test()
	{
		var collection = new ExtendedPropertyCollection();
		collection.Add(new ExtendedProperty("Prop1", "Value1"));
		collection.Add(new ExtendedProperty("Prop2", "Value2"));
		
		var list = collection.ToList();
		
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual("Prop1", list[0].Name);
		Assert.AreEqual("Prop2", list[1].Name);
	}

	[TestMethod]
	public void ColumnCollection_Enumeration_Test()
	{
		var collection = new ColumnCollection();
		collection.Add(new Column("Col1", DbType.Int32));
		collection.Add(new Column("Col2", DbType.String));
		
		var list = collection.ToList();
		
		Assert.AreEqual(2, list.Count);
		Assert.AreEqual("Col1", list[0].ColumnName);
		Assert.AreEqual("Col2", list[1].ColumnName);
	}

	[TestMethod]
	public void TableCollection_Clear_Test()
	{
		var collection = new TableCollection();
		collection.Add(new Table("dbo", "Table1"));
		collection.Add(new Table("dbo", "Table2"));
		
		collection.Clear();
		
		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod]
	public void TableCollection_SortByForeignKeyConstraints_CircularReference_Test()
	{
		var collection = new TableCollection();
		
		// Create circular reference: A -> B -> A (shouldn't happen in real DB but test handling)
		var tableA = new Table("dbo", "TableA");
		tableA.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		tableA.Columns.Add(new Column("TableBId", DbType.Int32, true) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "TableB", 
			ReferencedColumn = "Id" 
		});
		
		var tableB = new Table("dbo", "TableB");
		tableB.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		tableB.Columns.Add(new Column("TableAId", DbType.Int32, true) 
		{ 
			ReferencedSchema = "dbo",
			ReferencedTable = "TableA", 
			ReferencedColumn = "Id" 
		});
		
		collection.Add(tableA);
		collection.Add(tableB);
		
		// Should not throw, but may not perfectly sort circular references
		collection.SortByForeignKeyConstraints();
		
		Assert.AreEqual(2, collection.Count);
	}
}
