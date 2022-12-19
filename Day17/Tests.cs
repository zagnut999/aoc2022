namespace aoc2022.Day17;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day17");

    [Test]
    public void Example()
    {
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await ReadInputFile();

    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();

    }
}