namespace aoc2022.Day12;

[TestFixture]
internal class TestsDFS
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

    // State:
    // Start, End, Current
    // Visited, Score
    // Choices

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

    public IReadOnlyList<Node> StartWalk(Node start, Node end)
    {
        var list =  Walk(start, end, new List<Node>().AsReadOnly());

        return list.Where(x => x.finished).Select(x => x.path).MinBy(x => x.Count) ?? new List<Node>().AsReadOnly();
    }

    private List<(IReadOnlyList<Node> path, bool finished)> Walk(Node current, Node end, IReadOnlyList<Node> visited)
    {
        if (current.Equals(end))
            return new() { (visited, true) };

        var options = current.Neighbors.Where(x => x.Height - current.Height <= 1 && !visited.Contains(x)).ToList();
        if (!options.Any())
            return new () {(visited, false)};

        var results = new List<(IReadOnlyList<Node> Visited, bool Finished)>();
        foreach (var next in options)
            results.AddRange(Walk(next, end, visited.Add(next)));

        return results.Where(x=>x.Finished).ToList();
    }


    [Test]
    public void Example()
    {
        var (start, end, nodes) = GenerateTheMatrix(_sample);
        start.ShouldNotBeNull();
        end.ShouldNotBeNull();
        nodes.Count.ShouldBe(40);

        var path = StartWalk(start, end);
        path.Count.ShouldBe(31);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();
    }

    [Test]
    [Ignore("doesn't work")]
    public async Task Actual()
    {
        var list = await ReadInputFile();
        var (start, end, nodes) = GenerateTheMatrix(list);
        var path = StartWalk(start, end);
        path.Count.ShouldBe(31);
    }

    [Test]
    [Ignore("DNF")]
    public void ExamplePart2()
    {
    }

    [Test]
    [Ignore("DNF")]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();

    }
}