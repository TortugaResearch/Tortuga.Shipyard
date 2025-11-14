using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class AdditionalCoverageTests : TestsBase
{
	[TestMethod]
	public void Column_AllDefaults_Together_Test()
	{
		var column1 = new Column("Col1", DbType.String, 10) { Default = "'Test'", DefaultLocalTime = true, DefaultUtcTime = true };

		Assert.IsNotNull(column1.Default);
		Assert.IsTrue(column1.DefaultLocalTime);
		Assert.IsTrue(column1.DefaultUtcTime);
	}

	[TestMethod]
	public void Column_DefaultConstraintName_WithDefault_Test()
	{
		var column = new Column("Status", DbType.String, 10) { Default = "'Active'" };

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(column);

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);

		Assert.IsNotNull(column.DefaultConstraintName);
		Assert.AreEqual("D_TestTable_Status", column.DefaultConstraintName);
	}

	[TestMethod]
	public void Column_DefaultConstraintName_WithDefaultLocalTime_Test()
	{
		var column = new Column("CreatedDate", DbType.DateTime2, 7);
		column.DefaultLocalTime = true;

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(column);

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);

		Assert.IsNotNull(column.DefaultConstraintName);
		Assert.AreEqual("D_TestTable_CreatedDate", column.DefaultConstraintName);
	}

	[TestMethod]
	public void Column_DefaultConstraintName_WithDefaultUtcTime_Test()
	{
		var column = new Column("CreatedDateUtc", DbType.DateTime2, 7) { DefaultUtcTime = true };

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(column);

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);

		Assert.IsNotNull(column.DefaultConstraintName);
		Assert.AreEqual("D_TestTable_CreatedDateUtc", column.DefaultConstraintName);
	}

	[TestMethod]
	public void Column_DefaultConstraintName_WithoutDefault_Test()
	{
		var column = new Column("Name", DbType.String, 100);

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(column);

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);

		Assert.IsNull(column.DefaultConstraintName);
	}

	[TestMethod]
	public void Generator_CalculateJoinExpressions_MissingColumn_Test()
	{
		var employeeTable = new Table("dbo", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));

		var view = employeeTable.CreateView("dbo", "TestView");
		view.Sources[0].Alias = "e";

		var joinSource = new JoinedViewSource("dbo", "Department", JoinType.InnerJoin, ["NonExistentColumn"], ["DepartmentId"]) { Alias = "d" };
		view.Sources.Add(joinSource);

		var generator = new SqlServerGenerator();

		try
		{
			generator.CalculateJoinExpressions(view);
			Assert.Fail("Expected InvalidOperationException");
		}
		catch (InvalidOperationException ex)
		{
			Assert.IsTrue(ex.Message.Contains("NonExistentColumn"));
		}
	}

	[TestMethod]
	public void JoinedViewSource_Properties_Test()
	{
		var leftCols = new List<string> { "Col1", "Col2" };
		var rightCols = new List<string> { "Col3", "Col4" };

		var joinSource = new JoinedViewSource("dbo", "Table1", JoinType.LeftJoin, leftCols, rightCols);

		Assert.AreEqual("dbo", joinSource.SchemaName);
		Assert.AreEqual("Table1", joinSource.TableOrViewName);
		Assert.AreEqual(JoinType.LeftJoin, joinSource.JoinType);
		Assert.AreEqual(2, joinSource.LeftJoinColumns.Count);
		Assert.AreEqual(2, joinSource.RightJoinColumns.Count);
		Assert.AreEqual("Col1", joinSource.LeftJoinColumns[0]);
		Assert.AreEqual("Col4", joinSource.RightJoinColumns[1]);
	}

	[TestMethod]
	public void PostgreSql_WithSchemaInForeignKey_Test()
	{
		var expected = @"CREATE TABLE dbo.orders
(
	order_id integer NOT NULL,
	customer_id integer NOT NULL,
	CONSTRAINT orders_pkey PRIMARY KEY (order_id),
	CONSTRAINT orders_customer_id_fkey FOREIGN KEY (customer_id) REFERENCES sales.customers(customer_id)
);

";

		var table = new Table("dbo", "Orders");
		table.Columns.Add(new Column("OrderId", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new Column("CustomerId", DbType.Int32));
		table.Columns[1].ReferencedSchema = "sales";
		table.Columns[1].ReferencedTable = "Customers";
		table.Columns[1].ReferencedColumn = "CustomerId";

		var generator = new PostgreSqlGenerator() { UseSnakeCase = true };
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void SqlServer_CrossJoin_Test()
	{
		var expected = @"CREATE VIEW dbo.ProductCombinations
AS
SELECT
	p1.ProductId,
	p1.ProductName,
	p2.ProductId AS Product2Id,
	p2.ProductName AS Product2Name
FROM p1
CROSS JOIN p2;

";

		var product1Table = new Table("dbo", "Product");
		product1Table.Columns.Add(new Column("ProductId", DbType.Int32));
		product1Table.Columns.Add(new Column("ProductName", DbType.String, 100));

		var product2Table = new Table("dbo", "Product");
		product2Table.Columns.Add(new Column("ProductId", DbType.Int32));
		product2Table.Columns.Add(new Column("ProductName", DbType.String, 100));

		var view = product1Table.CreateView("dbo", "ProductCombinations");
		view.Sources[0].Alias = "p1";

		var crossJoinSource = new JoinedViewSource("dbo", "Product", JoinType.CrossJoin, [], []) { Alias = "p2" };
		crossJoinSource.AddColumn("ProductId", "Product2Id");
		crossJoinSource.AddColumn("ProductName", "Product2Name");
		view.Sources.Add(crossJoinSource);

		var generator = new SqlServerGenerator();
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void SqlServer_FullJoin_Test()
	{
		var expected = @"CREATE VIEW dbo.AllData
AS
SELECT
	t1.Id,
	t1.Data1,
	t2.Id AS Id2,
	t2.Data2
FROM t1
FULL OUTER JOIN t2
	ON t1.Id = t2.Id;

";

		var table1 = new Table("dbo", "Table1");
		table1.Columns.Add(new Column("Id", DbType.Int32));
		table1.Columns.Add(new Column("Data1", DbType.String, 100));

		var table2 = new Table("dbo", "Table2");
		table2.Columns.Add(new Column("Id", DbType.Int32));
		table2.Columns.Add(new Column("Data2", DbType.String, 100));

		var view = table1.CreateView("dbo", "AllData");
		view.Sources[0].Alias = "t1";

		var fullJoinSource = new JoinedViewSource("dbo", "Table2", JoinType.FullJoin, ["Id"], ["Id"]) { Alias = "t2" };
		fullJoinSource.AddColumn("Id", "Id2");
		fullJoinSource.AddColumn("Data2");
		view.Sources.Add(fullJoinSource);

		var generator = new SqlServerGenerator();
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void SqlServer_NoBatchSeparator_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = false;
		var output = generator.BuildTable(table);

		Assert.IsFalse(output.Contains("GO"));
	}

	[TestMethod]
	public void SqlServer_RightJoin_Test()
	{
		var expected = @"CREATE VIEW dbo.EmployeeView
AS
SELECT
	e.EmployeeId,
	e.FirstName,
	e.DepartmentId,
	d.DepartmentName
FROM e
RIGHT JOIN d
	ON e.DepartmentId = d.DepartmentId;

";

		var employeeTable = new Table("dbo", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("FirstName", DbType.String, 50));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var departmentTable = new Table("dbo", "Department");
		departmentTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		departmentTable.Columns.Add(new Column("DepartmentName", DbType.String, 100));

		var view = employeeTable.CreateView("dbo", "EmployeeView");
		view.Sources[0].Alias = "e";

		var rightJoinSource = new JoinedViewSource("dbo", "Department", JoinType.RightJoin, ["DepartmentId"], ["DepartmentId"]) { Alias = "d" };
		rightJoinSource.AddColumn("DepartmentName");
		view.Sources.Add(rightJoinSource);

		var generator = new SqlServerGenerator();
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void SqlServer_Table_WithTableProperties_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY
);
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Main test table', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TestTable', @level2type = NULL, @level2name = NULL;
GO

EXEC sp_addextendedproperty @name = N'Version', @value = N'1.0', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TestTable', @level2type = NULL, @level2name = NULL;
GO

";

		var table = new Table("dbo", "TestTable");
		table.Description = "Main test table";
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Properties.Add(new ExtendedProperty("Version", "1.0"));

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void ViewSource_SchemaName_Property_Test()
	{
		var source = new ViewSource("Schema1", "Table1");
		Assert.AreEqual("Schema1", source.SchemaName);

		source.SchemaName = "Schema2";
		Assert.AreEqual("Schema2", source.SchemaName);
	}

	[TestMethod]
	public void ViewSource_TableOrViewName_Property_Test()
	{
		var source = new ViewSource("dbo", "Table1");
		Assert.AreEqual("Table1", source.TableOrViewName);

		source.TableOrViewName = "Table2";
		Assert.AreEqual("Table2", source.TableOrViewName);
	}
}
