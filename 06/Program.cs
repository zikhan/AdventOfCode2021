var filePath = "input.txt";

List<int> fishes = (await File.ReadAllLinesAsync(filePath)).Select(x => x.Split(',').Select(int.Parse)).SelectMany(x => x).ToList();

// BRUTE FORCE MF
int fishToAdd = fishes.Count(x => x == 0);
for (int i = 0; i < 80; i++)
{
    fishes = fishes.Select(x => x == 0 ? 6 : x - 1).ToList();
    fishes.AddRange(Enumerable.Repeat(8, fishToAdd));
    fishToAdd = fishes.Count(x => x == 0);
}

Console.WriteLine($"Part 1: {fishes.Count()}");


// Jumbled notes to understand why I was struggling with this math problem
// 1st gen (256 + (7-x))/7 = Total Children (tc)
// List of Birthdays for all children = [1..tc] as child => (7 * child - x);
// nth gen ((256 - DayBorn) >= 9 ? 1 : 0) = firstChild [Can be done due to reverse for] + firstChild ? clamp(floor((256 - DayBorn - 9)/7),0,256) : 0 =====> Total Nth Gen children
// nth gen list of Birthdays = [1..tc] as child => if child == 1 ? abs(9 + dayborn)) : (dayborn 7*child)

// 3,4,3,1,2 over 18 days

// 3 -> [1..(18+(3+1))//7]  => [7*1 - 3, 7*2 - 3, 7*3 - 3] = [4,11,18]
// [4,11,18] -> (18 - 4) >= 9 ? 1 : 0 + clamp(floor((18 - 4 -  9)/7), 0, 18) => 1 child 

var dayCount = 256;
fishes = (await File.ReadAllLinesAsync(filePath)).Select(x => x.Split(',').Select(int.Parse)).SelectMany(x => x).ToList();
var fishCount = fishes.LongCount();
var nthGen = fishes.Select(x => (x, Enumerable.Range(0, ((dayCount + (7- (x + 1)))/7)))).SelectMany(x => x.Item2.Select(child => (7 - ( 7 - ( x.x + 1))) + 7 * child)).GroupBy(x => x).Select(g => (birthday: g.Key, quantity: g.LongCount())).ToList();
fishCount += nthGen.Sum(x => x.quantity); // Count the second gen
for (int day = 0; day < dayCount; day++)
{
    var bornToday = nthGen.FirstOrDefault(x => x.birthday == day).quantity;
    // Next child birthday
    if(bornToday > 0) {
        nthGen = nthGen.Where(x => x.birthday != day).ToList(); // Remove birthdays today, we have their children
        var daysRemaining = dayCount - day;
        var theirNextGenCount = (daysRemaining >= 9 ? 1 : 0) + Math.Clamp((daysRemaining - 9) / 7, 0, dayCount);
        if (theirNextGenCount > 0) {
            fishCount += theirNextGenCount * bornToday;
            var theirNextGenBirthdays = Enumerable.Range(0, theirNextGenCount).Select(nextGenChild => day + 9 + (7 * nextGenChild));
            nthGen.AddRange(theirNextGenBirthdays.Select(x => (x, bornToday)).ToList());
            // Clean up cause i'm lazy
            nthGen = nthGen.GroupBy(x => x.birthday).Select(g => (g.Key, g.Sum(x => x.quantity))).ToList();
        }
    }
}

Console.WriteLine($"Part 2: {fishCount}");