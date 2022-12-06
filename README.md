https://adventofcode.com/2022



namespace aoc2022.Day06;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>() { };

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day06");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void Example()
    {
    }

    [Test]
    public async Task Actual()
    {
    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
    }
}