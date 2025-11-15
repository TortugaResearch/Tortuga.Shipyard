using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class SqlServerViewTests : TestsBase
{
	[TestMethod]
	public void Scenario_1()
	{
		var expected = @"CREATE VIEW Reporting.NameAddresses
AS
SELECT
	na.NameAddressesKey,
	na.AddressCategory,
	na.AddressNumber,
	na.AlternateNameNumber,
	na.AlternateNameType,
	na.NameId,
	na.NotePadId,
	na.SequenceNumber,
	na.WireEnabledIndicator,
	na.IndChgsLastModifiedDateTime,
	na.IndChgsLastModifiedByUserId,
	na.AddressKey,
	na.NameKey,
	na.NotePadKey,
	na.LastImportRunId,
	na.LastImportDateTime,
	n.FirstName,
	n.LastName,
	n.MiddleName,
	n.BirthDate,
	a.AddressLine1,
	a.AddressLine2,
	a.AddressLine3,
	a.AddressLine4,
	a.AddressLine5,
	a.AddressLine6,
	a.AddressSearchField,
	a.AddressType,
	a.City,
	a.Country,
	a.CreditBureauAddressIndicator,
	a.CreditBureauResidenceCode,
	a.EmailAddress,
	a.[State],
	a.TimeZone,
	a.WebAddress,
	a.ZipCode
FROM Storage.NameAddresses na
INNER JOIN Storage.Name n
	ON na.NameKey = n.NameKey
LEFT JOIN Storage.Address a
	ON na.AddressKey = a.AddressKey;

";

		var nameAddressTable = CreateNameAddressesTable();
		var nameTable = CreateNameTable();
		var addressTable = CreateAddressTable();

		var view = nameAddressTable.CreateView("Reporting", "NameAddresses");
		view.Join(JoinType.InnerJoin, nameTable, "NameKey");
		view.Join(JoinType.LeftJoin, addressTable, "AddressKey");

		var generator = new SqlServerGenerator();
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_2()
	{
		var expected = @"CREATE VIEW Reporting.NameAddresses
AS
SELECT
	na.NameAddressesKey,
	na.AddressCategory,
	na.AddressNumber,
	na.AlternateNameNumber,
	na.AlternateNameType,
	na.NameId,
	na.NotePadId,
	na.SequenceNumber,
	na.WireEnabledIndicator,
	na.IndChgsLastModifiedDateTime,
	na.IndChgsLastModifiedByUserId,
	na.AddressKey,
	na.NameKey,
	na.NotePadKey,
	na.LastImportRunId,
	na.LastImportDateTime,
	n.NameKey AS Name_NameKey,
	n.FirstName AS Name_FirstName,
	n.LastName AS Name_LastName,
	n.MiddleName AS Name_MiddleName,
	n.BirthDate AS Name_BirthDate,
	n.NameId AS Name_NameId,
	a.AddressKey AS Address_AddressKey,
	a.AddressLine1 AS Address_AddressLine1,
	a.AddressLine2 AS Address_AddressLine2,
	a.AddressLine3 AS Address_AddressLine3,
	a.AddressLine4 AS Address_AddressLine4,
	a.AddressLine5 AS Address_AddressLine5,
	a.AddressLine6 AS Address_AddressLine6,
	a.AddressNumber AS Address_AddressNumber,
	a.AddressSearchField AS Address_AddressSearchField,
	a.AddressType AS Address_AddressType,
	a.City AS Address_City,
	a.Country AS Address_Country,
	a.CreditBureauAddressIndicator AS Address_CreditBureauAddressIndicator,
	a.CreditBureauResidenceCode AS Address_CreditBureauResidenceCode,
	a.EmailAddress AS Address_EmailAddress,
	a.[State] AS Address_State,
	a.TimeZone AS Address_TimeZone,
	a.WebAddress AS Address_WebAddress,
	a.ZipCode AS Address_ZipCode,
	a.IndChgsLastModifiedDateTime AS Address_IndChgsLastModifiedDateTime,
	a.IndChgsLastModifiedByUserId AS Address_IndChgsLastModifiedByUserId,
	a.LastImportRunId AS Address_LastImportRunId,
	a.LastImportDateTime AS Address_LastImportDateTime,
	a.NameId AS Address_NameId
FROM Storage.NameAddresses na
INNER JOIN Storage.Name n
	ON na.NameKey = n.NameKey
LEFT JOIN Storage.Address a
	ON na.AddressKey = a.AddressKey;

";

		var nameAddressTable = CreateNameAddressesTable();
		var nameTable = CreateNameTable();
		var addressTable = CreateAddressTable();

		var view = nameAddressTable.CreateView("Reporting", "NameAddresses");
		view.Join(JoinType.InnerJoin, nameTable, "NameKey", new JoinRules { PrefixColumnAlias = nameTable.TableName + "_" });
		view.Join(JoinType.LeftJoin, addressTable, "AddressKey", new JoinRules { PrefixColumnAlias = addressTable.TableName + "_" });

		var generator = new SqlServerGenerator();
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_3()
	{
		var expected = @"CREATE VIEW Reporting.NamesWithAddresses
AS
SELECT
	n.NameKey,
	n.FirstName,
	n.LastName,
	n.MiddleName,
	n.BirthDate,
	n.NameId,
	na.NameAddressesKey,
	na.AddressCategory,
	na.AddressNumber,
	na.AlternateNameNumber,
	na.AlternateNameType,
	na.NotePadId,
	na.SequenceNumber,
	na.WireEnabledIndicator,
	na.IndChgsLastModifiedDateTime,
	na.IndChgsLastModifiedByUserId,
	na.AddressKey,
	na.NotePadKey,
	na.LastImportRunId,
	na.LastImportDateTime,
	a.AddressLine1,
	a.AddressLine2,
	a.AddressLine3,
	a.AddressLine4,
	a.AddressLine5,
	a.AddressLine6,
	a.AddressSearchField,
	a.AddressType,
	a.City,
	a.Country,
	a.CreditBureauAddressIndicator,
	a.CreditBureauResidenceCode,
	a.EmailAddress,
	a.[State],
	a.TimeZone,
	a.WebAddress,
	a.ZipCode
FROM Storage.Name n
LEFT JOIN Storage.NameAddresses na
	ON n.NameId = na.NameId
LEFT JOIN Storage.Address a
	ON n.NameId = a.NameId;

";

		var nameAddressTable = CreateNameAddressesTable();
		var nameTable = CreateNameTable();
		var addressTable = CreateAddressTable();

		var view = nameTable.CreateView("Reporting", "NamesWithAddresses");
		view.Join(JoinType.LeftJoin, nameAddressTable, "NameId");
		view.Join(JoinType.LeftJoin, addressTable, "NameId");

		var generator = new SqlServerGenerator();
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_4()
	{
		var expected = @"CREATE VIEW Reporting.NamesWithAddresses
AS
SELECT
	n.NameKey,
	n.FirstName,
	n.LastName,
	n.MiddleName,
	n.BirthDate,
	n.NameId,
	na.NameAddressesKey,
	na.AddressCategory,
	na.AddressNumber,
	na.AlternateNameNumber,
	na.AlternateNameType,
	na.NotePadId,
	na.SequenceNumber,
	na.WireEnabledIndicator,
	na.IndChgsLastModifiedDateTime,
	na.IndChgsLastModifiedByUserId,
	na.AddressKey,
	na.NotePadKey,
	na.LastImportRunId,
	na.LastImportDateTime,
	a.AddressLine1,
	a.AddressLine2,
	a.AddressLine3,
	a.AddressLine4,
	a.AddressLine5,
	a.AddressLine6,
	a.AddressSearchField,
	a.AddressType,
	a.City,
	a.Country,
	a.CreditBureauAddressIndicator,
	a.CreditBureauResidenceCode,
	a.EmailAddress,
	a.[State],
	a.TimeZone,
	a.WebAddress,
	a.ZipCode
FROM Storage.Name n
LEFT JOIN Storage.NameAddresses na
	ON n.NameId = na.NameId
LEFT JOIN Storage.Address a
	ON na.AddressKey = a.AddressKey;

";

		var nameAddressTable = CreateNameAddressesTable();
		var nameTable = CreateNameTable();
		var addressTable = CreateAddressTable();

		var view = nameTable.CreateView("Reporting", "NamesWithAddresses");
		view.Join(JoinType.LeftJoin, nameAddressTable, "NameId");
		view.Join(JoinType.LeftJoin, addressTable, "AddressKey");

		var generator = new SqlServerGenerator();
		generator.CalculateAliases(view);
		generator.CalculateJoinExpressions(view);
		var output = generator.BuildView(view);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	private static Table CreateAddressTable()
	{
		var result = new Table("Storage", "Address");
		result.Columns.Add(new("AddressKey", DbType.Int32)
		{ IsPrimaryKey = true, Description = "Generated primary key based on (AddressNumber)" });
		result.Columns.Add(new("AddressLine1", DbType.String, 50, true));
		result.Columns.Add(new("AddressLine2", DbType.String, 50, true));
		result.Columns.Add(new("AddressLine3", DbType.String, 50, true));
		result.Columns.Add(new("AddressLine4", DbType.String, 50, true));
		result.Columns.Add(new("AddressLine5", DbType.String, 50, true));
		result.Columns.Add(new("AddressLine6", DbType.String, 50, true));
		result.Columns.Add(new("AddressNumber", DbType.Int32, false) { IsUnique = true, UniqueConstraintName = "PK2_Address" });
		result.Columns.Add(new("AddressSearchField", DbType.String, 50, true));
		result.Columns.Add(new("AddressType", DbType.Int16, true));
		result.Columns.Add(new("City", DbType.String, 50, true));
		result.Columns.Add(new("Country", DbType.Int32, true));
		result.Columns.Add(new("CreditBureauAddressIndicator", DbType.String, 2, true));
		result.Columns.Add(new("CreditBureauResidenceCode", DbType.String, 1, true));
		result.Columns.Add(new("EmailAddress", DbType.String, 40, true));
		result.Columns.Add(new("State", DbType.Int32, true));
		result.Columns.Add(new("TimeZone", DbType.Int16, true));
		result.Columns.Add(new("WebAddress", DbType.String, 40, true));
		result.Columns.Add(new("ZipCode", DbType.String, 12, true));
		result.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		result.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		result.Columns.Add(new("LastImportRunId", DbType.Guid, false));
		result.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7, false));
		result.Columns.Add(new("NameId", DbType.Int32));
		return result;
	}

	private static Table CreateNameAddressesTable()
	{
		var result = new Table("Storage", "NameAddresses");

		result.Columns.Add(new("NameAddressesKey", DbType.Int32) { IsPrimaryKey = true, IsIdentity = true });
		result.Columns.Add(new("AddressCategory", DbType.Int16, true));
		result.Columns.Add(new("AddressNumber", DbType.Int32, true));
		result.Columns.Add(new("AlternateNameNumber", DbType.Int32, true));
		result.Columns.Add(new("AlternateNameType", DbType.Int16, true));
		result.Columns.Add(new("NameId", DbType.Int32));
		result.Columns.Add(new("NotePadId", DbType.Int32, true));
		result.Columns.Add(new("SequenceNumber", DbType.Int16));
		result.Columns.Add(new("WireEnabledIndicator", DbType.Int16, true));
		result.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		result.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		result.Columns.Add(new("AddressKey", DbType.Int32, true) { ReferencedTable = "Address", ReferencedColumn = "AddressKey" });
		result.Columns.Add(new("NameKey", DbType.Int32) { ReferencedTable = "Name", ReferencedColumn = "NameKey" });
		result.Columns.Add(new("NotePadKey", DbType.Int32) { ReferencedTable = "NotePad", ReferencedColumn = "NotePadKey" });
		result.Columns.Add(new("LastImportRunId", DbType.Guid));
		result.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7));
		return result;
	}

	private static Table CreateNameTable()
	{
		var result = new Table("Storage", "Name");
		result.Columns.Add(new("NameKey", DbType.Int32)
		{ IsPrimaryKey = true, Description = "Generated primary key based on (AddressNumber)" });
		result.Columns.Add(new("FirstName", DbType.String, 30, true));
		result.Columns.Add(new("LastName", DbType.String, 30, false));
		result.Columns.Add(new("MiddleName", DbType.String, 30, true));
		result.Columns.Add(new("BirthDate", DbType.DateTime2, 7, true));
		result.Columns.Add(new("NameId", DbType.Int32));
		return result;
	}
}
