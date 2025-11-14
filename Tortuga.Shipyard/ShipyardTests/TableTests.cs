using System.Data;
using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed class TableTests
{
	[TestMethod]
	public void Table_ClusteredIndex_Test()
	{
		var table = new Table("dbo", "TestTable");
		var clusteredIndex = new Index { IndexName = "CX_Test" };
		clusteredIndex.OrderedColumns.Add("Id");
		table.ClusteredIndex = clusteredIndex;

		Assert.IsNotNull(table.ClusteredIndex);
		Assert.AreEqual("CX_Test", table.ClusteredIndex.IndexName);
	}

	[TestMethod]
	public void Table_Columns_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32));
		table.Columns.Add(new Column("Name", DbType.String, 50));

		Assert.AreEqual(2, table.Columns.Count);
		Assert.AreEqual("Id", table.Columns[0].ColumnName);
		Assert.AreEqual("Name", table.Columns[1].ColumnName);
	}

	[TestMethod]
	public void Table_Constructor_Test()
	{
		var table = new Table("dbo", "TestTable");
		Assert.AreEqual("dbo", table.SchemaName);
		Assert.AreEqual("TestTable", table.TableName);
	}

	[TestMethod]
	public void Table_CreateHistoryTable_DefaultSchemaName_Test()
	{
		var table = new Table("Data", "Address")
		{
			HistoryTableName = "AddressHistory"
		};

		table.Columns.Add(new Column("AddressKey", DbType.Int32));

		var historyTable = table.CreateHistoryTable();

		Assert.AreEqual("Data", historyTable.SchemaName);
		Assert.AreEqual("AddressHistory", historyTable.TableName);
	}

	[TestMethod]
	public void Table_CreateHistoryTable_NoHistoryTableName_Test()
	{
		var table = new Table("Data", "Address") { HistorySchemaName = "History" };
		table.Columns.Add(new Column("AddressKey", DbType.Int32));

		Assert.ThrowsException<InvalidOperationException>(() => table.CreateHistoryTable());
	}

	[TestMethod]
	public void Table_CreateHistoryTable_Test()
	{
		var table = new Table("Data", "Address")
		{
			HistorySchemaName = "History",
			HistoryTableName = "AddressHistory"
		};

		table.Columns.Add(new Column("AddressKey", DbType.Int32)
		{
			IsIdentity = true,
			IsPrimaryKey = true
		});
		table.Columns.Add(new Column("AddressLine1", DbType.String, 50, true));
		table.Columns.Add(new Column("ValidFromDateTime", DbType.DateTime2, 7)
		{
			IsRowStart = true
		});
		table.Columns.Add(new Column("ValidToDateTime", DbType.DateTime2, 7)
		{
			IsRowEnd = true,
			IsHidden = true
		});

		var historyTable = table.CreateHistoryTable();

		Assert.AreEqual("History", historyTable.SchemaName);
		Assert.AreEqual("AddressHistory", historyTable.TableName);
		Assert.AreEqual(4, historyTable.Columns.Count);

		// Verify that the history columns have been properly cleaned
		var addressKeyColumn = historyTable.Columns[0];
		Assert.AreEqual("AddressKey", addressKeyColumn.ColumnName);
		Assert.IsFalse(addressKeyColumn.IsIdentity);
		Assert.IsFalse(addressKeyColumn.IsPrimaryKey);

		var validFromColumn = historyTable.Columns[2];
		Assert.AreEqual("ValidFromDateTime", validFromColumn.ColumnName);
		Assert.IsFalse(validFromColumn.IsRowStart);

		var validToColumn = historyTable.Columns[3];
		Assert.AreEqual("ValidToDateTime", validToColumn.ColumnName);
		Assert.IsFalse(validToColumn.IsRowEnd);
		Assert.IsFalse(validToColumn.IsHidden);
	}

	[TestMethod]
	public void Table_CreateView_AllColumns_Test()
	{
		var table = new Table("Storage", "Employee");
		table.Columns.Add(new Column("EmployeeId", DbType.Int32));
		table.Columns.Add(new Column("FirstName", DbType.String, 50));
		table.Columns.Add(new Column("LastName", DbType.String, 50));

		var view = table.CreateView("Reporting", "EmployeeView");

		Assert.AreEqual("Reporting", view.SchemaName);
		Assert.AreEqual("EmployeeView", view.ViewName);
		Assert.AreEqual(1, view.Sources.Count);
		Assert.AreEqual(3, view.Sources[0].Outputs.Count);
	}

	[TestMethod]
	public void Table_CreateView_SpecificColumns_Test()
	{
		var table = new Table("Storage", "Employee");
		table.Columns.Add(new Column("EmployeeId", DbType.Int32));
		table.Columns.Add(new Column("FirstName", DbType.String, 50));
		table.Columns.Add(new Column("LastName", DbType.String, 50));
		table.Columns.Add(new Column("SSN", DbType.String, 11));

		var view = table.CreateView("Reporting", "EmployeeView", "EmployeeId", "FirstName", "LastName");

		Assert.AreEqual("Reporting", view.SchemaName);
		Assert.AreEqual("EmployeeView", view.ViewName);
		Assert.AreEqual(1, view.Sources.Count);
		Assert.AreEqual(3, view.Sources[0].Outputs.Count);
		Assert.AreEqual("EmployeeId", ((ViewColumn)view.Sources[0].Outputs[0]).ColumnName);
		Assert.AreEqual("FirstName", ((ViewColumn)view.Sources[0].Outputs[1]).ColumnName);
		Assert.AreEqual("LastName", ((ViewColumn)view.Sources[0].Outputs[2]).ColumnName);
	}

	[TestMethod]
	public void Table_Description_Test()
	{
		var table = new Table("dbo", "TestTable")
		{
			Description = "This is a test table"
		};

		Assert.AreEqual("This is a test table", table.Description);
	}

	[TestMethod]
	public void Table_HasCompoundPrimaryKey_False_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Name", DbType.String, 50));

		Assert.IsFalse(table.HasCompoundPrimaryKey);
	}

	[TestMethod]
	public void Table_HasCompoundPrimaryKey_True_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id1", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Id2", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Name", DbType.String, 50));

		Assert.IsTrue(table.HasCompoundPrimaryKey);
	}

	[TestMethod]
	public void Table_HasForeignKeyConstraints_False_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("Name", DbType.String, 50));

		Assert.IsFalse(table.HasForeignKeyConstraints);
	}

	[TestMethod]
	public void Table_HasForeignKeyConstraints_True_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("ParentId", DbType.Int32)
		{
			ReferencedTable = "Parent",
			ReferencedColumn = "Id"
		});

		Assert.IsTrue(table.HasForeignKeyConstraints);
	}

	[TestMethod]
	public void Table_HistoryTable_Test()
	{
		var table = new Table("Data", "Address")
		{
			HistorySchemaName = "History",
			HistoryTableName = "Address"
		};

		Assert.AreEqual("History", table.HistorySchemaName);
		Assert.AreEqual("Address", table.HistoryTableName);
	}

	[TestMethod]
	public void Table_Indexes_Test()
	{
		var table = new Table("dbo", "TestTable");
		var index = new Index { IndexName = "IX_Test" };
		index.OrderedColumns.Add("Name");
		table.Indexes.Add(index);

		Assert.AreEqual(1, table.Indexes.Count);
		Assert.AreEqual("IX_Test", table.Indexes[0].IndexName);
	}

	[TestMethod]
	public void Table_PrimaryKeyConstraintName_Test()
	{
		var table = new Table("dbo", "TestTable")
		{
			PrimaryKeyConstraintName = "PK_TestTable"
		};

		Assert.AreEqual("PK_TestTable", table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	public void Table_Properties_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Properties.Add(new ExtendedProperty("TestProp", "TestValue"));

		Assert.AreEqual(1, table.Properties.Count);
		Assert.AreEqual("TestProp", table.Properties[0].Name);
		Assert.AreEqual("TestValue", table.Properties[0].Value);
	}

	[TestMethod]
	public void Table_ReferencesTable_False_Test()
	{
		var table1 = new Table("dbo", "Table1");
		table1.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });

		var table2 = new Table("dbo", "Table2");
		table2.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });

		Assert.IsFalse(table1.ReferencesTable(table2));
		Assert.IsFalse(table2.ReferencesTable(table1));
	}

	[TestMethod]
	public void Table_ReferencesTable_True_Test()
	{
		var parentTable = new Table("dbo", "Parent");
		parentTable.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });

		var childTable = new Table("dbo", "Child");
		childTable.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		childTable.Columns.Add(new Column("ParentId", DbType.Int32)
		{
			ReferencedSchema = "dbo",
			ReferencedTable = "Parent",
			ReferencedColumn = "Id"
		});

		Assert.IsTrue(childTable.ReferencesTable(parentTable));
	}
}
