using NUnit.Framework.Interfaces;

namespace aoc2022.Day08;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new List<string>()
    {
        "30373",
        "25512",
        "65332",
        "33549",
        "35390"
    };

    public class Node
    {
        public bool Visible { get; set; }
        public int TreesSeen { get; set; }


        public int X { get; }
        public int Y { get; }
        public int Height { get; set; }

        public Node(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public override string ToString()
        {
            return $"{X},{Y}: {Height} {Visible} {TreesSeen}";
        }
    }

    private static void GenerateTheMatrix(List<string> input, out int xMax, out int yMax, out Node[,] nodesMatrix, out List<Node> nodes)
    {
        xMax = input.First().Length;
        yMax = input.Count;
        nodesMatrix = new Node[xMax, yMax];
        nodes = new List<Node>();

        for (var x = 0; x < xMax; x++)
        {
            for (var y = 0; y < yMax; y++)
            {
                var height = int.Parse(input[y][x].ToString());
                var node = new Node(x, y, height);
                nodesMatrix[x, y] = node;
                nodes.Add(node);
            }
        }
    }

    //Worst ever
    public int NumberVisible(List<string> list)
    {
        GenerateTheMatrix(list, out var xMax, out var yMax, out var nodesMatrix, out var nodes);

        //Edges
        for (var x = 0; x < xMax; x++)
        {
            nodesMatrix[x, 0].Visible = true;
            nodesMatrix[x, yMax-1].Visible = true;
        }

        for (var y = 0; y < yMax; y++)
        {
            nodesMatrix[0, y].Visible = true;
            nodesMatrix[xMax - 1, y].Visible = true;
        }

        //Sweep left to right
        for (var x = 0; x < xMax; x++)
        {
            SweepUpAndDown(yMax, nodesMatrix, x);
            SweepUpAndDown(yMax, nodesMatrix, xMax - 1 - x);
        }

        //Sweep top to bottom
        for (var y = 0; y < yMax; y++)
        {
            SweepLeftToRight(xMax, nodesMatrix, y);
            SweepLeftToRight(xMax, nodesMatrix, yMax - 1 - y);
        }

        PrintMatrix("Final", xMax, yMax, nodesMatrix);

        return nodes.Count(x=>x.Visible);
    }

    private void SweepLeftToRight(int xMax, Node[,] nodesMatrix, int y)
    {
        var maxHeight = 0;
        for (var x = 0; x < xMax; x++)
        {
            if (maxHeight == 10) continue;

            var node = nodesMatrix[x, y];
            if (node.Height > maxHeight)
            {
                node.Visible = true;
                maxHeight = node.Height;
            }
        }

        maxHeight = 0;
        for (var x = xMax - 1; x >= 0; x--)
        {
            if (maxHeight == 10) continue;

            var node = nodesMatrix[x, y];
            if (node.Height > maxHeight)
            {
                node.Visible = true;
                maxHeight = node.Height;
            }
        }
    }

    private void PrintMatrix(string label, int xMax, int yMax, Node[,] nodesMatrix)
    {
        Console.WriteLine(label);
        for (int x = 0; x < xMax; x++)
        {
            for (int y = 0; y < yMax; y++)
            {
                var node = nodesMatrix[x, y];
                Console.Write(node.Visible ? "O" : "x");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private static void SweepUpAndDown(int yMax, Node[,] nodesMatrix, int x)
    {
        var maxHeight = 0;
        for (var y = 0; y < yMax; y++)
        {
            if (maxHeight == 10) continue;

            var node = nodesMatrix[x, y];
            if (node.Height > maxHeight)
            {
                node.Visible = true;
                maxHeight = node.Height;
            }
        }

        maxHeight = 0;
        for (var y = yMax - 1; y >= 0; y--)
        {
            if (maxHeight == 10) continue;

            var node = nodesMatrix[x, y];
            if (node.Height > maxHeight)
            {
                node.Visible = true;
                maxHeight = node.Height;
            }
        }
    }

    [Test]
    public void Example()
    {
        NumberVisible(_sample).ShouldBe(21);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day08");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day08");
        NumberVisible(list).ShouldBe(1798);
    }

    //Worst ever
    public int NumberNeighborsVisible(List<string> list)
    {
        GenerateTheMatrix(list, out var xMax, out var yMax, out var nodesMatrix, out var nodes);

        

        PrintMatrix("Final", xMax, yMax, nodesMatrix);

        return nodes.Count(x => x.Visible);
    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
    }
}