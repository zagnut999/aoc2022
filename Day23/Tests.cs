using System.Reflection.Emit;
using static aoc2022.Day09.Tests;

namespace aoc2022.Day23;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new() {"....#..",
        "..###.#",
        "#...#.#",
        ".#...##",
        "#.###..",
        "##.#.##",
        ".#..#.."
    };

    private readonly List<string> _sample2 = new()
    {
        ".....",
        "..##.",
        "..#..",
        ".....",
        "..##.",
        "....."
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day23");

    private class Node
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveTo(Node node)
        {
            X = node.X;
            Y = node.Y;
            _neighbors = null;
        }

        private List<Node>? _neighbors;

        public List<Node> PossibleNeighbors() {
            if (_neighbors != null) return _neighbors;

            _neighbors = new List<Node>
            {
                new (X - 1, Y - 1),
                new (X - 1, Y),
                new (X - 1, Y + 1),

                new (X, Y - 1),
                new (X, Y + 1),

                new (X + 1, Y - 1),
                new (X + 1, Y),
                new (X + 1, Y + 1)
            };

            return _neighbors;
        }

        public List<Node> PossibleNeighbors(Direction direction)
        {
            var node = this;
            switch (direction)
            {
                case Direction.North:
                    return PossibleNeighbors().Where(n => n.Y == node.Y - 1).ToList();
                case Direction.South:
                    return PossibleNeighbors().Where(n => n.Y == node.Y + 1).ToList();
                case Direction.West:
                    return PossibleNeighbors().Where(n => n.X == node.X - 1).ToList();
                case Direction.East:
                    return PossibleNeighbors().Where(n => n.X == node.X + 1).ToList();
                default:
                    throw new ArgumentException();
            }
        }

        public Node GetNodeInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Node(X, Y - 1); ;
                case Direction.South:
                    return new Node(X, Y + 1); ;
                case Direction.West:
                    return new Node(X - 1, Y);
                case Direction.East:
                    return new Node(X + 1, Y);
                default:
                    throw new ArgumentException();
            }
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Node node) return false;

            return X == node.X && Y == node.Y;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    private List<Node> ParseInput(List<string> input)
    {
        var result = new List<Node>();
        var yMax = input.Count;
        var xMax = input.First().Length;

        for (var y = 0; y < yMax; y++)
        {
            for (var x = 0; x < xMax; x++)
            {
                var item = input[y][x];
                if (item == '#') result.Add(new Node(x, y));
            }
        }

        return result;
    }
    private void PrintMap(string label, List<Node> nodes)
    {
        var xMax = nodes.Max(node => node.X) + 2;
        var xMin = nodes.Min(node => node.X) - 2;
        var yMax = nodes.Max(node => node.Y) + 2;
        var yMin = nodes.Min(node => node.Y) - 2;
        Console.WriteLine(label);
        for (var y = yMin; y <= yMax; y++)
        {
            for (var x = xMin; x <= xMax; x++)
            {
                var node = nodes.FirstOrDefault(node => node.X == x && node.Y == y);
                Console.Write(node == null ? '.' : '#');
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private enum Direction
    {
        North = 0,
        South,
        West,
        East
    }

    [Test]
    public void TestDirection()
    {
        var firstDirection = Direction.North;
        for (var y = 0; y < 10; y++)
        {
            for (var x = (int)firstDirection; x < (int)firstDirection + 4; x++)
            {
                var currentDirection = (Direction)(x % 4);
                Console.WriteLine(currentDirection.ToString());
            }
            Console.WriteLine();
            firstDirection = (Direction) (((int)firstDirection + 1) % 4);
        }
    }

    [Test]
    public void RemovingDups()
    {
        var node1 = new Node(0, 0);
        var node2 = new Node(0, 0);
        node1.Equals(node2).ShouldBeTrue();

        var nodeLeft = new Node(1, 1);
        var nextSteps = new Dictionary<Node, Node>
        {
            { new Node(0, 0), new Node(1, 1) },
            { new Node(1, 0), new Node(1, 1) },
            { nodeLeft, new Node(0, 0) }
        };

        var toRemove = nextSteps
            .GroupBy(x => x.Value)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Select(x => x.Key))
            .ToList();
        
        toRemove.ShouldBeEquivalentTo(new List<Node> { new Node(0, 0), new Node(1, 0) });
        toRemove.Contains(new Node(1, 1)).ShouldBeFalse();

        toRemove.ForEach(x => nextSteps.Remove(x));
        nextSteps.ContainsKey(nodeLeft).ShouldBeTrue();
    }

    private int Score(List<Node> nodes)
    {
        var xMax = nodes.Max(node => node.X);
        var xMin = nodes.Min(node => node.X);
        var yMax = nodes.Max(node => node.Y);
        var yMin = nodes.Min(node => node.Y);
        var count = 0;
        for (var y = yMin; y <= yMax; y++)
        {
            for (var x = xMin; x <= xMax; x++)
            {
                if (!nodes.Any(node => node.X == x && node.Y == y))
                    count++;
            }
        }
        return count;
    }

    private (int Score, int Iterations) Run(List<string> input, bool showMaps = false, int iterations = 10)
    {
        var nodes = ParseInput(input);
        if (showMaps) PrintMap("Initial", nodes);

        var nextSteps = new Dictionary<Node, Node>();
        var direction = Direction.North;
        int counter;
        for (counter = 0; counter < iterations; counter++)
        {
            foreach (var node in nodes)
            {
                //No neighbors
                if (!node.PossibleNeighbors().Any(x => nodes.Contains(x)))
                    continue;

                for (var x = (int)direction; x < (int)direction + 4; x++)
                {
                    var currentDirection = (Direction)(x % 4);

                    // Is there a neighbor this way?
                    if (node.PossibleNeighbors(currentDirection).Any(x => nodes.Contains(x))) continue;

                    nextSteps.Add(node, node.GetNodeInDirection(currentDirection));
                    break;
                }
            }
            if (nextSteps.Count == 0) break;

            var toRemove = nextSteps.GroupBy(x => x.Value)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Select(x => x.Key))
                .ToList();

            toRemove.ForEach(x => nextSteps.Remove(x));

            foreach (var step in nextSteps)
            {
                step.Key.MoveTo(step.Value);
            }
            nextSteps.Clear();

            if (showMaps) PrintMap($"Round {counter + 1}", nodes);

            direction = (Direction)(((int)direction + 1) % 4);
        }

        return (Score(nodes), counter + 1);
    }

    [Test]
    public void Example()
    {
        var result = Run(_sample, true);
        result.Score.ShouldBe(110);
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
        var result = Run(list);

        result.Score.ShouldBe(4158);
    }

    [Test]
    public void ExamplePart2()
    {
        var result = Run(_sample, true, int.MaxValue);
        result.Iterations.ShouldBe(20);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        var result = Run(list, iterations: int.MaxValue);

        result.Iterations.ShouldBe(19);

    }
}