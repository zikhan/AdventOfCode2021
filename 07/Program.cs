var filePath = "input.txt";

var positions = (await File.ReadAllLinesAsync(filePath)).SelectMany(x => x.Split(',').Select(int.Parse)).ToList();

var max = positions.Max();
var min = positions.Min();
var totalPositions = max - min;

var grid = new int[totalPositions][];

for(int i = 0; i < totalPositions; i++){
	grid[i] = new int[positions.Count()];
	for(int j = 0; j < positions.Count(); j++){
		var distance = Math.Abs(positions[j] - i);
		grid[i][j] = Enumerable.Range(1,distance).Sum();
	}
}

var minMove = grid.Select(x => x.Sum()).Min();

Console.WriteLine($"Part 2: {minMove}");