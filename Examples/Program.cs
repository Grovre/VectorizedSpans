using System.Numerics;

var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, 13 };

var sum = Examples.Examples.Sum(numbers);
Console.WriteLine($"{numbers.Sum()} == {sum}\n");

Examples.Examples.NegativeIsolation(numbers);
Console.WriteLine(string.Join(", ", numbers));
