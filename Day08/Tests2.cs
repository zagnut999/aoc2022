﻿namespace aoc2022.Day08;

[TestFixture]
internal class Tests2
{
    private readonly List<string> _sample = new()
    {
        "30373",
        "25512",
        "65332",
        "33549",
        "35390"
    };

    public class Node
    {
        public Node(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public int Height { get; set; }

        public int X { get; }
        public int Y { get; }

        public Node? Up { get; set; }
        public Node? Down { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public bool Visible =>
            (Up == null || Down == null || Left == null || Right == null) 
            || CheckVisible(Up, NextUp) 
            || CheckVisible(Down, NextDown)
            || CheckVisible(Left, NextLeft)
            || CheckVisible(Right, NextRight);

        
        public int TreesSeen
        {
            get
            {
                var up = CheckTreesSeen(Up, NextUp);
                var down = CheckTreesSeen(Down, NextDown);
                var left = CheckTreesSeen(Left, NextLeft);
                var right = CheckTreesSeen(Right, NextRight);
                return up * down * left * right;
            }
        }
        public override string ToString()
        {
            return $"{X},{Y}: {Height} {Visible} {TreesSeen}";
        }

        private bool CheckVisible(Node? node, Func<Node, Node?> next)
        {
            if (node == null) return true;
            if (node.Height >= Height) return false;

            return CheckVisible(next(node), next);
        }
        
        private int CheckTreesSeen(Node? node, Func<Node, Node?> next)
        {
            if (node == null) return 0;
            if (node.Height >= Height) return 1;

            return 1 + CheckTreesSeen(next(node), next);
        }

        private static readonly Func<Node, Node?> NextUp = x => x.Up;
        private static readonly Func<Node, Node?> NextDown = x => x.Down;
        private static readonly Func<Node, Node?> NextLeft = x => x.Left;
        private static readonly Func<Node, Node?> NextRight = x => x.Right;
    }

    private static (int xMax, int yMax, List<Node> nodes) GenerateTheMatrix(List<string> input)
    {
        var xMax = input.First().Length;
        var yMax = input.Count;
        var nodes = new List<Node>();

        Node? upNeighbor = null;
        for (var x = 0; x < xMax; x++)
        {
            Node? leftNeighbor = null;
            for (var y = 0; y < yMax; y++)
            {
                var height = int.Parse(input[y][x].ToString());
                var node = new Node(x, y, height)
                {
                    Up = upNeighbor,
                    Left = leftNeighbor
                };

                if (upNeighbor != null) upNeighbor.Down = node;
                if (leftNeighbor != null) leftNeighbor.Right = node;

                leftNeighbor = node;
                
                nodes.Add(node);

                upNeighbor = upNeighbor?.Right;
            }

            upNeighbor = nodes.First(node => node.X == x && node.Y == 0);
        }
        return (xMax, yMax, nodes);
    }

    public int NumberVisible(List<string> list)
    {
        var (xMax, yMax, nodes) = GenerateTheMatrix(list);

        PrintMatrix("Final", xMax, yMax, nodes);

        return nodes.Count(x => x.Visible);
    }

    private void PrintMatrix(string label, int xMax, int yMax, List<Node> nodes)
    {
        Console.WriteLine(label);
        for (var x = 0; x < xMax; x++)
        {
            for (var y = 0; y < yMax; y++)
            {
                var node = nodes.First(node => node.X == x && node.Y == y);
                Console.Write(node.Visible ? "O" : "x");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
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
        var (_, _, nodes) = GenerateTheMatrix(list);

        return nodes.Max(x => x.TreesSeen);
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