namespace ShipyardTests;

public class TestsBase
{
	protected void CompareOutput(string expected, string output)
	{
		var lines1 = expected.Split("\r\n");
		var lines2 = output.Split("\r\n");
		for (int i = 0; i < Math.Min(lines1.Length, lines2.Length); i++)
		{
			Assert.AreEqual(lines1[i], lines2[i], $"Mismatch on line {i + 1}");
		}

		Assert.AreEqual(expected, output);
	}
}
