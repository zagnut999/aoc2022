https://adventofcode.com/2022


```
namespace aoc2022.DayXX;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>() { };

    [Test]
    public void Example()
    {
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("DayXX");
        list.ShouldNotBeEmpty();
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
```