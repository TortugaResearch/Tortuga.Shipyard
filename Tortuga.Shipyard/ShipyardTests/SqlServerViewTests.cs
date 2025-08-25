using System.Data;
using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class SqlServerViewTests
{
	[TestMethod]
	public void Scneario_1()
	{
		var nameAddressTable = new Table("Storage", "NameAddresses");

		nameAddressTable.Columns.Add(new("NameAddressesKey", DbType.Int32) { IsPrimaryKey = true, IsIdentity = true });
		nameAddressTable.Columns.Add(new("AddressCategory", DbType.Int16, true));
		nameAddressTable.Columns.Add(new("AddressNumber", DbType.Int32, true));
		nameAddressTable.Columns.Add(new("AlternateNameNumber", DbType.Int32, true));
		nameAddressTable.Columns.Add(new("AlternateNameType", DbType.Int16, true));
		nameAddressTable.Columns.Add(new("NameId", DbType.Int32));
		nameAddressTable.Columns.Add(new("NotePadId", DbType.Int32, true));
		nameAddressTable.Columns.Add(new("SequenceNumber", DbType.Int16));
		nameAddressTable.Columns.Add(new("WireEnabledIndicator", DbType.Int16, true));
		nameAddressTable.Columns.Add(new("IndChgsLastModifiedDateTime", DbType.DateTime2, 6, true));
		nameAddressTable.Columns.Add(new("IndChgsLastModifiedByUserId", DbType.String, 8, true));
		nameAddressTable.Columns.Add(new("AddressKey", DbType.Int32, true) { FKTableName = "Address", FKColumnName = "AddressKey" });
		nameAddressTable.Columns.Add(new("NameKey", DbType.Int32) { FKTableName = "Name", FKColumnName = "NameKey" });
		nameAddressTable.Columns.Add(new("NotePadKey", DbType.Int32) { FKTableName = "NotePad", FKColumnName = "NotePadKey" });
		nameAddressTable.Columns.Add(new("LastImportRunId", DbType.Guid));
		nameAddressTable.Columns.Add(new("LastImportDateTime", DbType.DateTime2, 7));

		var nameTable = new Table("Storage", "Name");

		var addressTable = new Table("Storage", "Address");

		var view = nameAddressTable.CreateView("Reporting", "NameAddresses");

		//view.Join(JoinType.Inner, nameTable, "NameKey");
	}
}
