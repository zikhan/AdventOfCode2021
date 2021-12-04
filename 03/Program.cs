using System.Collections;

var filePath = "input.txt";

List<int> accumaltiveTotal = new();
int totalrecords = 0;
var numberCache = new List<bool[]>();
await foreach(var number in GetDiagNumbers(filePath)){
    for (int i = 0; i < number.Length; i++)
    {
        if(accumaltiveTotal.Count <= i)
            accumaltiveTotal.Add(0);
        
        if(number[i])
            accumaltiveTotal[i]++;

    }
    numberCache.Add(number);
    totalrecords++;
}

int numberSize = accumaltiveTotal.Count();
BitArray gammaBits = new BitArray(accumaltiveTotal.Select(x => (double)x / totalrecords > 0.5 ? true : false).ToArray());
int gamma = BitArrayToInt(gammaBits);
gammaBits.Not();
int epsilon = BitArrayToInt(gammaBits);

Console.WriteLine($"Part 1: {gamma * epsilon}");

BitArray o2rating = FindRating(true, numberCache);
BitArray co2rating = FindRating(false, numberCache);

var lifeSupportRating = BitArrayToInt(o2rating) * BitArrayToInt(co2rating);

Console.WriteLine($"Part 2: {lifeSupportRating}");

async IAsyncEnumerable<bool[]> GetDiagNumbers(string filePath) {
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(filePath);
    string? line;
    while ((line = await reader.ReadLineAsync()) is not null)
    {
        yield return line.Select(x => x == '1').ToArray();
    }
}

// https://stackoverflow.com/a/5283199
int BitArrayToInt(BitArray ba){
    int[] temp = new int[1];

    if(BitConverter.IsLittleEndian)
        new BitArray(ba.Cast<bool>().Reverse().ToArray()).CopyTo(temp, 0);
    else
        ba.CopyTo(temp, 0);
    return temp[0];
}

BitArray FindRating(bool mostCommon, List<bool[]> diagNumbers, int index = 0){
    if (index >= diagNumbers.First().Count())
        throw new Exception("shit happens");

    // Do most/least common computer again. Screw DRY
    bool compare(bool mostCommon, double div) =>
        mostCommon ? div >= 0.5 : div < 0.5;

    double accul = diagNumbers.Select(x => x[index] ? 1 : 0).Sum();
    var commonBitFilter = compare(mostCommon, accul / diagNumbers.Count());

    var results = diagNumbers.Where(x => x[index] == commonBitFilter).ToList();

    if(results.Count() == 1) {
        return new BitArray(results.First());
    }
    // C# is the wrong tool. The CLR supports tail, but 
    return FindRating(mostCommon, results, index+1);
}