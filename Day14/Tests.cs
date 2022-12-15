using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace aoc2022.Day14;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "498,4 -> 498,6 -> 496,6",
        "503,4 -> 502,4 -> 502,9 -> 494,9"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day14");

    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Node StartNode() => new Node(500, 0);
        public Node Left => new (X - 1, Y + 1);
        public Node Down => new (X, Y + 1);
        public Node Right => new (X + 1, Y + 1);

        public Node(int x, int y)
        {
            X = x; Y = y;
        }

        public Node(string input)
        {
            var split = input.Split(',');
            X = int.Parse(split[0]);
            Y = int.Parse(split[1]);
        }

        public override string ToString() => $"{X},{Y}";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is not Node node) return false;

            return node.X == X && node.Y == Y;
        }
    }

    private List<Node> Expand(Node first, Node second)
    {
        var line = new List<Node>();

        if (first.X == second.X)
        {
            var yStart = first.Y < second.Y ? first.Y : second.Y;
            var yEnd = first.Y < second.Y ? second.Y : first.Y;

            for (var y = yStart; y <= yEnd; y++)
            {
                line.Add(new Node(first.X, y));
            }
        }
        else
        {
            var xStart = first.X < second.X ? first.X : second.X;
            var xEnd = first.X < second.X ? second.X : first.X;

            for (var x = xStart; x <= xEnd; x++)
            {
                line.Add(new Node(x, first.Y));
            }
        }

        return line;
    }

    private Dictionary<Node, string> ParseInput(List<string> input)
    {
        var map = new Dictionary<Node, string>();
        foreach (var item in input)
        {
            var points = item.Split(" -> ").Select(pair => new Node(pair)).ToList();
            for (var index = 0; index < points.Count - 1; index++)
            {
                foreach (var node in Expand(points[index], points[index + 1]))
                {
                    if (!map.ContainsKey(node))
                        map.Add(node, "#");
                }
            }
        }
        return map;
    }

    private List<string> Output(Dictionary<Node, string> map)
    {
        var result = new List<string>();
        var yMax = map.Keys.Max(node => node.Y);
        var xMin = map.Keys.Min(node => node.X);
        var xMax = map.Keys.Max(node => node.X);

        for (var y = 0; y <= yMax; y++)
        {
            var line = $"{y:D3}  >";
            for (var x = xMin; x <= xMax; x++)
            {
                var node = new Node(x, y);
                if (map.ContainsKey(node))
                    line += map[node];
                else 
                    line += " ";
                
            }
            result.Add(line + "<");
        }
        return result;
    }

    private void Print(Dictionary<Node, string> map)
    {
        var output = Output(map);
        Console.WriteLine(string.Join('\n', output));
    }

    private bool Drip(Dictionary<Node, string> map, int? floor = null)
    {
        var yMax = map.Keys.Max(node => node.Y);

        var node = Node.StartNode();
        while (node.Y < yMax || floor.HasValue)
        {
            if (floor.HasValue && (
                    node.Down.Y == floor
                    || node.Left.Y == floor
                    || node.Right.Y == floor))

            {
                map.Add(node, "O");
                return true;
            }
            else if (!map.ContainsKey(node.Down))
            {
                node = node.Down;
            }
            else if (!map.ContainsKey(node.Left))
            {
                node = node.Left;
            }
            else if (!map.ContainsKey(node.Right))
            {
                node = node.Right;
            }
            else
            {
                if (map.ContainsKey(node) && map[node] == "+")
                {
                    map[node] = "O";
                    return false;
                }
                else
                    map.Add(node, "O");
                return true;
            }
        }

        return false;
    }

    [Test]
    public void Example()
    {
        var map = ParseInput(_sample);
        map.Add(Node.StartNode(), "+");

        Print(map);

        while (Drip(map))
        {
            Print(map);
        }

        Print(map);
        map.Values.Count(x => x == "O").ShouldBe(24);
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
        var map = ParseInput(list);
        map.Add(Node.StartNode(), "+");

        Print(map);

        while (Drip(map))
        {
            
        }

        Print(map);
        map.Values.Count(x => x == "O").ShouldBe(1072);
    }

    [Test]
    public void ExamplePart2()
    {
        var map = ParseInput(_sample);
        map.Add(Node.StartNode(), "+");
        var yMax = map.Keys.Max(node => node.Y) + 2;
        Print(map);
        var counter = 0;
        while (Drip(map, yMax))
        {
            Print(map);
            counter++;
        }

        Print(map);
        map.Values.Count(x => x == "O").ShouldBe(93);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        var map = ParseInput(list);
        map.Add(Node.StartNode(), "+");
        var yMax = map.Keys.Max(node => node.Y) + 2;

        Print(map);

        while (Drip(map, yMax))
        {

        }

        Print(map);
        map.Values.Count(x => x == "O").ShouldBe(24659);

    }
}