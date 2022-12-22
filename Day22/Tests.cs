using System.Text.RegularExpressions;

namespace aoc2022.Day22;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "        ...#",
        "        .#..",
        "        #...",
        "        ....",
        "...#.......#",
        "........#...",
        "..#....#....",
        "..........#.",
        "        ...#....",
        "        .....#..",
        "        .#......",
        "        ......#.",
        "",
        "10R5L5R10L4R5L5"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day22");


    private class Node
    {
        public Node(int x, int y, char type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public char Type { get; set; }

        public int X { get; }
        public int Y { get; }

        public Node? Up { get; set; }
        public Node? Down { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        

        public override string ToString()
        {
            return $"{X},{Y}: {Type}";
        }

        public Node? Next(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right: return Right;
                case Direction.Down: return Down;
                case Direction.Left: return Left;
                case Direction.Up: return Up;
                default: throw new ArgumentException();
            }
        }
    }

    private static readonly Regex CommandRegex = new Regex(@"(?<distance>\d+)|(?<turn>[LR])");
    private interface ICommand {}

    private class Move : ICommand
    {
        public int Distance { get; set; }
        public Move(int distance)
        {
            Distance = distance;
        }
    }
    private class Turn : ICommand
    {
        public string Direction { get; set; }

        public Turn(string direction)
        {
            Direction = direction;
        }
    }

    private Direction NewDirection(Direction current, Turn turn)
    {
        switch (current)
        {
            case Tests.Direction.Right:
                return turn.Direction == "L" ? Tests.Direction.Up : Tests.Direction.Down;
            case Tests.Direction.Left:
                return turn.Direction == "L" ? Tests.Direction.Down : Tests.Direction.Up;
            case Tests.Direction.Down:
                return turn.Direction == "L" ? Tests.Direction.Right : Tests.Direction.Left;
            case Tests.Direction.Up:
                return turn.Direction == "L" ? Tests.Direction.Left : Tests.Direction.Right;
            default:
                throw new ArgumentException();
        }
    }

    private enum Direction
    {
        Right = 0,
        Down,
        Left,
        Up
    }

    private char GetType(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right: return '>';
            case Direction.Down: return 'V';
            case Direction.Left: return '<';
            case Direction.Up: return '^';
            default: throw new ArgumentException();
        }
    }

    private (List<Node> Nodes, Node Start, List<ICommand> Commands) ParseInput(List<string> input)
    {
        var nodes = new List<Node>();
        Node? start = null;

        var yMax = input.Count - 2;
        var xMax = input.Take(yMax).Max(line => line.Length);

        for (var y = 0; y < yMax; y++)
        {
            for (var x = 0; x < xMax; x++)
            {
                if (input[y].Length <= x) continue;
                var type = input[y][x];
                if (type == ' ') continue;

                var current = new Node(x, y, type);
                
                nodes.Add(current);

                var up = nodes.FirstOrDefault(node => node.X == x && node.Y == y - 1);
                var left = nodes.FirstOrDefault(node => node.X == x - 1 && node.Y == y);

                current.Left = left;
                current.Up = up;

                start ??= current;
                if (left != null) left.Right = current;
                if (up != null) up.Down = current;
            }
        }

        for (var y = 0; y < yMax; y++)
        {
            var row = nodes.Where(node => node.Y == y).ToList();
            row.First().Left = row.Last();
            row.Last().Right = row.First();
        }

        for (var x = 0; x < xMax; x++)
        {
            var column = nodes.Where(node => node.X == x).ToList();
            column.First().Up = column.Last();
            column.Last().Down = column.First();
        }

        var commands = new List<ICommand>();
        var matches = CommandRegex.Matches(input.Last());
        foreach (Match match in matches)
        {
            if (match.Groups["distance"].Success)
                commands.Add(new Move(int.Parse(match.Groups["distance"].Value)));
            else
                commands.Add(new Turn(match.Groups["turn"].Value));
        }

        return (nodes, start!, commands);
    }

    private void PrintMap(string label, List<Node> nodes)
    {
        var xMax = nodes.Max(node => node.X);
        var yMax = nodes.Max(node => node.Y);
        Console.WriteLine(label);
        for (var y = 0; y <= yMax; y++)
        {
            for (var x = 0; x <= xMax; x++)
            {
                var node = nodes.FirstOrDefault(node => node.X == x && node.Y == y);
                Console.Write(node?.Type ?? ' ');
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    [Test]
    public void Example()
    {
        var result = ParseInput(_sample);
        result.Start.ShouldNotBeNull();
        PrintMap("init",result.Nodes);

        var currentDirection = Direction.Right;
        var currentNode = result.Start;
        currentNode.Type = GetType(currentDirection);
        foreach (var command in result.Commands)
        {
            switch (command)
            {
                case Turn turn:
                    currentDirection = NewDirection(currentDirection, turn);
                    currentNode.Type = GetType(currentDirection);
                    PrintMap($"{currentNode}, {currentDirection}, {turn.Direction}", result.Nodes);
                    break;

                case Move move:
                    for (var z = 0; z < move.Distance; z++)
                    {
                        var next = currentNode.Next(currentDirection);
                        if (next != null && next.Type != '#')
                        {
                            currentNode = next;
                            currentNode.Type = GetType(currentDirection);
                        }
                    }
                    PrintMap($"{currentNode}, {currentDirection}, {move.Distance}", result.Nodes);
                    break;
            }
            
        }
        PrintMap("final", result.Nodes);
        var score = 1000 * (currentNode.Y + 1) + 4 * (currentNode.X + 1) + (int)currentDirection;

        score.ShouldBe(6032);
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
        var result = ParseInput(list);
        result.Start.ShouldNotBeNull();
        
        var currentDirection = Direction.Right;
        var currentNode = result.Start;
        currentNode.Type = GetType(currentDirection);
        foreach (var command in result.Commands)
        {
            switch (command)
            {
                case Turn turn:
                    currentDirection = NewDirection(currentDirection, turn);
                    currentNode.Type = GetType(currentDirection);
                    break;

                case Move move:
                    for (var z = 0; z < move.Distance; z++)
                    {
                        var next = currentNode.Next(currentDirection);
                        if (next != null && next.Type != '#')
                        {
                            currentNode = next;
                            currentNode.Type = GetType(currentDirection);
                        }
                    }
                    break;
            }

        }
        var score = 1000 * (currentNode.Y + 1) + 4 * (currentNode.X + 1) + (int)currentDirection;

        score.ShouldBe(155060);
    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();

    }
}