using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace aoc2022.Day11;

[TestFixture]
internal class Tests
{
    private readonly string _sample = string.Join('\n', new List<string>()
    {
        "Monkey 0:",
        "  Starting items: 79, 98",
        "  Operation: new = old * 19",
        "  Test: divisible by 23",
        "    If true: throw to monkey 2",
        "    If false: throw to monkey 3",
        "",
        "Monkey 1:",
        "  Starting items: 54, 65, 75, 74",
        "  Operation: new = old + 6",
        "  Test: divisible by 19",
        "    If true: throw to monkey 2",
        "    If false: throw to monkey 0",
        "",
        "Monkey 2:",
        "  Starting items: 79, 60, 97",
        "  Operation: new = old * old",
        "  Test: divisible by 13",
        "    If true: throw to monkey 1",
        "    If false: throw to monkey 3",
        "",
        "Monkey 3:",
        "  Starting items: 74",
        "  Operation: new = old + 3",
        "  Test: divisible by 17",
        "    If true: throw to monkey 0",
        "    If false: throw to monkey 1"
    });

    private static async Task<string> ReadInputFile() => await Utilities.ReadInputByDayRaw("Day11");

    private class Monkey
    {
        public int MonkeyID { get; init; }
        public List<int> Items { get; init; } = new();
        public string Operation { get; init; } = string.Empty;
        public int Test { get; init; }
        public int IfTrue { get; init; }
        public int IfFalse { get; init; }
        public uint Inspections { get; set; }

        private readonly Regex _operationAddInt = new(@"old \+ (\d+)");
        private readonly Regex _operationMultiplyInt = new(@"old \* (\d+)");

        public int DoOp(int value)
        {
            if (Operation == "old * old")
                return value * value;

            if (Operation == "old + old")
                return value + value;

            var multiMatch = _operationMultiplyInt.Match(Operation);
            var addMatch = _operationAddInt.Match(Operation);
            if (addMatch.Success)
                return value + int.Parse(addMatch.Groups[1].Value);
            if (multiMatch.Success)
                return value * int.Parse(multiMatch.Groups[1].Value);

            throw new ArgumentException($"Operation not found: {Operation}");
        }
    }

    private class MonkeyBusiness
    {
        public List<Monkey> Monkeys { get; }

        public MonkeyBusiness(string input)
        {
            Monkeys = ParseInput(input);
        }

        public void DoTheBusiness(int iterations = 20, bool tooWorried = false)
        {
            for (var round = 0; round < iterations; round++)
            {
                foreach (var monkey in Monkeys)
                {
                    foreach (var item in monkey.Items)
                    {
                        monkey.Inspections++;
                        var newValue = monkey.DoOp(item);

                        if (!tooWorried)
                            newValue /= 3;
                        else
                            newValue *= monkey.Test;

                        if (newValue % monkey.Test == 0)
                        {
                            Monkeys[monkey.IfTrue].Items.Add(newValue);
                        }
                        else
                        {
                            Monkeys[monkey.IfFalse].Items.Add(newValue);
                        }
                    }
                    monkey.Items.Clear();
                }
            }
        }

        public static readonly Regex MonkeyRegex = new(@"Monkey (?<monkeyID>\d+):\n  Starting items: (?<items>.*)\n  Operation: new = (?<operation>.*)\n  Test: divisible by (?<test>.*)\n.+ (?<true>\d+)\n.+ (?<false>\d+)", RegexOptions.Multiline);
        public static List<Monkey> ParseInput(string input)
        {
            var monkeys = new List<Monkey>();

            var matches = MonkeyRegex.Matches(input);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                var monkey = new Monkey
                {
                    MonkeyID = int.Parse(groups["monkeyID"].Value),
                    Items = groups["items"].Value.Split(", ").Select(int.Parse).ToList(),
                    Operation = groups["operation"].Value,
                    Test = int.Parse(groups["test"].Value),
                    IfTrue = int.Parse(groups["true"].Value),
                    IfFalse = int.Parse(groups["false"].Value)
                };

                monkeys.Add(monkey);
            }

            return monkeys;
        }
    }

    [Test]
    public void OperationTests()
    {
        var value = 10;
        var monkey = new Monkey { MonkeyID = 1, Operation = "old + old", Test = 2, IfTrue = 1, IfFalse = 2, Items = new List<int> { 1, 2 } };
        monkey.DoOp(value).ShouldBe(20);

        monkey = new Monkey { MonkeyID = 1, Operation = "old * old", Test = 2, IfTrue = 1, IfFalse = 2, Items = new List<int> { 1, 2 } };
        monkey.DoOp(value).ShouldBe(100);

        monkey = new Monkey { MonkeyID = 1, Operation = "old * 3", Test = 2, IfTrue = 1, IfFalse = 2, Items = new List<int> { 1, 2 } };
        monkey.DoOp(value).ShouldBe(30);

        monkey = new Monkey { MonkeyID = 1, Operation = "old + 3", Test = 2, IfTrue = 1, IfFalse = 2, Items = new List<int> { 1, 2 } };
        monkey.DoOp(value).ShouldBe(13);

    }

    [Test]
    public async Task ParseInputTests()
    {
        var input = await Utilities.ReadInputByDayRaw("Day11");

        var matches = MonkeyBusiness.MonkeyRegex.Matches(input);

        matches.Count.ShouldBe(8);

        foreach (Match match in matches)
        {
            match.Groups.Count.ShouldBe(6 + 1); //The zero group is special
            var groups = match.Groups;
            groups["monkeyID"].Value.ShouldNotBeNullOrEmpty();
            groups["items"].Value.ShouldNotBeNullOrEmpty();
            groups["operation"].Value.ShouldNotBeNullOrEmpty();
            groups["test"].Value.ShouldNotBeNullOrEmpty();
            groups["true"].Value.ShouldNotBeNullOrEmpty();
            groups["false"].Value.ShouldNotBeNullOrEmpty();
        }

        var monkeys = MonkeyBusiness.ParseInput(input);
        monkeys.Count.ShouldBe(8);
    }

    [Test]
    public void Example()
    {
        var monkeyBusiness = new MonkeyBusiness(_sample);
        monkeyBusiness.DoTheBusiness();

        monkeyBusiness.Monkeys[0].Inspections.ShouldBe(101U);
        monkeyBusiness.Monkeys[1].Inspections.ShouldBe(95U);
        monkeyBusiness.Monkeys[2].Inspections.ShouldBe(7U);
        monkeyBusiness.Monkeys[3].Inspections.ShouldBe(105U);

        var inspections = monkeyBusiness.Monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x=>x.Inspections).ToList();
        (inspections[0] * inspections[1]).ShouldBe(10605U);
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
        var monkeyBusiness = new MonkeyBusiness(list);
        monkeyBusiness.DoTheBusiness();

        var inspections = monkeyBusiness.Monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x => x.Inspections).ToList();
        (inspections[0] * inspections[1]).ShouldBe(57838U);
    }

    [Test]
    public void ExamplePart2()
    {
        var monkeyBusiness = new MonkeyBusiness(_sample);
        monkeyBusiness.DoTheBusiness(10000, true);

        monkeyBusiness.Monkeys[0].Inspections.ShouldBe(52166U);
        monkeyBusiness.Monkeys[1].Inspections.ShouldBe(47830U);
        monkeyBusiness.Monkeys[2].Inspections.ShouldBe(1938U);
        monkeyBusiness.Monkeys[3].Inspections.ShouldBe(52013U);

        var inspections = monkeyBusiness.Monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x => x.Inspections).ToList();
        (inspections[0] * inspections[1]).ShouldBe(2713310158U);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();

        var monkeyBusiness = new MonkeyBusiness(list);
        monkeyBusiness.DoTheBusiness(10000, true);

        var inspections = monkeyBusiness.Monkeys.OrderByDescending(x => x.Inspections).Take(2).Select(x => x.Inspections).ToList();
        (inspections[0] * inspections[1]).ShouldBe(57838U);
    }
}
