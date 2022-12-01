namespace aoc2022.Day01;

[TestFixture]
internal class Day01
{
    [Test]
    public void Example()
    {
        var rawList = new List<string> {
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
        Elf.FindHighestCalories(rawList).ShouldBe(24000);
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

    internal class Elf
    {
        public static int FindHighestCalories(List<string> rawList)
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

            return elves.Max();
        }
    }

}
