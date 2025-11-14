using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class OutputColumnTests
{
	[TestMethod]
	public void OutputColumnBase_CanInstantiate_Test()
	{
		var outputColumn = new OutputColumnBase();

		Assert.IsNotNull(outputColumn);
	}

	[TestMethod]
	public void ViewColumn_Constructor_WithBothParameters_Test()
	{
		var column = new ViewColumn("EmployeeId", "EmpId");

		Assert.AreEqual("EmployeeId", column.ColumnName);
		Assert.AreEqual("EmpId", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_Constructor_WithColumnNameOnly_Test()
	{
		var column = new ViewColumn("EmployeeId");

		Assert.AreEqual("EmployeeId", column.ColumnName);
		Assert.IsNull(column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_SetColumnName_Test()
	{
		var column = new ViewColumn("OriginalName");
		column.ColumnName = "NewName";

		Assert.AreEqual("NewName", column.ColumnName);
	}

	[TestMethod]
	public void ViewColumn_SetOutputColumnName_Test()
	{
		var column = new ViewColumn("EmployeeId");
		column.OutputColumnName = "EmpId";

		Assert.AreEqual("EmpId", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_ClearOutputColumnName_Test()
	{
		var column = new ViewColumn("EmployeeId", "EmpId");
		column.OutputColumnName = null;

		Assert.IsNull(column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_EmptyColumnName_Test()
	{
		var column = new ViewColumn("");

		Assert.AreEqual("", column.ColumnName);
	}

	[TestMethod]
	public void ViewColumn_EmptyOutputColumnName_Test()
	{
		var column = new ViewColumn("EmployeeId", "");

		Assert.AreEqual("", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_SpecialCharactersInColumnName_Test()
	{
		var column = new ViewColumn("Employee_Id");

		Assert.AreEqual("Employee_Id", column.ColumnName);
	}

	[TestMethod]
	public void ViewColumn_LongColumnName_Test()
	{
		var longName = new string('A', 128);
		var column = new ViewColumn(longName);

		Assert.AreEqual(longName, column.ColumnName);
	}

	[TestMethod]
	public void ViewColumn_MultiplePropertyChanges_Test()
	{
		var column = new ViewColumn("Original");
		column.ColumnName = "Changed1";
		column.OutputColumnName = "Output1";
		column.ColumnName = "Changed2";
		column.OutputColumnName = "Output2";

		Assert.AreEqual("Changed2", column.ColumnName);
		Assert.AreEqual("Output2", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_Constructor_Test()
	{
		var column = new ExpressionColumn("FirstName + ' ' + LastName", "FullName");

		Assert.AreEqual("FirstName + ' ' + LastName", column.Expression);
		Assert.AreEqual("FullName", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_SetExpression_Test()
	{
		var column = new ExpressionColumn("Original", "Output");
		column.Expression = "UPPER(FirstName)";

		Assert.AreEqual("UPPER(FirstName)", column.Expression);
	}

	[TestMethod]
	public void ExpressionColumn_SetOutputColumnName_Test()
	{
		var column = new ExpressionColumn("Expression", "Original");
		column.OutputColumnName = "NewOutput";

		Assert.AreEqual("NewOutput", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_ClearOutputColumnName_Test()
	{
		var column = new ExpressionColumn("Expression", "Output");
		column.OutputColumnName = null;

		Assert.IsNull(column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_ComplexExpression_Test()
	{
		var expression = "CASE WHEN Status = 'A' THEN 'Active' WHEN Status = 'I' THEN 'Inactive' ELSE 'Unknown' END";
		var column = new ExpressionColumn(expression, "StatusDescription");

		Assert.AreEqual(expression, column.Expression);
		Assert.AreEqual("StatusDescription", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_ExpressionWithPlaceholder_Test()
	{
		var column = new ExpressionColumn("{0}.FirstName + ' ' + {0}.LastName", "FullName");

		Assert.AreEqual("{0}.FirstName + ' ' + {0}.LastName", column.Expression);
	}

	[TestMethod]
	public void ExpressionColumn_EmptyExpression_Test()
	{
		var column = new ExpressionColumn("", "Output");

		Assert.AreEqual("", column.Expression);
	}

	[TestMethod]
	public void ExpressionColumn_EmptyOutputColumnName_Test()
	{
		var column = new ExpressionColumn("Expression", "");

		Assert.AreEqual("", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_MultipleChanges_Test()
	{
		var column = new ExpressionColumn("Initial", "Output1");
		column.Expression = "Modified1";
		column.OutputColumnName = "Output2";
		column.Expression = "Modified2";
		column.OutputColumnName = "Output3";

		Assert.AreEqual("Modified2", column.Expression);
		Assert.AreEqual("Output3", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_FunctionCallExpression_Test()
	{
		var column = new ExpressionColumn("GETDATE()", "CurrentDateTime");

		Assert.AreEqual("GETDATE()", column.Expression);
		Assert.AreEqual("CurrentDateTime", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_MathematicalExpression_Test()
	{
		var column = new ExpressionColumn("Price * Quantity * (1 - Discount)", "TotalAmount");

		Assert.AreEqual("Price * Quantity * (1 - Discount)", column.Expression);
		Assert.AreEqual("TotalAmount", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_IsInstanceOfOutputColumnBase_Test()
	{
		var column = new ViewColumn("TestColumn");

		Assert.IsInstanceOfType(column, typeof(OutputColumnBase));
	}

	[TestMethod]
	public void ExpressionColumn_IsInstanceOfOutputColumnBase_Test()
	{
		var column = new ExpressionColumn("TestExpression", "Output");

		Assert.IsInstanceOfType(column, typeof(OutputColumnBase));
	}

	[TestMethod]
	public void ViewColumn_WithQualifiedName_Test()
	{
		var column = new ViewColumn("dbo.Employee.EmployeeId", "EmpId");

		Assert.AreEqual("dbo.Employee.EmployeeId", column.ColumnName);
		Assert.AreEqual("EmpId", column.OutputColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_WithSubquery_Test()
	{
		var expression = "(SELECT COUNT(*) FROM Orders WHERE EmployeeId = {0}.EmployeeId)";
		var column = new ExpressionColumn(expression, "OrderCount");

		Assert.AreEqual(expression, column.Expression);
		Assert.AreEqual("OrderCount", column.OutputColumnName);
	}

	[TestMethod]
	public void ViewColumn_NullColumnName_CanSet_Test()
	{
		var column = new ViewColumn("Initial");
		// While this may violate validation rules, test that the property accepts it
		column.ColumnName = null!;

		Assert.IsNull(column.ColumnName);
	}

	[TestMethod]
	public void ExpressionColumn_NullExpression_CanSet_Test()
	{
		var column = new ExpressionColumn("Initial", "Output");
		// While this may violate validation rules, test that the property accepts it
		column.Expression = null!;

		Assert.IsNull(column.Expression);
	}
}
