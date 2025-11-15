using System.Data;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class GeneratorTests
{
	[TestMethod]
	public void Generator_BuildTables_Multiple_Test()
	{
		var generator = new TestGenerator();
		var table1 = new Table("dbo", "Table1");
		var table2 = new Table("dbo", "Table2");
		var tables = new[] { table1, table2 };

		var results = generator.BuildTables(tables).ToList();

		Assert.AreEqual(2, results.Count);
		Assert.AreEqual("TEST TABLE", results[0]);
		Assert.AreEqual("TEST TABLE", results[1]);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildTables_Null_Test()
	{
		var generator = new TestGenerator();
		var result = generator.BuildTables(null!).ToList();
	}

	[TestMethod]
	public void Generator_BuildViews_Multiple_Test()
	{
		var generator = new TestGenerator();
		var view1 = new View("dbo", "View1");
		view1.Sources.Add(new ViewSource("dbo", "Table1"));
		var view2 = new View("dbo", "View2");
		view2.Sources.Add(new ViewSource("dbo", "Table1"));
		var views = new[] { view1, view2 };

		var results = generator.BuildViews(views).ToList();

		Assert.AreEqual(2, results.Count);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildViews_Null_Test()
	{
		var generator = new TestGenerator();
		var result = generator.BuildViews(null!).ToList();
	}

	[TestMethod]
	public void Generator_CalculateAliases_DuplicateNames_Test()
	{
		var generator = new TestGenerator();
		var view = new View("dbo", "TestView");
		var source1 = new ViewSource("dbo", "Employee");
		var source2 = new ViewSource("dbo", "Employee");
		var source3 = new ViewSource("dbo", "Employee");
		view.Sources.Add(source1);
		view.Sources.Add(source2);
		view.Sources.Add(source3);

		generator.CalculateAliases(view);

		Assert.AreEqual("e", source1.Alias);
		Assert.AreEqual("e1", source2.Alias);
		Assert.AreEqual("e2", source3.Alias);
	}

	[TestMethod]
	public void Generator_CalculateAliases_MultipleSources_Test()
	{
		var generator = new TestGenerator();
		var view = new View("dbo", "TestView");
		var source1 = new ViewSource("dbo", "Employee");
		var source2 = new ViewSource("dbo", "Department");
		view.Sources.Add(source1);
		view.Sources.Add(source2);

		generator.CalculateAliases(view);

		Assert.AreEqual("e", source1.Alias);
		Assert.AreEqual("d", source2.Alias);
	}

	[TestMethod]
	public void Generator_CalculateAliases_NoUppercase_Test()
	{
		var generator = new TestGenerator();
		var view = new View("dbo", "TestView");
		var source = new ViewSource("dbo", "employee");
		view.Sources.Add(source);

		generator.CalculateAliases(view);

		Assert.IsNotNull(source.Alias);
		Assert.AreEqual("e", source.Alias);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_CalculateAliases_NullView_Test()
	{
		var generator = new TestGenerator();
		generator.CalculateAliases((View)null!);
	}

	[TestMethod]
	public void Generator_CalculateAliases_PreExistingAlias_Test()
	{
		var generator = new TestGenerator();
		var view = new View("dbo", "TestView");
		var source = new ViewSource("dbo", "Employee") { Alias = "emp" };
		view.Sources.Add(source);

		generator.CalculateAliases(view);

		Assert.AreEqual("emp", source.Alias);
	}

	[TestMethod]
	public void Generator_CalculateAliases_SingleSource_Test()
	{
		var generator = new TestGenerator();
		var view = new View("dbo", "TestView");
		var source = new ViewSource("dbo", "Employee");
		view.Sources.Add(source);

		generator.CalculateAliases(view);

		Assert.IsNotNull(source.Alias);
		Assert.AreEqual("e", source.Alias);
	}

	[TestMethod]
	public void Generator_CalculateJoinExpressions_MultipleColumns_Test()
	{
		var generator = new TestGenerator();

		var employeeTable = new Table("dbo", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));
		employeeTable.Columns.Add(new Column("LocationId", DbType.Int32));

		var view = employeeTable.CreateView("dbo", "EmployeeView");
		var source = view.Sources[0];
		source.Alias = "e";

		var assignmentJoin = new JoinedViewSource("dbo", "Assignment", JoinType.InnerJoin,
			new List<string> { "DepartmentId", "LocationId" },
			new List<string> { "DepartmentId", "LocationId" })
		{
			Alias = "a"
		};
		assignmentJoin.AddColumn("AssignmentDate");
		view.Sources.Add(assignmentJoin);

		generator.CalculateJoinExpressions(view);

		Assert.IsNotNull(assignmentJoin.JoinExpression);
		Assert.IsTrue(assignmentJoin.JoinExpression.Contains("AND"));
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_CalculateJoinExpressions_NullView_Test()
	{
		var generator = new TestGenerator();
		generator.CalculateJoinExpressions((View)null!);
	}

	[TestMethod]
	public void Generator_CalculateJoinExpressions_SimpleJoin_Test()
	{
		var generator = new TestGenerator();

		var employeeTable = new Table("dbo", "Employee");
		employeeTable.Columns.Add(new Column("EmployeeId", DbType.Int32));
		employeeTable.Columns.Add(new Column("DepartmentId", DbType.Int32));

		var view = employeeTable.CreateView("dbo", "EmployeeView");
		var source = view.Sources[0];
		source.Alias = "e";

		var departmentJoin = new JoinedViewSource("dbo", "Department", JoinType.InnerJoin,
			new List<string> { "DepartmentId" },
			new List<string> { "DepartmentId" })
		{
			Alias = "d"
		};
		departmentJoin.AddColumn("DepartmentName");
		view.Sources.Add(departmentJoin);

		generator.CalculateJoinExpressions(view);

		Assert.IsNotNull(departmentJoin.JoinExpression);
		Assert.AreEqual("e.[DepartmentId] = d.[DepartmentId]", departmentJoin.JoinExpression);
	}

	[TestMethod]
	public void Generator_EscapeAllIdentifiers_Property_Test()
	{
		var generator = new TestGenerator();

		Assert.IsFalse(generator.EscapeAllIdentifiers);

		generator.EscapeAllIdentifiers = true;
		Assert.IsTrue(generator.EscapeAllIdentifiers);
	}

	[TestMethod]
	public void Generator_EscapeText_MultipleQuotes_Test()
	{
		var generator = new TestGenerator();
		var result = generator.EscapeText("It's a test's value");
		Assert.AreEqual("'It''s a test''s value'", result);
	}

	[TestMethod]
	public void Generator_EscapeText_Null_Test()
	{
		var generator = new TestGenerator();
		var result = generator.EscapeText(null);
		Assert.IsNull(result);
	}

	[TestMethod]
	public void Generator_EscapeText_Simple_Test()
	{
		var generator = new TestGenerator();
		var result = generator.EscapeText("test");
		Assert.AreEqual("'test'", result);
	}

	[TestMethod]
	public void Generator_EscapeText_WithQuotes_Test()
	{
		var generator = new TestGenerator();
		var result = generator.EscapeText("O'Brien");
		Assert.AreEqual("'O''Brien'", result);
	}

	[TestMethod]
	public void Generator_EscapeTextUnicode_Test()
	{
		var generator = new TestGenerator();
		var result = generator.EscapeTextUnicode("test");
		Assert.AreEqual("'test'", result);
	}

	[TestMethod]
	public void Generator_Keywords_Test()
	{
		var generator = new TestGenerator();

		Assert.IsNotNull(generator.Keywords);
		Assert.AreEqual(0, generator.Keywords.Count);

		generator.Keywords.Add("SELECT");
		generator.Keywords.Add("FROM");

		Assert.AreEqual(2, generator.Keywords.Count);
		Assert.IsTrue(generator.Keywords.Contains("SELECT"));
		Assert.IsTrue(generator.Keywords.Contains("select")); // Case-insensitive
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_NameConstraints_Null_Test()
	{
		var generator = new TestGenerator();
		generator.NameConstraints((IEnumerable<Table>)null!);
	}

	[TestMethod]
	public void Generator_TabSize_Property_Test()
	{
		var generator = new TestGenerator();

		Assert.IsNull(generator.TabSize);

		generator.TabSize = 4;
		Assert.AreEqual(4, generator.TabSize);
	}

	[TestMethod]
	public void Generator_UseSnakeCase_Property_Test()
	{
		var generator = new TestGenerator();

		Assert.IsFalse(generator.UseSnakeCase);

		generator.UseSnakeCase = true;
		Assert.IsTrue(generator.UseSnakeCase);
	}

	[TestMethod]
	public void Generator_Validate_InvalidTable_NoColumns_Test()
	{
		var generator = new TestGenerator();
		var table = new Table("dbo", "TestTable");

		var results = generator.Validate(table);

		Assert.IsNotNull(results);
		Assert.IsTrue(results.Count > 0);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_Validate_Null_Test()
	{
		var generator = new TestGenerator();
		generator.Validate(null!);
	}

	[TestMethod]
	public void Generator_Validate_ValidTable_Test()
	{
		var generator = new TestGenerator();
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new Column("Id", DbType.Int32) { IsPrimaryKey = true });

		var results = generator.Validate(table);

		Assert.IsNotNull(results);
		Assert.AreEqual(0, results.Count);
	}

	private class TestGenerator : Generator
	{
		public override string BuildTable(Table table) => "TEST TABLE";

		//public override string BuildView(View view) => "TEST VIEW";
		public override string? EscapeIdentifier(string? identifier) => identifier == null ? null : $"[{identifier}]";

		public override void NameConstraints(Table table)
		{
		}
	}
}
