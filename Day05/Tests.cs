using System.Text.RegularExpressions;

namespace aoc2022.Day05;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>()
    {
        "    [D]    ",
        "[N] [C]    ",
        "[Z] [M] [P]",
        " 1   2   3 ",
        "",
        "move 1 from 2 to 1",
        "move 3 from 1 to 3",
        "move 2 from 2 to 1",
        "move 1 from 1 to 2"
    };

    public class State
    {
        private List<string> Stacks { get; set; } = new List<string>();
        private List<Move> Moves { get; set; } = new List<Move>();

        public State(List<string> input)
        {
            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("move"))
                    Moves.Add(new Move(line));
                else
                {
                    ProcessCrosscut(line);
                }
            }
        }

        public string TopOfStacks()
        {
            var result = "";

            foreach (var stack in Stacks)
            {
                result += stack.Any() ? stack.First() : ' ';
            }

            return result;
        }

        public void ProcessMoves9000()
        {
            foreach (var move in Moves)
            {
                if (Stacks.Count < move.From || Stacks.Count < move.To || Stacks[move.From - 1].Length < move.Take)
                    throw new ArgumentException(move.ToString());

                var originalFrom = Stacks[move.From - 1];
                var taken = originalFrom.Substring(0, move.Take);
                var newFrom = originalFrom.Remove(0, move.Take);
                Stacks[move.From - 1] = newFrom;

                var newTo = Stacks[move.To - 1];
                foreach (var take in taken)
                    newTo = take + newTo;
                Stacks[move.To - 1] = newTo;
            }
        }

        public void ProcessMoves9001()
        {
            foreach (var move in Moves)
            {
                if (Stacks.Count < move.From || Stacks.Count < move.To || Stacks[move.From - 1].Length < move.Take)
                    throw new ArgumentException(move.ToString());

                var originalFrom = Stacks[move.From - 1];
                var taken = originalFrom.Substring(0, move.Take);
                var newFrom = originalFrom.Remove(0, move.Take);
                Stacks[move.From - 1] = newFrom;

                var newTo = Stacks[move.To - 1];
                newTo = taken + newTo;
                Stacks[move.To - 1] = newTo;
            }
        }

        private static readonly Regex CrosscutRegex = new(@"[ \[](.)[ \]] ?");
        private static readonly string Numbers = "123456789";
        private void ProcessCrosscut(string line)
        {
            var matches = CrosscutRegex.Matches(line);

            foreach (var (match, index) in matches.Select((match, index)=> (match, index)))
            {
                if (match.Success)
                {
                    var value = match.Groups[1].Value;
                    if (value != " " && !Numbers.Contains(value))
                    {
                        while (Stacks.Count <= index)
                            Stacks.Add("");
                        
                        Stacks[index] += value;
                    }
                }
                else
                {
                    throw new ArgumentException("invalid format of crosscut");
                }
            }
        }
    }
    
    public class Move
    {
        public int Take { get; }
        public int From { get; }
        public int To { get; }

        private readonly string _command;
        private static readonly Regex MoveRegex = new(@"move (\d+) from (\d+) to (\d+)");

        public Move(string command)
        {
            _command = command;
            var match = MoveRegex.Match(command);
            if (!match.Success)
                throw new ArgumentException();

            Take = int.Parse(match.Groups[1].Value);
            From = int.Parse(match.Groups[2].Value);
            To = int.Parse(match.Groups[3].Value);
        }

        public override string ToString()
        {
            return _command;
        }
    }

    [Test]
    public void MoveTests()
    {
        var move = new Move("move 1 from 2 to 1");
        move.Take.ShouldBe(1);
        move.From.ShouldBe(2);
        move.To.ShouldBe(1);

        move = new Move("move 10 from 20 to 10");
        move.Take.ShouldBe(10);
        move.From.ShouldBe(20);
        move.To.ShouldBe(10);
    }

    [Test]
    public void ParseInputTest()
    {
        var state = new State(_sample);
        state.TopOfStacks().ShouldBe("NDP");
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day05");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void Example()
    {
        var state = new State(_sample);
        state.TopOfStacks().ShouldBe("NDP");
        state.ProcessMoves9000();
        state.TopOfStacks().ShouldBe("CMZ");
    }

    [Test]
    public async Task Actual()
    {
        var input = await Utilities.ReadInputByDay("Day05");
        var state = new State(input);
        state.TopOfStacks().ShouldBe("ZVGNQTHQM");
        state.ProcessMoves9000();
        state.TopOfStacks().ShouldBe("QGTHFZBHV");
    }

    [Test]
    public void ExamplePart2()
    {
        var state = new State(_sample);
        state.TopOfStacks().ShouldBe("NDP");
        state.ProcessMoves9001();
        state.TopOfStacks().ShouldBe("MCD");
    }

    [Test]
    public async Task ActualPart2()
    {
        var input = await Utilities.ReadInputByDay("Day05");
        var state = new State(input);
        state.TopOfStacks().ShouldBe("ZVGNQTHQM");
        state.ProcessMoves9001();
        state.TopOfStacks().ShouldBe("MGDMPSZTM");
    }
}

