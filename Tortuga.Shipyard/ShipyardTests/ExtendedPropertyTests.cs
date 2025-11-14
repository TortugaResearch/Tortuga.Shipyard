using Tortuga.Shipyard;

namespace ShipyardTests;

[TestClass]
public sealed class ExtendedPropertyTests
{
	[TestMethod]
	public void ExtendedProperty_Constructor_WithParameters_Test()
	{
		var property = new ExtendedProperty("TestName", "TestValue");
		
		Assert.AreEqual("TestName", property.Name);
		Assert.AreEqual("TestValue", property.Value);
	}

	[TestMethod]
	public void ExtendedProperty_Constructor_Default_Test()
	{
		var property = new ExtendedProperty();
		
		Assert.IsNull(property.Name);
		Assert.IsNull(property.Value);
	}

	[TestMethod]
	public void ExtendedProperty_SetProperties_Test()
	{
		var property = new ExtendedProperty
		{
			Name = "PropertyName",
			Value = "PropertyValue"
		};
		
		Assert.AreEqual("PropertyName", property.Name);
		Assert.AreEqual("PropertyValue", property.Value);
	}

	[TestMethod]
	public void ExtendedProperty_ChangeProperties_Test()
	{
		var property = new ExtendedProperty("InitialName", "InitialValue");
		
		property.Name = "ChangedName";
		property.Value = "ChangedValue";
		
		Assert.AreEqual("ChangedName", property.Name);
		Assert.AreEqual("ChangedValue", property.Value);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Add_Test()
	{
		var collection = new ExtendedPropertyCollection();
		collection.Add(new ExtendedProperty("Prop1", "Value1"));
		collection.Add(new ExtendedProperty("Prop2", "Value2"));
		
		Assert.AreEqual(2, collection.Count);
		Assert.AreEqual("Prop1", collection[0].Name);
		Assert.AreEqual("Value1", collection[0].Value);
		Assert.AreEqual("Prop2", collection[1].Name);
		Assert.AreEqual("Value2", collection[1].Value);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Remove_Test()
	{
		var collection = new ExtendedPropertyCollection();
		var property1 = new ExtendedProperty("Prop1", "Value1");
		var property2 = new ExtendedProperty("Prop2", "Value2");
		
		collection.Add(property1);
		collection.Add(property2);
		collection.Remove(property1);
		
		Assert.AreEqual(1, collection.Count);
		Assert.AreEqual("Prop2", collection[0].Name);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Clear_Test()
	{
		var collection = new ExtendedPropertyCollection();
		collection.Add(new ExtendedProperty("Prop1", "Value1"));
		collection.Add(new ExtendedProperty("Prop2", "Value2"));
		
		collection.Clear();
		
		Assert.AreEqual(0, collection.Count);
	}

	[TestMethod]
	public void ExtendedPropertyCollection_Contains_Test()
	{
		var collection = new ExtendedPropertyCollection();
		var property = new ExtendedProperty("Prop1", "Value1");
		
		collection.Add(property);
		
		Assert.IsTrue(collection.Contains(property));
	}

	[TestMethod]
	public void ExtendedProperty_NullValues_Test()
	{
		var property = new ExtendedProperty(null, null);
		
		Assert.IsNull(property.Name);
		Assert.IsNull(property.Value);
	}

	[TestMethod]
	public void ExtendedProperty_EmptyStrings_Test()
	{
		var property = new ExtendedProperty("", "");
		
		Assert.AreEqual("", property.Name);
		Assert.AreEqual("", property.Value);
	}

	[TestMethod]
	public void ExtendedProperty_SpecialCharacters_Test()
	{
		var property = new ExtendedProperty("Test's Name", "Value with \"quotes\"");
		
		Assert.AreEqual("Test's Name", property.Name);
		Assert.AreEqual("Value with \"quotes\"", property.Value);
	}
}
