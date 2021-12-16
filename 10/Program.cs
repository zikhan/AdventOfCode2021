var filePath = "input.txt";

var score = 0;
await foreach (var line in GetSystemLines(filePath))
{
    var stack = new Stack<char>();
    foreach (var ch in line.ToCharArray())
    {
        char? invalidCharacter = null;
        switch (ch)
        {
            case '{':
            case '[':
            case '(':
            case '<':
                stack.Push(ch);
                break;
            case '}':
            case ']':
            case ')':
            case '>':
                if (IsCompliment(stack.Peek(), ch))
                    stack.Pop();
                else
                    invalidCharacter ??= ch;
                break;
            default:
                break;
        }

        if (invalidCharacter != null){
            score += invalidCharacter switch
            {
                ')' => 3,
                ']' => 57,
                '}' => 1197,
                '>' => 25137,
                _ => 0
            };
            break;
        }
    }
}

Console.WriteLine($"Part 1: {score}");

var scoreList = new List<long>();
await foreach (var line in GetSystemLines(filePath))
{
    var stack = new Stack<char>();
    bool valid = true;
    foreach (var ch in line.ToCharArray())
    {
        switch (ch)
        {
            case '{':
            case '[':
            case '(':
            case '<':
                stack.Push(ch);
                break;
            case '}':
            case ']':
            case ')':
            case '>':
                if (IsCompliment(stack.Peek(), ch))
                    stack.Pop();
                else
                    valid = false;
                break;
            default:
                break;
        }

        if (!valid)
            // Ignore the corrupted lines
            break;
    }

    if (valid && stack.Any())
    {
        var completionBuilder = new List<char>();
        while (stack.TryPop(out char check))
        {
            completionBuilder.Add(
                check switch
                {
                    '(' => ')',
                    '[' => ']',
                    '{' => '}',
                    '<' => '>',
                    _ => throw new Exception("What the hell?")
                }
            );
        }


        scoreList.Add(
            completionBuilder.Select<char, long>(x =>
             x switch
             {
                 ')' => 1L,
                 ']' => 2L,
                 '}' => 3L,
                 '>' => 4L,
                 _ => 0L
             }).Aggregate(0L, (agg, v) => agg * 5L + v));;
    }
}

scoreList.Sort();
var median = scoreList[scoreList.Count / 2];

Console.WriteLine($"Part 2: {median}");

async IAsyncEnumerable<string> GetSystemLines(string filePath){
    using var file = File.OpenRead(filePath);
    using var reader = new StreamReader(file);
    string? line;
    while((line = await reader.ReadLineAsync()) != null){
        yield return line;
    }
}

bool IsCompliment(char A, char B){
    if (A == '{' && B == '}') return true;
    if (A == '(' && B == ')') return true;
    if (A == '[' && B == ']') return true;
    if (A == '<' && B == '>') return true;
    return false;
}