namespace aoc2022.Day01;

[TestFixture]
internal class Tests
{
    private List<string> _sample = new List<string> {
        "1000",
        "2000",
        "3000",
        "",
        "4000",
        "",
        "5000",
        "6000",
        "",
        "7000",
        "8000",
        "9000",
        "",
        "10000"
    };

    [Test]
    public void Example()
    {
        Elf.FindHighestCalories(_sample).ShouldBe(24000);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day01");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day01");
        var result = Elf.FindHighestCalories(list);

        result.ShouldBe(68467);
    }

    [Test]
    public void ExamplePart2()
    {
        Elf.FindSumOfThreeHighestCalories(_sample).ShouldBe(45000);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day01");
        var result = Elf.FindSumOfThreeHighestCalories(list);

        result.ShouldBe(203420);
    }

    internal class Elf
    {
        private static List<int> GetCalories(List<string> rawList)
        {
            var elves = new List<int>();
            var sum = 0;
            foreach (var item in rawList)
            {
                if (int.TryParse(item, out var cal))
                {
                    sum += cal;
                }
                else
                {
                    elves.Add(sum);
                    sum = 0;
                }
            }
            elves.Add(sum);
            return elves;
        }

        public static int FindHighestCalories(List<string> rawList)
        {
            var elves = GetCalories(rawList);

            return elves.Max();
        }

        public static int FindSumOfThreeHighestCalories(List<string> rawList)
        {
            return GetCalories(rawList).OrderByDescending(x => x).Take(3).Sum();
        }
    }
}
