using System.Numerics;
using VectorizedSpans;

namespace Examples;

public static class Examples
{
    public static int Sum(Span<int> numbers)
    {
        VectorizedSpan<int> vspan = numbers; // Yeah we got implicit conversions 😎
        var vsum = Vector<int>.Zero;

        foreach (var v in vspan)
            vsum += v;

        var sum = Vector.Sum(vsum);
        foreach (var n in vspan.Leftovers)
            sum += n;

        return sum;
    }

    public static void NegativeIsolation(Span<int> numbers)
    {
        var venumer = new VectorizedSpanEnumerator<int>(numbers, i => i + Vector<int>.Count);
        while (venumer.MoveNext())
        {
            var v = venumer.Current;
            const int shrCount = sizeof(int) * 8 - 1; // sign flag shr
            var negatives = v >>> shrCount;
            negatives *= v;
            negatives.TryCopyTo(venumer.VSpan[venumer.Index..]);
        }

        for (var i = venumer.VSpan.LeftoversIndex; i < numbers.Length; i++)
        {
            if (numbers[i] > 0)
                numbers[i] = 0;
        }
    }
}