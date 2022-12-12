https://adventofcode.com/2022


```
namespace aoc2022.DayXX;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new() { };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("DayXX");

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
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();

    }
}
```