using System.Data;
using System.Diagnostics;
using Tortuga.Shipyard;
using Index = Tortuga.Shipyard.Index;

namespace ShipyardTests;

[TestClass]
public sealed class SqlServerTableTests
{
	[TestMethod]
	public void TestMethod1()
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
	State INT NULL,
	TimeZone SMALLINT NULL,
	WebAddress NVARCHAR(40) NULL,
	ZipCode NVARCHAR(12) NULL,
	IndChgsLastModifiedDateTime DATETIME2(6) NULL,
	IndChgsLastModifiedByUserId NVARCHAR(8) NULL,
	LastImportRunId UNIQUEIDENTIFIER NOT NULL,
	LastImportDateTime DATETIME2(7) NOT NULL
);
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

		var generator = new SqlServerGenerator();
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
		table.Columns.Add(new("WebAddress", DbType.String, 40, true));
		table.Columns.Add(new("ZipCode", DbType.String, 12, true));
		table.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		table.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		table.Columns.Add(new("LastImportRunId", DbType.Guid, false));
		table.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7, false));

		generator.NameConstraints(table);
		generator.UseBatchSeperator = true;
		var output = generator.BuildTable(table);

		var lines1 = expected.Split("\r\n");
		var lines2 = output.Split("\r\n");
		for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
		{
			Assert.AreEqual(lines1[i], lines2[i], $"Mismatch on line {i + 1}");
		}

		Assert.AreEqual(expected, output);

		Debug.WriteLine(output);
	}

	[TestMethod]
	public void TestMethod2()
	{
		var expected = @"CREATE TABLE Imports.NameAddresses
(
	ImportKey INT IDENTITY CONSTRAINT PK_Imports_NameAddresses PRIMARY KEY NONCLUSTERED,
	ImportRunId UNIQUEIDENTIFIER NOT NULL,
	ImportDateTime DATETIME2(7) NOT NULL CONSTRAINT D_ImportDateTime DEFAULT SYSDATETIME(),
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

		var generator = new SqlServerGenerator();
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

		generator.UseBatchSeperator = true;
		generator.IncludeSchemaNameInConstraintNames = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		var lines1 = expected.Split("\r\n");
		var lines2 = output.Split("\r\n");
		for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
		{
			Assert.AreEqual(lines1[i], lines2[i], $"Mismatch on line {i + 1}");
		}

		Assert.AreEqual(expected, output);

		Debug.WriteLine(output);
	}

	[TestMethod]
	public void TestMethod3()
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

		var generator = new SqlServerGenerator();
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
		table.Columns.Add(new("AddressKey", DbType.Int32, true) { FKTableName = "Address", FKColumnName = "AddressKey" });
		table.Columns.Add(new("NameKey", DbType.Int32) { FKTableName = "Name", FKColumnName = "NameKey" });
		table.Columns.Add(new("NotePadKey", DbType.Int32) { FKTableName = "NotePad", FKColumnName = "NotePadKey" });
		table.Columns.Add(new("LastImportRunId", DbType.Guid));
		table.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7));

		table.Indexes.Add(new Index() { IndexName = "PK2_NameAddresses", OrderedColumns = { "NameId", "SequenceNumber" }, IsUnique = true, IsConstraint = true });

		generator.UseBatchSeperator = true;
		generator.NameConstraints(table);
		var output = generator.BuildTable(table);

		var lines1 = expected.Split("\r\n");
		var lines2 = output.Split("\r\n");
		for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
		{
			Assert.AreEqual(lines1[i], lines2[i], $"Mismatch on line {i + 1}");
		}

		Assert.AreEqual(expected, output);

		Debug.WriteLine(output);
	}
}
