var filePath = "input.txt";

int outputSimpleNumbers = 0;
await foreach (var item in ReadInputs(filePath))
{
    outputSimpleNumbers +=
    item.Split(" | ")[1].Split(' ').Count(x => x.Length switch
    {
        2 or 3 or 4 or 7 => true,
        _ => false
    });
}

Console.WriteLine($"Part 1: {outputSimpleNumbers}");

// Part 2 notes
//    1 
//  7   2
//    6
//  5   3
//    4
// #1 -> both characters guarantee they either 2 or 3
// #7 -> all three guarantee either 1 or 2 or 3. 1 can be deduced as #7 Excluding #1
// #4 -> all 4 guarantee either 2 or 3 or 6 or 7. 6 or 7 come from #4 Excluding #1
// #8 -> all 7 guarantee either 1 or 2 or 3 or 4 or 5 or 6 or 7. 4 or 5 come from #8 Excluding #4 Union #7
// #6 -> Similar to #9 or #8 with 2 removed, #6 will be #8 with 1 of #1 removed,
// #9 -> similar to #6 or #8 with 5 removed, #9 will be #6 count, but (5) added, #9 is only 6 count and full intersects #4
// #0 -> 6-seg that's not #6 or #9
// #5 -> Intersect of #6 and #9
// #2 -> Intersect with number #5 will equal #8
// #3 -> 5-seg not #5 or #2

int outputSum = 0;
bool CharArrayEqual(IEnumerable<char> x, IEnumerable<char> y)
    => x.OrderBy(z => z).SequenceEqual(y.OrderBy(z => z));
await foreach (var item in ReadInputs(filePath))
{
    var readings = item.Split(" | ")[0].Split(' ');
    var num1 = readings.Where(x => x.Length == 2).Single() ?? throw new NullReferenceException();
    var num4 = readings.Where(x => x.Length == 4).Single() ?? throw new NullReferenceException();
    var num7 = readings.Where(x => x.Length == 3).Single() ?? throw new NullReferenceException();
    var num8 = readings.Where(x => x.Length == 7).Single() ?? throw new NullReferenceException();
    var num9 = readings.Where(x => x.Length == 6 && x.Intersect(num4).Count() == num4.Length).Single()  ?? throw new NullReferenceException();
    var num6 = readings.Where(x => x.Length == 6 && x.Union(num1).Intersect(num8).Count() == num8.Length).Single() ?? throw new NullReferenceException();
    var num0 = readings.Where(x => x.Length == 6 && !CharArrayEqual(x, num6) && !CharArrayEqual(x, num9)).Single() ?? throw new NullReferenceException();
    var num5 = readings.Where(x => x.Length == 5 && x.Except(num6.Intersect(num6)).Count() == 0).Single() ?? throw new NullReferenceException();
    var num2 = readings.Where(x => x.Length == 5 && x.Union(num5).Count() == 7).Single();
    var num3 = readings.Where(x => x.Length == 5 && !CharArrayEqual(x, num5) && !CharArrayEqual(x, num2)).Single() ?? throw new NullReferenceException();

    var output = item.Split(" | ")[1].Split(' ');
    char Parse(IEnumerable<char> outputDigit){
        if (CharArrayEqual(outputDigit, num0))
            return '0';
        if (CharArrayEqual(outputDigit, num1))
            return '1';
        if (CharArrayEqual(outputDigit, num2))
            return '2';
        if (CharArrayEqual(outputDigit, num3))
            return '3';
        if (CharArrayEqual(outputDigit, num4))
            return '4';
        if (CharArrayEqual(outputDigit, num5))
            return '5';
        if (CharArrayEqual(outputDigit, num6))
            return '6';
        if (CharArrayEqual(outputDigit, num7))
            return '7';
        if (CharArrayEqual(outputDigit, num8))
            return '8';
        if (CharArrayEqual(outputDigit, num9))
            return '9';
        throw new Exception("shit");
    }

    string @out = "";
    foreach (var digit in output)
        @out += Parse(digit);

    outputSum += int.Parse(@out);
}

Console.WriteLine($"Part 2: {outputSum}");

async IAsyncEnumerable<string> ReadInputs(string filePath){
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(file);
    string? line;
    while ((line = await reader.ReadLineAsync()) is not null)
    {
        yield return line;
    }
}