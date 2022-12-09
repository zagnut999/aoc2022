namespace aoc2022.Day09;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "R 4",
        "U 4",
        "L 3",
        "D 1",
        "R 4",
        "D 1",
        "L 5",
        "R 2"
    };

    private (Direction direction, int distance) ParseInput(string line)
    {
        var entry = line.Split(" ");
        var direction = entry[0];
        var distance = int.Parse(entry[1]);

        return (GetDirection(direction), distance);
    }

    private enum Direction { U, D, L, R }
    private Direction GetDirection(string name) => (Direction)Enum.Parse(typeof(Direction), name);

    [Test]
    public void TestDirection()
    {
        GetDirection("U").ShouldBe(Direction.U);
        GetDirection("D").ShouldBe(Direction.D);
        GetDirection("L").ShouldBe(Direction.L);
        GetDirection("R").ShouldBe(Direction.R);
    }

    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Location(Location location)
        {
            X = location.X;
            Y = location.Y;
        }

        public (int diffX, int diffY) DistanceFrom(Location location)
        {
            var diffX = location.X - X;
            var diffY = location.Y - Y;

            return (diffX, diffY);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Location location) return false;

            return X == location.X && Y == location.Y;
        }

        public override int GetHashCode()
        {
            return base.ToString()!.GetHashCode();
        }
    }

    private class Node
    {
        private readonly Location _location;
        private readonly List<Location> _visited = new();

        public int X
        {
            get => _location.X;
            private set => _location.X = value;
        }

        public int Y {
            get => _location.Y;
            private set => _location.Y = value;
        }
        public int ID { get; }

        public IReadOnlyList<Location> Visited => _visited.AsReadOnly();

        public Node(int x, int y, int id = 0)
        {
            _location = new Location(x, y);
            AddedVisited(_location);
            ID = id;
        }

        public void Step(Direction direction)
        {
            Move(direction);

            AddedVisited(_location);
        }

        private void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.U:
                    Y += 1;
                    break;
                case Direction.D:
                    Y -= 1;
                    break;
                case Direction.L:
                    X -= 1;
                    break;
                case Direction.R:
                    X += 1;
                    break;
            }
        }

        public void StepDiagonally(Direction first, Direction second)
        {
            Move(first);
            Move(second);
            AddedVisited(_location);
        }

        public (int diffX, int diffY) DistanceFrom(Node node)
        {
            return _location.DistanceFrom(node._location);
        }

        public override string ToString()
        {
            return $"{ID} ({X},{Y})";
        }

        private void AddedVisited(Location location)
        {
            _visited.Add(new Location(location));
        }
    }

    private class State
    {
        private readonly List<Node> _knots = new();
        public Node Head => _knots.First();
        public Node Tail => _knots.Last();

        public State(int numberOfKnots = 2)
        {
            for (var i = 0; i < numberOfKnots; i++)
            {
                _knots.Add(new Node(0, 0, i));
            }
        }

        public void MoveHead(Direction direction, int distance)
        {
            for (var i = 0; i < distance; i++)
            {
                Head.Step(direction);
                for (var j = 1; j < _knots.Count; j++)
                {
                    MoveTailIfNeeded(_knots[j - 1], _knots[j]);
                }
            }
        }

        private void MoveTailIfNeeded(Node head, Node tail)
        {
            var (diffX, diffY) = tail.DistanceFrom(head);

            // don't move
            if (Math.Abs(diffX) <= 1 && Math.Abs(diffY) <= 1)
                return;

            // move up or down
            if (diffX == 0)
                tail.Step(diffY > 1 ? Direction.U : Direction.D);

            // move left or right
            else if (diffY == 0)
                tail.Step(diffX > 1 ? Direction.R : Direction.L);

            //move diagonally
            else
            {
                var stepX = diffX < 0 ? Direction.L : Direction.R;
                var stepY = diffY < 0 ? Direction.D : Direction.U;
                tail.StepDiagonally(stepX, stepY);
            }
        }
    }

    [Test]
    public void Walk_Up()
    {
        var line = "U 4";
        var state = new State();
        var (direction, distance) = ParseInput(line);
        state.MoveHead(direction, distance);

        state.Tail.Y.ShouldBe(3);
        state.Head.Y.ShouldBe(4);
    }

    [Test]
    public void Walk_Down()
    {
        var line = "D 4";
        var state = new State();
        var (direction, distance) = ParseInput(line);
        state.MoveHead(direction, distance);

        state.Tail.Y.ShouldBe(-3);
        state.Head.Y.ShouldBe(-4);
    }

    [Test]
    public void Walk_Left()
    {
        var line = "L 4";
        var state = new State();
        var (direction, distance) = ParseInput(line);
        state.MoveHead(direction, distance);

        state.Tail.X.ShouldBe(-3);
        state.Head.X.ShouldBe(-4);
    }

    [Test]
    public void Walk_Right()
    {
        var line = "R 4";
        var state = new State();
        var (direction, distance) = ParseInput(line);
        state.MoveHead(direction, distance);

        state.Tail.X.ShouldBe(3);
        state.Head.X.ShouldBe(4);
    }

    [Test]
    public void Walk_DoNotMove()
    {
        var line = "R 1";
        var state = new State();
        var (direction, distance) = ParseInput(line);
        state.MoveHead(direction, distance);

        state.Tail.X.ShouldBe(0);
        state.Head.X.ShouldBe(1);
    }

    [Test]
    public void Walk_DiagUpRight()
    {
        var lines = new List<string>
        {
            "R 1",
            "U 1",
            "R 1"
        };
        var state = new State();
        foreach (var line in lines)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        state.Tail.X.ShouldBe(1);
        state.Tail.Y.ShouldBe(1);
        state.Head.X.ShouldBe(2);
        state.Head.Y.ShouldBe(1);
    }
    [Test]
    public void Walk_DiagDownRight()
    {
        var lines = new List<string>
        {
            "R 1",
            "D 1",
            "R 1"
        };
        var state = new State();
        foreach (var line in lines)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        state.Tail.X.ShouldBe(1);
        state.Tail.Y.ShouldBe(-1);
        state.Head.X.ShouldBe(2);
        state.Head.Y.ShouldBe(-1);
    }

    [Test]
    public void Walk_DiagUpLeft()
    {
        var lines = new List<string>
        {
            "L 1",
            "U 1",
            "L 1"
        };
        var state = new State();
        foreach (var line in lines)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        state.Tail.X.ShouldBe(-1);
        state.Tail.Y.ShouldBe(1);
        state.Head.X.ShouldBe(-2);
        state.Head.Y.ShouldBe(1);
    }

    [Test]
    public void Walk_DiagDownLeft()
    {
        var lines = new List<string>
        {
            "L 1",
            "D 1",
            "L 1"
        };
        var state = new State();
        foreach (var line in lines)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        state.Tail.X.ShouldBe(-1);
        state.Tail.Y.ShouldBe(-1);
        state.Head.X.ShouldBe(-2);
        state.Head.Y.ShouldBe(-1);
        state.Tail.Visited.Distinct().Count().ShouldBe(2);
    }

    [Test]
    public void Example()
    {
        var state = new State();
        foreach (var line in _sample)
        {
            var (direction, distance)= ParseInput(line);
            state.MoveHead(direction, distance);
        }
        var uniqueVisits = state.Tail.Visited.Distinct().ToList();
        foreach (var location in uniqueVisits)
        {
            Console.WriteLine(location);
        }
        uniqueVisits.Count().ShouldBe(13);
    }

    [Test]
    public async Task ReadDaysFile()
    {
        var list = await Utilities.ReadInputByDay("Day09");
        list.ShouldNotBeEmpty();
    }

    [Test]
    public async Task Actual()
    {
        var list = await Utilities.ReadInputByDay("Day09");
        var state = new State();
        foreach (var line in list)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        var uniqueVisits = state.Tail.Visited.Distinct().ToList();
        uniqueVisits.Count().ShouldBe(6011);
    }
    
    [Test]
    public void ExamplePart2()
    {
        var state = new State(10);
        foreach (var line in _sample)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        var uniqueVisits = state.Tail.Visited.Distinct().ToList();
        foreach (var location in uniqueVisits)
        {
            Console.WriteLine(location);
        }
        uniqueVisits.Count().ShouldBe(1);
    }

    [Test]
    public void ExamplePart2a()
    {
        var state = new State(10);
        var lines = new List<string>
        {
            "R 5",
            "U 8",
            "L 8",
            "D 3",
            "R 17",
            "D 10",
            "L 25",
            "U 20"
        };

        foreach (var line in lines)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        var uniqueVisits = state.Tail.Visited.Distinct().ToList();
        foreach (var location in uniqueVisits)
        {
            Console.WriteLine(location);
        }
        uniqueVisits.Count().ShouldBe(36);
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await Utilities.ReadInputByDay("Day09");
        var state = new State(10);
        foreach (var line in list)
        {
            var (direction, distance) = ParseInput(line);
            state.MoveHead(direction, distance);
        }
        var uniqueVisits = state.Tail.Visited.Distinct().ToList();
        uniqueVisits.Count().ShouldBe(2419);
    }
}