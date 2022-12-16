using System.Text.RegularExpressions;
using NUnit.Framework.Constraints;

namespace aoc2022.Day15;

[TestFixture]
[Ignore("DNF")]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "Sensor at x=2, y=18: closest beacon is at x=-2, y=15",
        "Sensor at x=9, y=16: closest beacon is at x=10, y=16",
        "Sensor at x=13, y=2: closest beacon is at x=15, y=3",
        "Sensor at x=12, y=14: closest beacon is at x=10, y=16",
        "Sensor at x=10, y=20: closest beacon is at x=10, y=16",
        "Sensor at x=14, y=17: closest beacon is at x=10, y=16",
        "Sensor at x=8, y=7: closest beacon is at x=2, y=10",
        "Sensor at x=2, y=0: closest beacon is at x=2, y=10",
        "Sensor at x=0, y=11: closest beacon is at x=2, y=10",
        "Sensor at x=20, y=14: closest beacon is at x=25, y=17",
        "Sensor at x=17, y=20: closest beacon is at x=21, y=22",
        "Sensor at x=16, y=7: closest beacon is at x=15, y=3",
        "Sensor at x=14, y=3: closest beacon is at x=15, y=3",
        "Sensor at x=20, y=1: closest beacon is at x=15, y=3"
    };
    
    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day15");

    private List<(Node Sensor, Node Beacon)> ParseInput(List<string> input)
    {
        var result = new List<(Node Sensor, Node Beacon)> ();
        foreach (var line in input)
        {
            result.Add(ParseLine(line));
        }
        return result;
    }

    public static readonly Regex LineRegex = new Regex(@"Sensor at x=(?<sensorX>-?\d*), y=(?<sensorY>-?\d*): closest beacon is at x=(?<beaconX>-?\d*), y=(?<beaconY>-?\d*)");

    [Test]
    public void RegexTest()
    {
        var match = LineRegex.Match(_sample[0]);
        match.Success.ShouldBeTrue();
        match.Groups["sensorX"].Value.ShouldBe("2");
        match.Groups["sensorY"].Value.ShouldBe("18");
        match.Groups["beaconX"].Value.ShouldBe("-2");
        match.Groups["beaconY"].Value.ShouldBe("15");
    }

    private (Node Sensor, Node Beacon) ParseLine(string line)
    {
        var match = LineRegex.Match(line);
        var sensorX = int.Parse(match.Groups["sensorX"].Value);
        var sensorY = int.Parse(match.Groups["sensorY"].Value);
        var beaconX = int.Parse(match.Groups["beaconX"].Value);
        var beaconY = int.Parse(match.Groups["beaconY"].Value);

        return (new Node(sensorX,sensorY), new Node(beaconX,beaconY));
    }

    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Node(int x, int y)
        {
            X = x; Y = y;
        }

        public int DistanceTo(Node other)
        {
            return Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
        }

        public override string ToString() => $"{X},{Y}";

        public override int GetHashCode() => ToString().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is not Node node) return false;

            return node.X == X && node.Y == Y;
        }
    }

    private Dictionary<Node, string> GenerateMap(List<string> input)
    {
        var nodes = ParseInput(input);

        var map = new Dictionary<Node, string>();

        // Add sensors and beacons
        foreach (var (sensor, beacon) in nodes)
        {
            map.Add(sensor, "S");

            if (!map.ContainsKey(beacon))
                map.Add(beacon, "B");

        }

        // Filling
        foreach (var (sensor, beacon) in nodes)
        {
            var distance = sensor.DistanceTo(beacon);

            // This is gross
            for (var x = sensor.X - distance; x < sensor.X + distance; x++)
            {
                for (var y = sensor.Y - distance; y < sensor.Y + distance; y++)
                {
                    var node = new Node(x, y);
                    if (!map.ContainsKey(node) && sensor.DistanceTo(node) <= distance)
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

    private (Dictionary<Node, string> Map, Dictionary<Node, int> Ranges) GenerateMap2(List<string> input)
    {
        var nodes = ParseInput(input);

        var map = new Dictionary<Node, string>();

        // Add sensors and beacons
        foreach (var (sensor, beacon) in nodes)
        {
            map.Add(sensor, "S");

            if (!map.ContainsKey(beacon))
                map.Add(beacon, "B");

        }

        var ranges = new Dictionary<Node, int>();
        foreach (var (sensor, beacon) in nodes)
        {
            ranges.Add(sensor, sensor.DistanceTo(beacon));
        }

        return (map, ranges);
    }

    [Test]
    public void Example()
    {
        //var map = GenerateMap(_sample);
        //Print(map);

        var (map, ranges) = GenerateMap2(_sample);
        var y = 10;
        var maxRange = ranges.Values.Max(); //This could be better

        var xMin = ranges.Keys.Min(node => node.X) - maxRange;
        var xMax = ranges.Keys.Max(node => node.X) + maxRange;

        var count = 0;
        for (var x = xMin; x <= xMax; x++)
        {
            var node = new Node(x, y);

            if (map.ContainsKey(node))
                continue;

            if (ranges.Keys.Any(sensor => sensor.DistanceTo(node) <= ranges[sensor]))
                count++;
        }
        
        count.ShouldBe(26);
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
        var (map, ranges) = GenerateMap2(list);
        var y = 10;
        var maxRange = ranges.Values.Max(); //This could be better

        var xMin = ranges.Keys.Min(node => node.X) - maxRange;
        var xMax = ranges.Keys.Max(node => node.X) + maxRange;

        var count = 0;
        for (var x = xMin; x <= xMax; x++)
        {
            var node = new Node(x, y);

            if (map.ContainsKey(node))
                continue;

            if (ranges.Keys.Any(sensor => sensor.DistanceTo(node) <= ranges[sensor]))
                count++;
        }

        count.ShouldBe(26);
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