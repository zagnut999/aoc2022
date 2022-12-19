namespace aoc2022.Day18;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "2,2,2",
        "1,2,2",
        "3,2,2",
        "2,1,2",
        "2,3,2",
        "2,2,1",
        "2,2,3",
        "2,2,4",
        "2,2,6",
        "1,2,5",
        "3,2,5",
        "2,1,5",
        "2,3,5"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day18");

    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Node(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }

        public bool IsNeighbor(Node node)
        {
            if (node.X == X && node.Y == Y && Math.Abs(node.Z - Z) == 1) return true;
            if (node.X == X && node.Z == Z && Math.Abs(node.Y - Y) == 1) return true;
            if (node.Y == Y && node.Z == Z && Math.Abs(node.X - X) == 1) return true;
            return false;
        }

        public bool CanSee(Node node)
        {
            if (node.X == X && node.Y == Y) return true;
            if (node.X == X && node.Z == Z) return true;
            if (node.Y == Y && node.Z == Z) return true;
            return false;
        }

        public override string ToString() => $"{X},{Y},{Z}";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is not Node node) return false;

            return node.X == X && node.Y == Y && node.Z == Z;
        }
    }

    List<Node> ParseInput(List<string> input)
    {
        var result = new List<Node>();
        foreach (var line in input)
        {
            var parts = line.Split(',');
            result.Add(new Node(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
        }
        return result;
    }

    [Test]
    public void Example()
    {
        var nodes = ParseInput(_sample);
        var sum = nodes.Sum(x => 6 - nodes.Where(node => !node.Equals(x)).Count(node => node.IsNeighbor(x)));
        sum.ShouldBe(64);
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
        var nodes = ParseInput(list);
        var sum = nodes.Sum(x => 6 - nodes.Where(node => !node.Equals(x)).Count(node => node.IsNeighbor(x)));
        sum.ShouldBe(3564);
    }

    [Test]
    [Ignore("DNF")]
    public void ExamplePart2()
    {
        var nodes = ParseInput(_sample);
        var sum = nodes.Sum(x => 6 - nodes.Where(node => !node.Equals(x)).Count(node => node.IsNeighbor(x)));
        
        sum.ShouldBe(58);
    }

    [Test]
    [Ignore("DNF")]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();

    }
}