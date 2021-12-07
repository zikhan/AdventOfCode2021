var filePath = "input.txt";

using var file = File.OpenRead(filePath);
using var reader = new StreamReader(file);

var numbers = (await reader.ReadLineAsync())?.Split(',').Select(int.Parse).ToArray() ?? throw new ArgumentNullException("File");

var bingoCards = new List<BingoCard>();

string? line;
while((line = await reader.ReadLineAsync()) is not null){
    if(string.IsNullOrWhiteSpace(line))
    {
        int?[][] rows = new int?[5][];
        for (int i = 0; i < 5; i++) { // Guarenteed input
            line = (await reader.ReadLineAsync()) ?? throw new ArgumentNullException("No line");
            rows[i] = line.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).Cast<int?>().ToArray();
        }
        bingoCards.Add(new BingoCard(rows));
    }
}

int? part1answer = null;
int? part2answer = null;
foreach (var number in numbers)
{
    foreach (var card in bingoCards)
    {
        if (card.Mark(number)) // if Bingo after mark
        {
            part1answer ??= card.SumUnmarked() * number;
            if (bingoCards.All(x => x.Bingoed))
            {
                // This is the last one
                part2answer = card.SumUnmarked() * number;
            }
        }
    }
    if (part1answer is not null && part2answer is not null)
        break;
}

Console.WriteLine($"Part 1: {part1answer}");
Console.WriteLine($"Part 2: {part2answer}");

public class BingoCard{
    int?[][] Rows;
    int?[][] Columns;

    public bool Bingoed = false;

    public BingoCard(int?[][] rows){
        Rows = rows;
        Columns = new int?[5][];
        for (int i = 0; i < rows.Length; i++)
        {
            Columns[i] = new int?[5];
            for (int j = 0; j < Columns[i].Length; j++)
                Columns[i][j] = Rows[j][i];
        }
    }

    // Returns if bingo
    public bool Mark(int x){
        if (Bingoed) // If bingoed, no need to mark
            return false;

        bool marked = false;
        for (int i = 0; i < Rows.Length; i++)
            for (int j = 0; j < Rows[i].Length; j++)
                if (Rows[i][j] == x) {
                    Rows[i][j] = null;
                    Columns[j][i] = null;
                    marked = true;
                }

        if (marked)
        {
            foreach (var row in Rows)
                if (row.All(x => !x.HasValue)) { Bingoed = true; return true; }
            foreach (var col in Columns)
                if (col.All(x => !x.HasValue)) { Bingoed = true; return true; }
        }
        return false;
    }

    public int SumUnmarked() =>
        Rows.Sum(x => x?.Sum(x => x ?? 0) ?? 0);
}
