var filePath = "input.txt";

var position = (x: 0, z: 0);
var positionPt2 = (x: 0, z: 0, aim: 0);
List<(string command, int unit)> instructionCache = new();
await foreach(var instruction in GetInstructions(filePath)){
    // Part 1 Rules
    (int x, int z) op = instruction switch {
        ("forward", _) => (instruction.unit, 0),
        ("down", _) => (0, instruction.unit),
        ("up", _) => (0, instruction.unit * -1),
        (_, _) => (0,0) // noop
    };

    position.x += op.x;
    position.z += op.z;

    // Part 2 rules
    (int aim, int forward) op2 = instruction switch {
        ("down", _) => (instruction.unit, 0),
        ("up", _) => (-1 * instruction.unit, 0),
        ("forward", _) => (0, instruction.unit),
        (_, _) => (0,0)
    };

    positionPt2.aim += op2.aim;
    positionPt2.x += op2.forward;
    positionPt2.z += op2.forward * positionPt2.aim;
}

Console.WriteLine($"Part 1: {position.x * position.z}");
Console.WriteLine($"Part 2: {positionPt2.x * positionPt2.z}");

async IAsyncEnumerable<(string command, int unit)> GetInstructions(string filePath)
{
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(file);
    string? line;
    while ((line = await reader.ReadLineAsync()) is not null){
        var vals = line.Split(" ");
        yield return (vals[0], int.Parse(vals[1]));
    }
}