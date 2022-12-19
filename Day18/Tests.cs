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

    private List<Node> Neighbors(Node node)
    {
        return new List<Node>
        {
            new(node.X, node.Y, node.Z - 1),
            new(node.X, node.Y, node.Z + 1),
            new(node.X, node.Y - 1, node.Z),
            new(node.X, node.Y + 1, node.Z),
            new(node.X - 1, node.Y, node.Z),
            new(node.X + 1, node.Y, node.Z)
        };
    }

    [Test]
    public void ExamplePart2()
    {
        var nodes = ParseInput(_sample);

        //Search area
        var minX = nodes.Min(node => node.X) - 1;
        var minY = nodes.Min(node => node.Y) - 1;
        var minZ = nodes.Min(node => node.Z) - 1;
        var maxX = nodes.Max(node => node.X) + 1;
        var maxY = nodes.Max(node => node.Y) + 1;
        var maxZ = nodes.Max(node => node.Z) + 1;

        var visited = new List<Node>();
        var queue = new Queue<Node>();
        var touched = new Dictionary<Node, int>();
        nodes.ForEach(node => touched.Add(node, 0));

        var start = new Node(minX, minY, minZ);
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            visited.Add(current);

            var neighbors = Neighbors(current);

            var neighborNodes = neighbors.Where(node => touched.ContainsKey(node)).ToList();
            foreach (var neighbor in neighborNodes)
            {
                touched[neighbor]++;
            }

            //Add neighbors
            foreach (var neighbor in neighbors)
            {
                if (queue.Contains(neighbor))
                    continue;
                if (visited.Contains(neighbor))
                    continue;
                if (touched.ContainsKey(neighbor))
                    continue;
                if (neighbor.X < minX || neighbor.X > maxX 
                    || neighbor.Y < minY || neighbor.Y > maxY 
                    || neighbor.Z < minZ || neighbor.Z > maxZ)
                {
                    visited.Add(neighbor); // maybe not needed
                    continue;
                }

                queue.Enqueue(neighbor);
            }
        }

        touched.Values.Sum(x => x).ShouldBe(58);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        var nodes = ParseInput(list);
        //Search area
        var minX = nodes.Min(node => node.X) - 1;
        var minY = nodes.Min(node => node.Y) - 1;
        var minZ = nodes.Min(node => node.Z) - 1;
        var maxX = nodes.Max(node => node.X) + 1;
        var maxY = nodes.Max(node => node.Y) + 1;
        var maxZ = nodes.Max(node => node.Z) + 1;

        var visited = new List<Node>();
        var queue = new Queue<Node>();
        var touched = new Dictionary<Node, int>();
        nodes.ForEach(node => touched.Add(node, 0));

        var start = new Node(minX, minY, minZ);
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            visited.Add(current);

            var neighbors = Neighbors(current);

            var neighborNodes = neighbors.Where(node => touched.ContainsKey(node)).ToList();
            foreach (var neighbor in neighborNodes)
            {
                touched[neighbor]++;
            }

            //Add neighbors
            foreach (var neighbor in neighbors)
            {
                if (queue.Contains(neighbor))
                    continue;
                if (visited.Contains(neighbor))
                    continue;
                if (touched.ContainsKey(neighbor))
                    continue;
                if (neighbor.X < minX || neighbor.X > maxX
                                      || neighbor.Y < minY || neighbor.Y > maxY
                                      || neighbor.Z < minZ || neighbor.Z > maxZ)
                {
                    visited.Add(neighbor); // maybe not needed
                    continue;
                }

                queue.Enqueue(neighbor);
            }
        }

        touched.Values.Sum(x => x).ShouldBe(2106);

    }
}