using System.Numerics;

var filePath = "input.txt";

List<(int x, int y)> marks= new();

int maxX = 0;
int maxY = 0;
await foreach (var line in ReadLines(filePath)){
    var point1 = line.Split(" -> ")[0];
    var point2 = line.Split(" -> ")[1];
    var x1 = int.Parse(point1.Split(',')[0]);
    var y1 = int.Parse(point1.Split(',')[1]);
    var x2 = int.Parse(point2.Split(',')[0]);
    var y2 = int.Parse(point2.Split(',')[1]);

    if (x1 == x2) {
        marks.AddRange(Enumerable.Range(Math.Min(y1,y2), Math.Abs(y1 - y2)+1).Select(y => (x1,y)));
    }
    else if (y1 == y2) {
        marks.AddRange(Enumerable.Range(Math.Min(x1,x2), Math.Abs(x1 - x2)+1).Select(x => (x,y1)));
    }
    else { // Diag for part 2
        // calc slope and get points
        (int x, int y) slope = (x2 - x1, y2 - y1);
        slope.x /= Math.Abs(slope.x);
        slope.y /= Math.Abs(slope.y);
        (int x, int y) point = (x1,y1);
        marks.Add(point);
        do
        {
            point.x += slope.x;
            point.y += slope.y;
            marks.Add(point);
        } while (x2 != point.x && y2 != point.y);
    }
    
    maxX = Math.Max(maxX, Math.Max(x1, x2));
    maxY = Math.Max(maxY, Math.Max(y1, y2));
}

int?[,] grid = new int?[maxX+1,maxY+1];
foreach (var mark in marks)
{
    if(grid[mark.x,mark.y] is null)
        grid[mark.x, mark.y] = 1;
    else
        grid[mark.x, mark.y] += 1;
}
var overlapped = 0;
foreach (var number in grid)
    if (number is not null)
        if (number >= 2)
            overlapped++;

Console.WriteLine($"twice overlapped: {overlapped}");

async IAsyncEnumerable<string> ReadLines(string filePath)
{
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(file);
    string? line;
    while((line = await reader.ReadLineAsync()) is not null){
        yield return line;
    }
}
