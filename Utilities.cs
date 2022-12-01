namespace aoc2022;

internal static class Utilities
{
    public static async Task<List<string>> ReadInput(string path)
    {
        return (await File.ReadAllLinesAsync(path)).ToList();
    }
    
    public static async Task<List<string>> ReadInputByDay(string day)
    {
        var path = $"../../../{day}/input.txt";

        if (File.Exists(path))
        {
            var list = await File.ReadAllLinesAsync(path);
            return list.ToList();
        }

        return new List<string>();
    }
}
