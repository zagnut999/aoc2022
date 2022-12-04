namespace aoc2022.Day04;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>()
    {
        "2-4,6-8",
        "2-3,4-5",
        "5-7,7-9",
        "2-8,3-7",
        "6-6,4-6",
        "2-6,4-8"
    };

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day04");
        list.ShouldNotBeEmpty();
    }

    private List<int> ParseLinePart(string part)
    {
        var split = part.Split("-").Select(int.Parse).ToList();
        var count = split[1] - split[0] + 1;
        return Enumerable.Range(split[0], count).ToList();
    }

    [Test]
    public void ParseLinePartTest()
    {
        var list = ParseLinePart("1-4");
        list.ShouldBe(new List<int> { 1, 2, 3, 4 });

        list = ParseLinePart("2-4");
        list.ShouldBe(new List<int> { 2, 3, 4 });

        list = ParseLinePart("3-4");
        list.ShouldBe(new List<int> { 3, 4 });

        list = ParseLinePart("4-4");
        list.ShouldBe(new List<int> { 4 });
    }

    private (List<int>, List<int>) ParseLine(string line)
    {
        var split = line.Split(',');
        return (ParseLinePart(split[0]), ParseLinePart(split[1]));
    }

    [Test]
    public void ParseLineTest()
    {
        var (list1, list2) = ParseLine("2-4,6-8");
        list1.ShouldBe(new List<int> { 2, 3, 4 });
        list2.ShouldBe(new List<int> { 6, 7, 8 });
    }

    private bool OneContainsOther(List<int> first, List<int> second)
    {
        var intersect  = first.Intersect(second).ToList();
        if (!intersect.Any())
            return false;

        if (intersect.Count == first.Count || intersect.Count == second.Count)
            return true;

        return false;
    }

    [Test]
    public void OneContainsOtherTest()
    {
        OneContainsOther(new List<int> { 1 }, new List<int> { 1 }).ShouldBeTrue();
        OneContainsOther(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 }).ShouldBeTrue();
        OneContainsOther(new List<int> { 1, 2 }, new List<int> { 1, 2, 3 }).ShouldBeTrue();

        OneContainsOther(new List<int> { 1, 2, 3, 4 }, new List<int> { 2, 3, 4 }).ShouldBeTrue();
        OneContainsOther(new List<int> { 2, 3, 4 }, new List<int> { 1, 2, 3, 4 }).ShouldBeTrue();

        OneContainsOther(new List<int> { 1, 2, 3 }, new List<int> { 2, 3, 4 }).ShouldBeFalse();
        OneContainsOther(new List<int> { 2, 3, 4 }, new List<int> { 1, 2, 3 }).ShouldBeFalse();

        OneContainsOther(new List<int> { 1, 2, 3, 4 }, new List<int> { 2, 3, 4, 5 }).ShouldBeFalse();
        OneContainsOther(new List<int> { 2, 3, 4, 5 }, new List<int> { 1, 2, 3, 4 }).ShouldBeFalse();


        OneContainsOther(new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 }).ShouldBeFalse();
        OneContainsOther(new List<int> { 4, 5, 6 }, new List<int> { 1, 2, 3 }).ShouldBeFalse();
    }

    [Test]
    public void Example()
    {
        var count = 0;

        foreach (var item in _sample)
        {
            var (first, second) = ParseLine(item);
            if (OneContainsOther(first, second))
                count++;
        }

        count.ShouldBe(2);
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day04");
        var count = 0;

        foreach (var item in list)
        {
            var (first, second) = ParseLine(item);
            if (OneContainsOther(first, second))
                count++;
        }

        count.ShouldBe(518);
    }

    private bool OneOverlapsOther(List<int> first, List<int> second)
    {
        var intersect = first.Intersect(second).ToList();
        if (!intersect.Any())
            return false;

        return true;
    }

    [Test]
    public void OneOverlapsOtherTest()
    {
        OneOverlapsOther(new List<int> { 1 }, new List<int> { 1 }).ShouldBeTrue();
        OneOverlapsOther(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 }).ShouldBeTrue();
        OneOverlapsOther(new List<int> { 1, 2 }, new List<int> { 1, 2, 3 }).ShouldBeTrue();

        OneOverlapsOther(new List<int> { 1, 2, 3, 4 }, new List<int> { 2, 3, 4 }).ShouldBeTrue();
        OneOverlapsOther(new List<int> { 2, 3, 4 }, new List<int> { 1, 2, 3, 4 }).ShouldBeTrue();

        OneOverlapsOther(new List<int> { 1, 2, 3 }, new List<int> { 2, 3, 4 }).ShouldBeTrue();
        OneOverlapsOther(new List<int> { 2, 3, 4 }, new List<int> { 1, 2, 3 }).ShouldBeTrue();

        OneOverlapsOther(new List<int> { 1, 2, 3, 4 }, new List<int> { 2, 3, 4, 5 }).ShouldBeTrue();
        OneOverlapsOther(new List<int> { 2, 3, 4, 5 }, new List<int> { 1, 2, 3, 4 }).ShouldBeTrue();


        OneOverlapsOther(new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 }).ShouldBeFalse();
        OneOverlapsOther(new List<int> { 4, 5, 6 }, new List<int> { 1, 2, 3 }).ShouldBeFalse();
    }

    [Test]
    public void ExamplePart2()
    {
        var count = 0;

        foreach (var item in _sample)
        {
            var (first, second) = ParseLine(item);
            if (OneOverlapsOther(first, second))
                count++;
        }

        count.ShouldBe(4);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day04");
        var count = 0;

        foreach (var item in list)
        {
            var (first, second) = ParseLine(item);
            if (OneOverlapsOther(first, second))
                count++;
        }

        count.ShouldBe(909);
    }

}
