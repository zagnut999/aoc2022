namespace aoc2022.Day03;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "vJrwpWtwJgWrhcsFMMfFFhFp",
        "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL",
        "PmmdzqPrVvPwwTWBwg",
        "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn",
        "nttgJtRGJQctTZtZT",
        "CrZsJsPPZsGzwwsLwLmpwMDw"
    };

    private (List<char>, List<char>) ParseLine(string line)
    {
        var middle = line.Length / 2;
        var first = line.Take(middle).ToList();
        var second = line.Skip(middle).Take(middle).ToList();
        return (first, second);
    }

    private int Priority(char item)
    {
        if (item is >= 'a' and <= 'z')
            return item - 'a' + 1;
        return item - 'A' + 1 + 26;
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day03");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void PriorityTests()
    {
        var value = Priority('a');
        value.ShouldBe(1);
        value = Priority('z');
        value.ShouldBe(26);
        value = Priority('A');
        value.ShouldBe(27);
        value = Priority('Z');
        value.ShouldBe(52);
    }

    [Test]
    public void Example()
    {
        var score = 0;
        foreach (var line in _sample)
        {
            var (first, second) = ParseLine(line);
            var overlap = first.Intersect(second).First();
            var priority = Priority(overlap);
            score += priority;
            Console.WriteLine($"{overlap} {priority}");
        }

        score.ShouldBe(157);
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day03");
        var score = 0;
        foreach (var line in list)
        {
            var (first, second) = ParseLine(line);
            var overlap = first.Intersect(second).First();
            var priority = Priority(overlap);
            score += priority;
            Console.WriteLine($"{overlap} {priority}");
        }

        score.ShouldBe(7597);
    }

    [Test]
    public void ExamplePart2()
    {
        var score = 0;

        for (var i = 0; i < _sample.Count / 3; i++)
        {
            var lines = _sample.Skip(i * 3).Take(3).ToList();
            var overlap = lines[0].Intersect(lines[1]).Intersect(lines[2]).First();
            var priority = Priority(overlap);
            score += priority;
            Console.WriteLine($"{overlap} {priority}");
        }

        score.ShouldBe(70);
    }

    [Test]
    public async Task ActualPart2()
    {
        var score = 0;
        var list = await Utilities.ReadInputByDay("Day03");
        for (var i = 0; i < list.Count / 3; i++)
        {
            var lines = list.Skip(i * 3).Take(3).ToList();
            var overlap = lines[0].Intersect(lines[1]).Intersect(lines[2]).First();
            var priority = Priority(overlap);
            score += priority;
            Console.WriteLine($"{overlap} {priority}");
        }

        score.ShouldBe(2607);
    }

}

