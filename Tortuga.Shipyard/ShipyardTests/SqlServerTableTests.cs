using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed partial class SqlServerTableTests : TestsBase
{
	[TestMethod]
	public void CheckConstraint_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	Age INT NOT NULL CONSTRAINT CK_Age CHECK (Age >= 0 AND Age <= 120)
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("Age", DbType.Int32)
		{
			Check = "Age >= 0 AND Age <= 120",
			CheckConstraintName = "CK_Age"
		});

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void CompoundPrimaryKey_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Key1 INT NOT NULL,
	Key2 INT NOT NULL,
	[Data] NVARCHAR(50) NULL,
	CONSTRAINT PK_TestTable PRIMARY KEY (Key1, Key2)
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Key1", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("Key2", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("Data", DbType.String, 50, true));

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void DefaultConstraintWithLocalTime_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	CreatedDate DATETIME2(7) NOT NULL CONSTRAINT D_TestTable_CreatedDate DEFAULT (SYSDATETIME())
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("CreatedDate", DbType.DateTime2, 7) { DefaultLocalTime = true });

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void DefaultConstraintWithUtcTime_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	CreatedDateUtc DATETIME2(7) NOT NULL CONSTRAINT D_TestTable_CreatedDateUtc DEFAULT (SYSUTCDATETIME())
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("CreatedDateUtc", DbType.DateTime2, 7) { DefaultUtcTime = true });

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void ForeignKeyWithDifferentSchema_Test()
	{
		var expected = @"CREATE TABLE dbo.[Order]
(
	OrderId INT NOT NULL CONSTRAINT PK_Order PRIMARY KEY,
	CustomerId INT NOT NULL CONSTRAINT FK_Order_CustomerId REFERENCES Sales.Customer(CustomerId)
);

";

		var table = new Table("dbo", "Order");
		table.Columns.Add(new("OrderId", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("CustomerId", DbType.Int32)
		{
			ReferencedSchema = "Sales",
			ReferencedTable = "Customer",
			ReferencedColumn = "CustomerId"
		});

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildTable_Null_Test()
	{
		var generator = new SqlServerGenerator();
		generator.BuildTable(null!);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_BuildView_Null_Test()
	{
		var generator = new SqlServerGenerator();
		generator.BuildView(null!);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Keyword_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeIdentifier("Select");

		Assert.AreEqual("[Select]", result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Normal_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeIdentifier("NormalColumn");

		Assert.AreEqual("NormalColumn", result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_Null_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeIdentifier(null);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_WithNumber_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeIdentifier("123Column");

		Assert.AreEqual("[123Column]", result);
	}

	[TestMethod]
	public void Generator_EscapeIdentifier_WithSpecialChars_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeIdentifier("Column Name");

		Assert.AreEqual("[Column Name]", result);
	}

	[TestMethod]
	public void Generator_EscapeTextUnicode_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeTextUnicode("Test value");

		Assert.AreEqual("N'Test value'", result);
	}

	[TestMethod]
	public void Generator_EscapeTextUnicode_WithQuotes_Test()
	{
		var generator = new SqlServerGenerator();
		var result = generator.EscapeTextUnicode("It's a test");

		Assert.AreEqual("N'It''s a test'", result);
	}

	[TestMethod]
	public void Generator_IncludeSchemaNameInConstraintNames_Test()
	{
		var generator = new SqlServerGenerator();
		generator.IncludeSchemaNameInConstraintNames = true;

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });

		generator.NameConstraints(table);

		Assert.AreEqual("PK_dbo_TestTable", table.PrimaryKeyConstraintName);
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void Generator_NameConstraints_Null_Test()
	{
		var generator = new SqlServerGenerator();
		generator.NameConstraints((Table)null!);
	}

	[TestMethod]
	public void Generator_TabSize_Integration_Test()
	{
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });

		var generator = new SqlServerGenerator();
		generator.TabSize = 4;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Assert.IsFalse(output.Contains("\t"));
		Assert.IsTrue(output.Contains("    ")); // 4 spaces
	}

	[TestMethod]
	public void Generator_UseBatchSeparator_Property_Test()
	{
		var generator = new SqlServerGenerator();

		Assert.IsFalse(generator.UseBatchSeperator);

		generator.UseBatchSeperator = true;
		Assert.IsTrue(generator.UseBatchSeperator);
	}

	[TestMethod]
	public void Generator_Validate_IdentityOnStringColumn_Test()
	{
		var generator = new SqlServerGenerator();
		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.String, 50) { IsIdentity = true, IsPrimaryKey = true });

		var results = generator.Validate(table);

		Assert.IsTrue(results.Any(r => r.MemberNames.Contains("IsIdentity")));
	}

	[TestMethod]
	public void Identify_Type_Fail_Test()
	{
		var table = new Table("dbo", "foo");
		table.Columns.Add(new("Test", DbType.String) { IsIdentity = true });
		var generator = new SqlServerGenerator();
		var results = generator.Validate(table);
		Assert.IsTrue(results.Any(e => e.MemberNames.Any(c => c == "IsIdentity")));
	}

	[TestMethod]
	public void Identify_Type_Test()
	{
		var table = new Table("dbo", "foo");
		table.Columns.Add(new("Test", DbType.Int32) { IsIdentity = true });
		var generator = new SqlServerGenerator();
		var results = generator.Validate(table);
		Assert.IsTrue(results.Count == 0);
	}

	[TestMethod]
	public void IdentityWithSeedAndIncrement_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT IDENTITY(1000, 10) CONSTRAINT PK_TestTable PRIMARY KEY
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32)
		{
			IsPrimaryKey = true,
			IsIdentity = true,
			IdentitySeed = 1000,
			IdentityIncrement = 10
		});

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void IndexWithIncludedColumns_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Email NVARCHAR(100) NULL,
	Phone NVARCHAR(20) NULL
);
GO

CREATE NONCLUSTERED INDEX IX_TestTable_Names ON dbo.TestTable (FirstName, LastName) INCLUDE (Email, Phone);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("FirstName", DbType.String, 50));
		table.Columns.Add(new("LastName", DbType.String, 50));
		table.Columns.Add(new("Email", DbType.String, 100, true));
		table.Columns.Add(new("Phone", DbType.String, 20, true));

		var index = new Index { IndexName = "IX_TestTable_Names" };
		index.OrderedColumns.Add("FirstName");
		index.OrderedColumns.Add("LastName");
		index.IncludedColumns.Add("Email");
		index.IncludedColumns.Add("Phone");
		table.Indexes.Add(index);

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void No_Columns_Test()
	{
		var table = new Table("dbo", "foo");
		var generator = new SqlServerGenerator();
		var results = generator.Validate(table);
		Assert.IsTrue(results.Any(e => e.MemberNames.Any(c => c == "Columns")));
	}

	[TestMethod]
	public void NullableUniqueColumn_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	Email NVARCHAR(100) NULL
);

CREATE UNIQUE INDEX UX_Email ON dbo.TestTable(Email) WHERE Email IS NOT NULL;

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("Email", DbType.String, 100, true)
		{
			IsUnique = true,
			UniqueConstraintName = "UX_Email"
		});

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_1()
	{
		var expected = @"CREATE TABLE Denormalized.Address
(
	AddressKey INT NOT NULL CONSTRAINT PK_Address PRIMARY KEY,
	AddressLine1 NVARCHAR(50) NULL,
	AddressLine2 NVARCHAR(50) NULL,
	AddressLine3 NVARCHAR(50) NULL,
	AddressLine4 NVARCHAR(50) NULL,
	AddressLine5 NVARCHAR(50) NULL,
	AddressLine6 NVARCHAR(50) NULL,
	AddressNumber INT NOT NULL CONSTRAINT PK2_Address UNIQUE,
	AddressSearchField NVARCHAR(50) NULL,
	AddressType SMALLINT NULL,
	City NVARCHAR(50) NULL,
	Country INT NULL,
	CreditBureauAddressIndicator NVARCHAR(2) NULL,
	CreditBureauResidenceCode NVARCHAR(1) NULL,
	EmailAddress NVARCHAR(40) NULL,
	[State] INT NULL,
	TimeZone SMALLINT NULL,
	WebAddress NVARCHAR(40) NULL,
	ZipCode NVARCHAR(12) NULL,
	IndChgsLastModifiedDateTime DATETIME2(6) NULL,
	IndChgsLastModifiedByUserId NVARCHAR(8) NULL,
	LastImportRunId UNIQUEIDENTIFIER NOT NULL,
	LastImportDateTime DATETIME2(7) NOT NULL,
	[Pass/Fail] BIT NOT NULL
);
GO

CREATE UNIQUE INDEX UX_Address_WebAddress ON Denormalized.Address(WebAddress) WHERE WebAddress IS NOT NULL;
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Address', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = NULL, @level2name = NULL;
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'Generated primary key based on (AddressNumber)', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressKey';
GO

EXEC sp_addextendedproperty @name = N'DataLineage', @value = N'Storage.Address.AddressNumber', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressKey';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'The first mailing address line.', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine1';
GO

EXEC sp_addextendedproperty @name = N'DataLineage', @value = N'Storage.Address.AddressLine1', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine1';
GO

EXEC sp_addextendedproperty @name = N'FieldCode', @value = N'ADDR215', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine1';
GO

EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'The second mailing address line.', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine2';
GO

EXEC sp_addextendedproperty @name = N'DataLineage', @value = N'Storage.Address.AddressLine2', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine2';
GO

EXEC sp_addextendedproperty @name = N'FieldCode', @value = N'ADDR218', @level0type = N'SCHEMA', @level0name = N'Denormalized', @level1type = N'TABLE', @level1name = N'Address', @level2type = N'COLUMN', @level2name = N'AddressLine2';
GO

";

		var table = new Table("Denormalized", "Address") { Description = "Address" };

		table.Columns.Add(new("AddressKey", DbType.Int32)
		{ IsPrimaryKey = true, Description = "Generated primary key based on (AddressNumber)" })
			.AddProperty("DataLineage", "Storage.Address.AddressNumber");

		table.Columns.Add(new("AddressLine1", DbType.String, 50, true)
		{ Description = "The first mailing address line." })
			.AddProperty("DataLineage", "Storage.Address.AddressLine1")
			.AddProperty("FieldCode", "ADDR215");

		table.Columns.Add(new("AddressLine2", DbType.String, 50, true)
		{ Description = "The second mailing address line." })
			.AddProperty("DataLineage", "Storage.Address.AddressLine2")
			.AddProperty("FieldCode", "ADDR218");

		table.Columns.Add(new("AddressLine3", DbType.String, 50, true));
		table.Columns.Add(new("AddressLine4", DbType.String, 50, true));
		table.Columns.Add(new("AddressLine5", DbType.String, 50, true));
		table.Columns.Add(new("AddressLine6", DbType.String, 50, true));
		table.Columns.Add(new("AddressNumber", DbType.Int32, false) { IsUnique = true, UniqueConstraintName = "PK2_Address" });
		table.Columns.Add(new("AddressSearchField", DbType.String, 50, true));
		table.Columns.Add(new("AddressType", DbType.Int16, true));
		table.Columns.Add(new("City", DbType.String, 50, true));
		table.Columns.Add(new("Country", DbType.Int32, true));
		table.Columns.Add(new("CreditBureauAddressIndicator", DbType.String, 2, true));
		table.Columns.Add(new("CreditBureauResidenceCode", DbType.String, 1, true));
		table.Columns.Add(new("EmailAddress", DbType.String, 40, true));
		table.Columns.Add(new("State", DbType.Int32, true));
		table.Columns.Add(new("TimeZone", DbType.Int16, true));
		table.Columns.Add(new("WebAddress", DbType.String, 40, true) { IsUnique = true });
		table.Columns.Add(new("ZipCode", DbType.String, 12, true));
		table.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		table.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		table.Columns.Add(new("LastImportRunId", DbType.Guid, false));
		table.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7, false));
		table.Columns.Add(new("Pass/Fail", DbType.Boolean, false));

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}

	[TestMethod]
	public void Scenario_2()
	{
		var expected = @"CREATE TABLE Imports.NameAddresses
(
	ImportKey INT IDENTITY CONSTRAINT PK_Imports_NameAddresses PRIMARY KEY NONCLUSTERED,
	ImportRunId UNIQUEIDENTIFIER NOT NULL,
	ImportDateTime DATETIME2(7) NOT NULL CONSTRAINT D_Imports_NameAddresses_ImportDateTime DEFAULT (SYSDATETIME()),
	NameAddressesKey INT NULL,
	AddressCategory SMALLINT NULL,
	AddressNumber INT NULL,
	AlternateNameNumber INT NULL,
	AlternateNameType SMALLINT NULL,
	NameId INT NULL,
	NotePadId INT NULL,
	SequenceNumber SMALLINT NULL,
	WireEnabledIndicator SMALLINT NULL,
	IndChgsLastModifiedDateTime DATETIME2(6) NULL,
	IndChgsLastModifiedByUserId NVARCHAR(8) NULL,
	AddressKey INT NULL,
	NameKey INT NULL,
	NotePadKey INT NULL
);
GO

CREATE CLUSTERED INDEX CX_Imports_NameAddresses ON Imports.NameAddresses (ImportRunId, ImportKey);
GO

CREATE NONCLUSTERED INDEX PK2_Imports_NameAddresses ON Imports.NameAddresses (ImportRunId, NameId, SequenceNumber);
GO

";

		var table = new Table("Imports", "NameAddresses");

		table.Columns.Add(new("ImportKey", DbType.Int32) { IsPrimaryKey = true, IsIdentity = true });
		table.Columns.Add(new("ImportRunId", DbType.Guid));
		table.Columns.Add(new("ImportDateTime", DbType.DateTime2, 7) { Default = "SYSDATETIME()" });
		table.Columns.Add(new("NameAddressesKey", DbType.Int32, true));
		table.Columns.Add(new("AddressCategory", DbType.Int16, true));
		table.Columns.Add(new("AddressNumber", DbType.Int32, true));
		table.Columns.Add(new("AlternateNameNumber", DbType.Int32, true));
		table.Columns.Add(new("AlternateNameType", DbType.Int16, true));
		table.Columns.Add(new("NameId", DbType.Int32, true));
		table.Columns.Add(new("NotePadId", DbType.Int32, true));
		table.Columns.Add(new("SequenceNumber", DbType.Int16, true));
		table.Columns.Add(new("WireEnabledIndicator", DbType.Int16, true));
		table.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		table.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		table.Columns.Add(new("AddressKey", DbType.Int32, true));
		table.Columns.Add(new("NameKey", DbType.Int32, true));
		table.Columns.Add(new("NotePadKey", DbType.Int32, true));

		table.ClusteredIndex = new Index() { OrderedColumns = { "ImportRunId", "ImportKey" } };
		table.Indexes.Add(new Index() { IndexName = "PK2_Imports_NameAddresses", OrderedColumns = { "ImportRunId", "NameId", "SequenceNumber" } });

		var generator = new SqlServerGenerator();
		generator.UseBatchSeperator = true;
		generator.IncludeSchemaNameInConstraintNames = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		CompareOutput(expected, output);
		Debug.WriteLine(output);
	}

	[TestMethod]
	public void Scenario_3()
	{
		var expected = @"CREATE TABLE Storage.NameAddresses
(
	NameAddressesKey INT IDENTITY CONSTRAINT PK_NameAddresses PRIMARY KEY,
	AddressCategory SMALLINT NULL,
	AddressNumber INT NULL,
	AlternateNameNumber INT NULL,
	AlternateNameType SMALLINT NULL,
	NameId INT NOT NULL,
	NotePadId INT NULL,
	SequenceNumber SMALLINT NOT NULL,
	WireEnabledIndicator SMALLINT NULL,
	IndChgsLastModifiedDateTime DATETIME2(6) NULL,
	IndChgsLastModifiedByUserId NVARCHAR(8) NULL,
	AddressKey INT NULL CONSTRAINT FK_NameAddresses_AddressKey REFERENCES Storage.Address(AddressKey),
	NameKey INT NOT NULL CONSTRAINT FK_NameAddresses_NameKey REFERENCES Storage.Name(NameKey),
	NotePadKey INT NOT NULL CONSTRAINT FK_NameAddresses_NotePadKey REFERENCES Storage.NotePad(NotePadKey),
	LastImportRunId UNIQUEIDENTIFIER NOT NULL,
	LastImportDateTime DATETIME2(7) NOT NULL
);
GO

ALTER TABLE Storage.NameAddresses ADD CONSTRAINT PK2_NameAddresses UNIQUE (NameId, SequenceNumber);
GO

";

		var table = new Table("Storage", "NameAddresses");

		table.Columns.Add(new("NameAddressesKey", DbType.Int32) { IsPrimaryKey = true, IsIdentity = true });
		table.Columns.Add(new("AddressCategory", DbType.Int16, true));
		table.Columns.Add(new("AddressNumber", DbType.Int32, true));
		table.Columns.Add(new("AlternateNameNumber", DbType.Int32, true));
		table.Columns.Add(new("AlternateNameType", DbType.Int16, true));
		table.Columns.Add(new("NameId", DbType.Int32));
		table.Columns.Add(new("NotePadId", DbType.Int32, true));
		table.Columns.Add(new("SequenceNumber", DbType.Int16));
		table.Columns.Add(new("WireEnabledIndicator", DbType.Int16, true));
		table.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		table.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		table.Columns.Add(new("AddressKey", DbType.Int32, true) { ReferencedTable = "Address", ReferencedColumn = "AddressKey" });
		table.Columns.Add(new("NameKey", DbType.Int32) { ReferencedTable = "Name", ReferencedColumn = "NameKey" });
		table.Columns.Add(new("NotePadKey", DbType.Int32) { ReferencedTable = "NotePad", ReferencedColumn = "NotePadKey" });
		table.Columns.Add(new("LastImportRunId", DbType.Guid));
		table.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7));

		table.Indexes.Add(new Index() { IndexName = "PK2_NameAddresses", OrderedColumns = { "NameId", "SequenceNumber" }, IsUnique = true, IsConstraint = true });

		var generator = new SqlServerGenerator();
		generator.UseBatchSeperator = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		CompareOutput(expected, output);
		Debug.WriteLine(output);
	}

	[TestMethod]
	public void Scenario_4()
	{
		var expected = @"CREATE TABLE [Data].Address
(
	AddressKey INT IDENTITY CONSTRAINT PK_Address PRIMARY KEY,
	AddressLine1 NVARCHAR(50) NULL,
	AddressLine2 NVARCHAR(50) NULL,
	City NVARCHAR(50) NULL,
	[State] INT NULL,
	ZipCode NVARCHAR(12) NULL,
	ValidFromDateTime DATETIME2(7) GENERATED ALWAYS AS ROW START NOT NULL,
	ValidToDateTime DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
	PERIOD FOR SYSTEM_TIME (ValidFromDateTime, ValidToDateTime)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = History.Address));
GO

";

		var expected2 = @"CREATE TABLE History.Address
(
	AddressKey INT NOT NULL,
	AddressLine1 NVARCHAR(50) NULL,
	AddressLine2 NVARCHAR(50) NULL,
	City NVARCHAR(50) NULL,
	[State] INT NULL,
	ZipCode NVARCHAR(12) NULL,
	ValidFromDateTime DATETIME2(7) NOT NULL,
	ValidToDateTime DATETIME2(7) NOT NULL
);
GO

";

		var table = new Table("Data", "Address");

		table.Columns.Add(new("AddressKey", DbType.Int32) { IsIdentity = true, IsPrimaryKey = true });
		table.Columns.Add(new("AddressLine1", DbType.String, 50, true));
		table.Columns.Add(new("AddressLine2", DbType.String, 50, true));
		table.Columns.Add(new("City", DbType.String, 50, true));
		table.Columns.Add(new("State", DbType.Int32, true));
		table.Columns.Add(new("ZipCode", DbType.String, 12, true));
		table.Columns.Add(new("ValidFromDateTime", DbType.DateTime2, 7) { IsRowStart = true });
		table.Columns.Add(new("ValidToDateTime", DbType.DateTime2, 7) { IsRowEnd = true, IsHidden = true });

		table.HistorySchemaName = "History";
		table.HistoryTableName = "Address";

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);

		var historyTable = table.CreateHistoryTable();
		var output2 = generator.BuildTable(historyTable);

		Debug.WriteLine(output2);
		CompareOutput(expected2, output2);
	}

	[TestMethod]
	public void SparseColumn_Test()
	{
		var expected = @"CREATE TABLE dbo.TestTable
(
	Id INT NOT NULL CONSTRAINT PK_TestTable PRIMARY KEY,
	OptionalData NVARCHAR(MAX) SPARSE
);
GO

";

		var table = new Table("dbo", "TestTable");
		table.Columns.Add(new("Id", DbType.Int32) { IsPrimaryKey = true });
		table.Columns.Add(new("OptionalData", DbType.String, -1, true) { IsSparse = true });

		var generator = new SqlServerGenerator();
		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		Debug.WriteLine(output);
		CompareOutput(expected, output);
	}
}
