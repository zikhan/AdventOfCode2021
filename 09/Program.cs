using System.Collections.Concurrent;
using Spectre.Console;

var filepath = "input.txt";

using var file = File.OpenRead(filepath);
using var reader = new StreamReader(file);

var grid = new List<List<int>>();
var line = "";

while ((line = await reader.ReadLineAsync()) != null)
{
    // Neat from: https://stackoverflow.com/a/239107
    grid.Add(line.Select(c => c - '0').ToList());
}

var riskPointBag = new ConcurrentBag<int>();
var lowPoints = new ConcurrentBag<(int y, int x)>();

Parallel.ForEach(grid, (val, state, i) => {
    if(state.ShouldExitCurrentIteration)
        return;

    foreach ((int v, int j) in val.Select((x,k) => (x, k)))
    {
        // fuck this capture, it may cause a mem overload
        // Is it less then the top number
        if (i - 1 >= 0 && v >= grid[(int)i - 1][j])
                continue;

        // Is it less then the bottom number
        if (i + 1 < grid.Count && v >= grid[(int)i + 1][j])
            continue;

        // Is it less then the left number
        if (j - 1 >= 0 && v >= val[j - 1])
            continue;

        // Is it less then the right number
        if (j + 1 < val.Count && v >= val[j + 1])
            continue;

        // If it made it here, it's the lowest
        riskPointBag.Add(v + 1);
        lowPoints.Add(((int)i, j));
    }
});


// Basin search
// Return: Count of fields included in basin around point
// find all unique points around point that aren't 9 and stop traversing that direction of travel

var table = new Table()
{
    ShowHeaders = false,
    Border = TableBorder.None,
    Expand = true,
    Width = 100
};
table.AddColumns(grid.First().Select((x, i) => new TableColumn(i.ToString()){ Padding = new Padding(0)}).ToArray());

foreach (var row in grid)
{
    table.AddRow(row.Select(x => new Markup(x.ToString(), new Style(decoration: Decoration.Dim))));
}
var basins = new ConcurrentBag<List<(int y, int x)>>();;

AnsiConsole.Live(table)
.AutoClear(true)
.Overflow(VerticalOverflow.Visible)
.Start(ctx => {
    ctx.Refresh();
    var random = new Random();
    Parallel.ForEach(lowPoints, (point, _, _) =>
    {
        var basin = new List<(int y, int x)>();
        var basinColor = Color.FromInt32(random.Next(0, 255));
        FindEmAll(point, basin, basinColor);
        ctx.Refresh();
        basins.Add(basin);
    });

    void FindEmAll((int y, int x) point, List<(int y, int x)> basin, Color basinColor)
    {
        if (!basin.Contains(point))
        {
            basin.Add(point);
            table.UpdateCell(point.y, point.x, new Markup($"[black on white]{grid[point.y][point.x]}[/]"));
            ctx.Refresh();
        }
        else{
            return;
        }

        if (point.y - 1 >= 0 && grid[point.y - 1][point.x] != 9) // Can search top
            FindEmAll((point.y - 1, point.x), basin, basinColor);
        if (point.y + 1 < grid.Count && grid[point.y + 1][point.x] != 9) // Can search bottom
            FindEmAll((point.y + 1, point.x), basin, basinColor);

        if (point.x - 1 >= 0 && grid[point.y][point.x - 1] != 9) // Can search left
            FindEmAll((point.y, point.x - 1), basin, basinColor);
        if (point.x + 1 < grid[point.y].Count && grid[point.y][point.x + 1] != 9) // Can search right
            FindEmAll((point.y, point.x + 1), basin, basinColor);

        // Update Cell to show no longer searching
        table.UpdateCell(point.y, point.x, new Markup($"{grid[point.y][point.x]}", new Style(foreground: basinColor, decoration: Decoration.Bold)));
    }
});

var pt2 = basins.OrderByDescending(x => x.Count).Take(3).Select(x => x.Count).Aggregate((value, agg) => value * agg);

AnsiConsole.WriteLine($"Part 1: {riskPointBag.Sum()}");
AnsiConsole.WriteLine($"Part 2: {pt2}");