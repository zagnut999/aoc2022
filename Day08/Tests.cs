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

    private static (int xMax, int yMax, Node[,] nodesMatrix, List<Node> nodes) GenerateTheMatrix(List<string> input)
    {
        var xMax = input.First().Length;
        var yMax = input.Count;
        var nodesMatrix = new Node[xMax, yMax];
        var nodes = new List<Node>();

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
        return (xMax, yMax, nodesMatrix, nodes);
    }

    //Worst ever
    public int NumberVisible(List<string> list)
    {
        var (xMax, yMax, nodesMatrix, nodes) = GenerateTheMatrix(list);

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

    public int NumberNeighborsVisible(List<string> list)
    {
        var (xMax, yMax, nodesMatrix, nodes) = GenerateTheMatrix(list);

        foreach (var node in nodes)
        {
            FindTreesSeen(node, nodesMatrix, xMax, yMax);
        }

        PrintMatrix("Final", xMax, yMax, nodesMatrix);

        return nodes.Max(x => x.TreesSeen);
    }

    private void FindTreesSeen(Node node, Node[,] nodesMatrix, int xMax, int yMax)
    {
        var upVis = 0;
        //Up (node.y => 0)
        for (var y = node.Y - 1; y >= 0; y--)
        {
            var neighbor = nodesMatrix[node.X, y];
            upVis++;

            if (neighbor.Height >= node.Height)
                break;
        }

        var downVis = 0;
        //Down (node.y => yMax)
        for (var y = node.Y + 1; y < yMax; y++)
        {
            var neighbor = nodesMatrix[node.X, y];
            downVis++;

            if (neighbor.Height >= node.Height)
                break;
        }

        //Left (node.x => 0)
        var leftVis = 0;
        for (var x = node.X - 1; x >= 0; x--)
        {
            var neighbor = nodesMatrix[x, node.Y];
            leftVis++;

            if (neighbor.Height >= node.Height)
                break;
        }

        //Right (node.x => xMax)
        var rightVis = 0;
        for (var x = node.X + 1; x < xMax; x++)
        {
            var neighbor = nodesMatrix[x, node.Y];
            rightVis++;

            if (neighbor.Height >= node.Height)
                break;
        }

        node.TreesSeen = upVis * downVis * rightVis * leftVis;
    }

    [Test]
    public void FindTreesSeenTests()
    {
        var list = new List<string>
        {
            "11111",
            "12221",
            "12321",
            "12231",
            "11111"
        };
        var (xMax, yMax, nodesMatrix, nodes) = GenerateTheMatrix(list);
        var node = nodesMatrix[2, 2];
        FindTreesSeen(node, nodesMatrix, xMax, yMax);
        node.TreesSeen.ShouldBe(16);

        node = nodesMatrix[1, 1];
        FindTreesSeen(node, nodesMatrix, xMax, yMax);
        node.TreesSeen.ShouldBe(1);

        node = nodesMatrix[0, 2];
        FindTreesSeen(node, nodesMatrix, xMax, yMax);
        node.TreesSeen.ShouldBe(0);

        node = nodesMatrix[3, 3];
        FindTreesSeen(node, nodesMatrix, xMax, yMax);
        node.TreesSeen.ShouldBe(9);

        list = new List<string>
        {
            "11111",
            "12241",
            "12321",
            "12231",
            "11111"
        };
        (xMax, yMax, nodesMatrix, nodes) = GenerateTheMatrix(list);
        node = nodesMatrix[3, 3];
        FindTreesSeen(node, nodesMatrix, xMax, yMax);
        node.TreesSeen.ShouldBe(6);
    }

    [Test]
    public void ExamplePart2()
    {
        NumberNeighborsVisible(_sample).ShouldBe(8);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day08");
        NumberNeighborsVisible(list).ShouldBe(259308);
    }
}