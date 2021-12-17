using Spectre.Console;

var filepath = "input.txt";
var numberOfPasses = 100;

using var file = File.OpenRead(filepath);
using var reader = new StreamReader(file);

var grid = new int[10,10];

for (int i = 0; i < 10; i++)
{
    if(await reader.ReadLineAsync() is string line)
        foreach ((char ch, int j) in line.Select((c, x) => (c,x)))
        {
            grid[i,j] = int.Parse(ch.ToString());
        }
}

var table = new Table()
{
    Border = TableBorder.None,
    ShowHeaders = false,
    Expand = false
};

table.AddColumns(Enumerable.Range(0,10).Select(_ => new TableColumn(_.ToString())).ToArray());

for (int i = 0; i < 10; i++)
{
    table.AddEmptyRow();
    for (int j = 0; j < 10; j++)
    {
        table.UpdateCell(i, j, new Markup(grid[i,j].ToString(), new Style(decoration: Decoration.Dim)));
    }
}

var flashed = 0;
var synced = false;
int step = 0;
await AnsiConsole.Live(table)
.StartAsync(async ctx => {
    // for (int pass = 0; pass < numberOfPasses; pass++) part 1
    while (!synced)
    {
        step++;
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                if (++grid[i,j] >= 10)
                {
                    UpdateSurrounding(i, j);
                }
            }

        // reset and count greater x / 10 > 0
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                if (grid[i,j] >= 10)
                {
                    if (step < numberOfPasses) // Keep the part 1 answer
                        flashed++;

                    grid[i,j] = 0;
                    table.UpdateCell(i, j, new Markup(grid[i,j].ToString(), new Style(decoration: Decoration.Bold)));
                }
                else
                    table.UpdateCell(i, j, new Markup(grid[i,j].ToString(), new Style(decoration: Decoration.Dim)));
            }
        
        synced = grid.Cast<int>().All(x => x == 0);

        ctx.Refresh();
        await Task.Delay(200);
    }
});

AnsiConsole.WriteLine($"Part 1: {flashed}");
AnsiConsole.WriteLine($"Part 2: {step}");

void UpdateSurrounding(int row, int col){
    if (grid[row,col] > 10)
        // Already Flashed, no need to update again
        return;


    // Update everything around it
    var top = row - 1;
    var right = col + 1;
    var bottom = row + 1;
    var left = col - 1;

    // top-left
    if (top >= 0 && left >= 0)
        if(++grid[top, left] >= 10) UpdateSurrounding(top, left);

    // Top
    if (top >= 0)
        if (++grid[top, col] >= 10) UpdateSurrounding(top, col);

    // Top-Right
    if (top >= 0 && right < 10)
        if(++grid[top, right] >= 10) UpdateSurrounding(top, right);
        
    // right
    if (right < 10)
        if(++grid[row, right] >= 10) UpdateSurrounding(row, right);

    // bottom-right
    if (bottom < 10 && right < 10)
        if(++grid[bottom, right] >= 10) UpdateSurrounding(bottom, right);

    //bottom
    if (bottom < 10)
     if(++grid[bottom, col] >= 10) UpdateSurrounding(bottom, col);

    // bottom-left
    if (bottom < 10 && left >= 0)
        if (++grid[bottom, left] >= 10) UpdateSurrounding(bottom, left);

    // left
    if (left >= 0)
        if (++grid[row, left] >= 10) UpdateSurrounding(row, left);
}