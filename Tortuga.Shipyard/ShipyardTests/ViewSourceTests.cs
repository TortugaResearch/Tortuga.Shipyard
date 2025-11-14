using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ViewSourceTests
{
	[TestMethod]
	public void ExpressionColumn_Constructor_Test()
	{
		var column = new ExpressionColumn("FirstName + ' ' + LastName", "FullName");

		Assert.AreEqual("FirstName + ' ' + LastName", column.Expression);
		Assert.AreEqual("FullName", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_PropertyChanges_Test()
	{
		var column = new ExpressionColumn("FirstName", "FName");
		column.Expression = "UPPER(FirstName)";
		column.OutputColumnName = "UpperFirstName";

		Assert.AreEqual("UPPER(FirstName)", column.Expression);
		Assert.AreEqual("UpperFirstName", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_Constructor_SingleParameter_Test()
	{
		var column = new ViewColumn("EmployeeId");

		Assert.AreEqual("EmployeeId", column.ColumnName);
		Assert.IsNull(column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_Constructor_TwoParameters_Test()
	{
		var column = new ViewColumn("EmployeeId", "EmpId");

		Assert.AreEqual("EmployeeId", column.ColumnName);
		Assert.AreEqual("EmpId", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_PropertyChanges_Test()
	{
		var column = new ViewColumn("EmployeeId");
		column.OutputColumnName = "EmpId";

		Assert.AreEqual("EmployeeId", column.ColumnName);
		Assert.AreEqual("EmpId", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewSource_AddColumn_SingleParameter_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");
		viewSource.AddColumn("EmployeeId");
		viewSource.AddColumn("FirstName");

		Assert.AreEqual(2, viewSource.Outputs.Count);

		var col1 = viewSource.Outputs[0] as ViewColumn;
		Assert.IsNotNull(col1);
		Assert.AreEqual("EmployeeId", col1.ColumnName);
		Assert.IsNull(col1.OutputColumnName);

		var col2 = viewSource.Outputs[1] as ViewColumn;
		Assert.IsNotNull(col2);
		Assert.AreEqual("FirstName", col2.ColumnName);
	}

	[TestMethod]
	public void ViewSource_AddColumn_WithOutputName_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");
		viewSource.AddColumn("EmployeeId", "EmpId");
		viewSource.AddColumn("FirstName", "FName");

		Assert.AreEqual(2, viewSource.Outputs.Count);

		var col1 = viewSource.Outputs[0] as ViewColumn;
		Assert.IsNotNull(col1);
		Assert.AreEqual("EmployeeId", col1.ColumnName);
		Assert.AreEqual("EmpId", col1.OutputColumnName);

		var col2 = viewSource.Outputs[1] as ViewColumn;
		Assert.IsNotNull(col2);
		Assert.AreEqual("FirstName", col2.ColumnName);
		Assert.AreEqual("FName", col2.OutputColumnName);
	}

	[TestMethod]
	public void ViewSource_AddExpression_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");
		viewSource.AddExpression("FirstName + ' ' + LastName", "FullName");

		Assert.AreEqual(1, viewSource.Outputs.Count);

		var expr = viewSource.Outputs[0] as ExpressionColumn;
		Assert.IsNotNull(expr);
		Assert.AreEqual("FirstName + ' ' + LastName", expr.Expression);
		Assert.AreEqual("FullName", expr.OutputColumnName);
	}

	[TestMethod]
	public void ViewSource_Alias_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee")
		{
			Alias = "e"
		};

		Assert.AreEqual("e", viewSource.Alias);
	}

	[TestMethod]
	public void ViewSource_Chaining_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee")
			.AddColumn("EmployeeId")
			.AddColumn("FirstName")
			.AddExpression("FullName", "{0}.FirstName + ' ' + {0}.LastName");

		Assert.AreEqual(3, viewSource.Outputs.Count);
	}

	[TestMethod]
	public void ViewSource_Constructor_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");

		Assert.AreEqual("Storage", viewSource.SchemaName);
		Assert.AreEqual("Employee", viewSource.TableOrViewName);
		Assert.IsNull(viewSource.Alias);
		Assert.AreEqual(0, viewSource.Outputs.Count);
	}

	[TestMethod]
	public void ViewSource_MixedOutputs_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");
		viewSource.AddColumn("EmployeeId");
		viewSource.AddExpression("FullName", "{0}.FirstName + ' ' + {0}.LastName");
		viewSource.AddColumn("Email", "EmailAddress");

		Assert.AreEqual(3, viewSource.Outputs.Count);
		Assert.IsInstanceOfType(viewSource.Outputs[0], typeof(ViewColumn));
		Assert.IsInstanceOfType(viewSource.Outputs[1], typeof(ExpressionColumn));
		Assert.IsInstanceOfType(viewSource.Outputs[2], typeof(ViewColumn));
	}

	[TestMethod]
	public void ViewSource_PropertyChanges_Test()
	{
		var viewSource = new ViewSource("Storage", "Employee");
		viewSource.SchemaName = "HR";
		viewSource.TableOrViewName = "Person";

		Assert.AreEqual("HR", viewSource.SchemaName);
		Assert.AreEqual("Person", viewSource.TableOrViewName);
	}
}
