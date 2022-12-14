using System.ComponentModel;

namespace aoc2022.Day12;

[TestFixture]
internal class TestsBFS
{
    private readonly List<string> _sample = new()
    {
        "Sabqponm",
        "abcryxxl",
        "accszExk",
        "acctuvwj",
        "abdefghi"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day12");

    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Height { get; set; }

        public Node? Up { get; set; }
        public Node? Down { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public List<Node> Neighbors
        {
            get
            {
                var list = new List<Node>();
                if (Up != null) list.Add(Up);
                if (Down != null) list.Add(Down);
                if (Left != null) list.Add(Left);
                if (Right != null) list.Add(Right);
                return list;
            }
        }

        public Node(int x, int y, char height)
        {
            X = x; Y = y; Height = height;
        }

        public override string ToString() => $"{X},{Y}";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is not Node node) return false;

            return node.X == X && node.Y==Y;
        }
    }

    private static (Node start, Node end, List<Node> nodes) GenerateTheMatrix(List<string> input)
    {
        var xMax = input.First().Length;
        var yMax = input.Count;
        var nodes = new List<Node>();
        Node? start = null, end = null;

        Node? upNeighbor = null;
        for (var x = 0; x < xMax; x++)
        {
            Node? leftNeighbor = null;
            for (var y = 0; y < yMax; y++)
            {
                var isStart = false;
                var isEnd = false;
                var height = input[y][x];

                if (height == 'S')
                {
                    height = 'a';
                    isStart = true;
                }
                else if (height == 'E')
                {
                    height = 'z';
                    isEnd = true;
                }

                var node = new Node(x, y, height)
                {
                    Up = upNeighbor,
                    Left = leftNeighbor
                };

                if (isStart) start = node;
                if (isEnd) end = node;

                if (upNeighbor != null) upNeighbor.Down = node;
                if (leftNeighbor != null) leftNeighbor.Right = node;

                leftNeighbor = node;

                nodes.Add(node);

                upNeighbor = upNeighbor?.Right;
            }

            upNeighbor = nodes.First(node => node.X == x && node.Y == 0);
        }
        return (start!, end!, nodes);
    }

    public List<Node> Walk(Node start, Node end)
    {
        var queue = new Queue<Node>();
        queue.Enqueue(start);
        
        var from = new Dictionary<Node, Node?> { { start, null } };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current.Equals(end))
                break;

            foreach (var neighbor in current.Neighbors.Where(x => x.Height - current.Height <= 1 && !from.ContainsKey(x)).ToList())
            {
                queue.Enqueue(neighbor);
                from.Add(neighbor, current);
            }
        }

        if (from.ContainsKey(end))
        {
            var nodes = new List<Node> { end };
            var previous = from[end];
            while (previous != null && !previous.Equals(start))
            {
                nodes.Add(previous);
                previous = from[previous];
            }

            return nodes;
        }

        return new List<Node>(queue);
    }


    [Test]
    public void Example()
    {
        var (start, end, nodes) = GenerateTheMatrix(_sample);
        start.ShouldNotBeNull();
        end.ShouldNotBeNull();
        nodes.Count.ShouldBe(40);

        var path = Walk(start, end);
        path.Count.ShouldBe(31);
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
        var (start, end, nodes) = GenerateTheMatrix(list);
        var path = Walk(start, end);
        path.Count.ShouldBe(412);
    }

    [Test]
    public void ExamplePart2()
    {
        var (start, end, nodes) = GenerateTheMatrix(_sample);
        var shortPathToAnA = new List<Node>();
        foreach (var node in nodes.Where(x => x.Height == 'a'))
        {
            var path = Walk(node, end);
            if (path.Count > 0 && (path.Count < shortPathToAnA.Count || shortPathToAnA.Count == 0))
            {
                shortPathToAnA = path;
            }
        }
        
        shortPathToAnA.Count.ShouldBe(29);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        var (start, end, nodes) = GenerateTheMatrix(list);

        var shortPathToAnA = new List<Node>();
        var aNodes = nodes.Where(x => x.Height == 'a').ToList();
        foreach (var node in aNodes)
        {
            var path = Walk(node, end);
            if (path.Count > 0 && (path.Count < shortPathToAnA.Count || shortPathToAnA.Count == 0))
            {
                shortPathToAnA = path;
            }
        }

        shortPathToAnA.Count.ShouldBe(402);
    }
}