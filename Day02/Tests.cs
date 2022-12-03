namespace aoc2022.Day02;

[TestFixture]
internal class Tests
{
    private enum Choice
    {
        Rock,
        Paper,
        Scissors
    }

    private readonly Dictionary<string, Choice> _mapping = new()
    {
        {"A", Choice.Rock},
        {"B", Choice.Paper},
        {"C", Choice.Scissors},
        {"X", Choice.Rock},
        {"Y", Choice.Paper},
        {"Z", Choice.Scissors}
    };

    private readonly Dictionary<Choice, Choice> _rules = new()
    {
        { Choice.Rock, Choice.Scissors }, //Rock beats Scissors
        { Choice.Scissors, Choice.Paper },
        { Choice.Paper, Choice.Rock }
    };

    private readonly List<string> _sample = new()
    {
        "A Y",
        "B X",
        "C Z"
    };

    private (Choice, Choice) ParseLine(string line)
    {
        var split = line.Split(" ");
        var first = _mapping[split[0]];
        var second = _mapping[split[1]];
        return (first, second);
    }

    private readonly Dictionary<Choice, int> _choicePoints = new()
    {
        { Choice.Rock, 1 },
        { Choice.Paper, 2 },
        { Choice.Scissors, 3 },
    };

    private int MatchPoints(Choice first, Choice second)
    {
        if (first == second)
            return 3;
        if (_rules[first] == second)
            return 6;
        return 0;
    }

    private int MatchScore(Choice first, Choice second)
    {
        return MatchPoints(first, second) + _choicePoints[first];
    }


    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day02");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void Example()
    {
        var score = 0;
        foreach (var line in _sample)
        {
            var (them, me) = ParseLine(line);
            var matchScore = MatchScore(me, them);
            score += matchScore;
            Console.WriteLine($"Them:{them} Me:{me} : {matchScore}");
        }

        score.ShouldBe(15);
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day02");

        var score = 0;
        foreach (var line in list)
        {
            var (them, me) = ParseLine(line);
            var matchScore = MatchScore(me, them);
            score += matchScore;
            Console.WriteLine($"Them:{them} Me:{me} : {matchScore}");
        }

        score.ShouldBe(15632);
    }

    public enum Result
    {
        Lose,
        Draw,
        Win
    }

    private readonly Dictionary<string, Result> _mappingResult = new()
    {
        {"X", Result.Lose},
        {"Y", Result.Draw},
        {"Z", Result.Win}
    };

    private (Choice, Result) ParseLinePart2(string line)
    {
        var split = line.Split(" ");
        var first = _mapping[split[0]];
        var second = _mappingResult[split[1]];
        return (first, second);
    }

    private Choice GetChoice(Choice them, Result result)
    {
        return result switch
        {
            Result.Draw => them,
            Result.Win => _rules.First(x => x.Value == them).Key,
            _ => _rules[them]
        };
    }


    [Test]
    public void ExamplePart2()
    {
        var score = 0;
        foreach (var line in _sample)
        {
            var (them, result) = ParseLinePart2(line);
            var me = GetChoice(them, result);
            var matchScore = MatchScore(me, them);
            score += matchScore;
            Console.WriteLine($"Them:{them} Me:{me} : {matchScore}");
        }

        score.ShouldBe(12);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day02");

        var score = 0;
        foreach (var line in list)
        {
            var (them, result) = ParseLinePart2(line);
            var me = GetChoice(them, result);
            var matchScore = MatchScore(me, them);
            score += matchScore;
            Console.WriteLine($"Them:{them} Me:{me} : {matchScore}");
        }

        score.ShouldBe(14416);
    }
}
