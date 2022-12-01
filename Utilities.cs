namespace aoc2022;

internal static class Utilities
{
    internal static List<string> ReadList(string path)
    {
        return File.ReadLines(path).ToList();
    }
}
