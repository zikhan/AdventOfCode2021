using System.Linq;

var filePath = "input.txt";
int? previousDepth = null;
int total = 0;
List<int> depths = new();
await foreach (var depth in GetDepths(filePath))
{
    if (previousDepth is int previous && previous < depth)
        total++;
    previousDepth = depth;
    depths.Add(depth);
}

Console.WriteLine($"Part 1 Total: {total}");

int skip = 0;
int? previousWindow = null;
int totalSlide = 0;
IEnumerable<int>? checks = null;
while ((checks = depths.Skip(skip).Take(3)).Count() == 3)
{
    if (previousWindow is int previous && previous < checks.Sum())
        totalSlide++;
    skip += 1;
    previousWindow = checks.Sum();
}

Console.WriteLine($"Part 2 Total: {totalSlide}");

async IAsyncEnumerable<int> GetDepths(string filePath)
{
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(file);
	string? line;
    while ((line = await reader.ReadLineAsync()) is not null)
        yield return int.Parse(line);
}
