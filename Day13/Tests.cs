namespace aoc2022.Day13;

[TestFixture]
internal class Tests
{
    private readonly List<string> _sample = new()
    {
        "[1,1,3,1,1]",
        "[1,1,5,1,1]",
        "",
        "[[1],[2,3,4]]",
        "[[1],4]",
        "",
        "[9]",
        "[[8,7,6]]",
        "",
        "[[4,4],4,4]",
        "[[4,4],4,4,4]",
        "",
        "[7,7,7,7]",
        "[7,7,7]",
        "",
        "[]",
        "[3]",
        "",
        "[[[]]]",
        "[[]]",
        "",
        "[1,[2,[3,[4,[5,6,7]]]],8,9]",
        "[1,[2,[3,[4,[5,6,0]]]],8,9]"
    };

    private static async Task<List<string>> ReadInputFile() => await Utilities.ReadInputByDay("Day13");

    //private List<(NodeList Left, NodeList Right)> ParseInput(List<string> input)
    //{
    //    var result = new List<(NodeList, NodeList)>();

    //    for (var i = 0; i < input.Count; i += 3)
    //    {
    //        var left = ParseLine(input[i]);
    //        var right = ParseLine(input[i + 1]);
    //        result.Add((left, right));
    //    }

    //    return result;
    //}

    //private NodeList ParseLine(string line)
    //{
    //    var result = new NodeList();

    //    var remaining = line.Substring(1);
    //    var newRemaining = Deeper(result, remaining);

    //    return result;
    //}

    //private string Deeper(NodeList node, string remainingLine)
    //{
    //    var remaining = remainingLine;
    //    if (remaining[0] == ']')
    //        return (remaining.Substring(1));

        
    //    if (remaining[0] == '[')
    //    {
    //        var newList = new NodeList();
    //        node.List!.Add(newList);
    //        remaining = Deeper(newList, remaining.Substring(1));
    //    }
    //    return remaining;
    //}

    //private interface INode { }

    //private class NodeNumber : INode
    //{
    //    public int Number { get; set; }
    //}

    //private class NodeList : INode
    //{
    //    public List<INode> List { get; set; } = new List<INode>();
    //}

    //private bool Compare(string left, string right)
    //{
    //    int leftIndex = 0, rightIndex = 0;
    //    int leftDepth = 0, rightDepth = 0;
    //    char leftCurrent, rightCurrent;
    //    do
    //    {
    //        if (left.Length > leftIndex)
    //            leftCurrent = left[leftIndex];
    //        else
    //            return true;

    //        if (right.Length > rightIndex)
    //            rightCurrent = right[rightIndex];
    //        else
    //            return false;




    //    } while (1 == 1);


    //    return false;
    //}

    [Test]
    public void Example()
    {
        //var input = ParseInput(_sample);

        //var result = new List<bool>();
        //foreach (var x in input)
        //{
        //    result.Add(Compare(x.Left, x.Right));
        //}

        //result.Select((x,index) => x ? index + 1 : 0).Sum().ShouldBe(13);
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
        list.ShouldNotBeEmpty();
    }

    [Test]
    public void ExamplePart2()
    {
    }

    [Test]
    public async Task ActualPart2()
    {
        var list = await ReadInputFile();
        list.ShouldNotBeEmpty();

    }
}