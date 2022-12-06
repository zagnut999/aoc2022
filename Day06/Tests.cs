namespace aoc2022.Day06;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>()
    {
        "mjqjpqmgbljsphdztnvjfqwrcgsmlb",
        "bvwbjplbgvbhsrlpgdmjqwftvncz",
        "nppdvjthqldpwncqszvftbrmjlhg",
        "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg",
        "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw"
    };

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day06");
        list.ShouldNotBeEmpty();
    }

    public int FindStartOfMessage(string message, int windowSize = 4)
    {
        var window = "";
        for (var i = 0; i < message.Length; i++)
        {
            if (window.Length == windowSize)
                window = window.Remove(0, 1);
            window += message[i];

            if (i > windowSize -1 && window.GroupBy(x=> x).All(x=>x.Count() == 1))
                return i + 1;
        }

        return -1;
    }

    [Test]
    public void Example()
    {
        FindStartOfMessage(_sample[0]).ShouldBe(7);
        FindStartOfMessage(_sample[1]).ShouldBe(5);
        FindStartOfMessage(_sample[2]).ShouldBe(6);
        FindStartOfMessage(_sample[3]).ShouldBe(10);
        FindStartOfMessage(_sample[4]).ShouldBe(11);
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day06");
        FindStartOfMessage(list[0]).ShouldBe(1155);
    }

    [Test]
    public void ExamplePart2()
    {
        var windowSize = 14;
        FindStartOfMessage(_sample[0], windowSize).ShouldBe(19);
        FindStartOfMessage(_sample[1], windowSize).ShouldBe(23);
        FindStartOfMessage(_sample[2], windowSize).ShouldBe(23);
        FindStartOfMessage(_sample[3], windowSize).ShouldBe(29);
        FindStartOfMessage(_sample[4], windowSize).ShouldBe(26);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day06");
        FindStartOfMessage(list[0], 14).ShouldBe(2789);
    }
}

