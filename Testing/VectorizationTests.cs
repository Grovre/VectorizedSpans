using System.Numerics;
using NUnit.Framework;
using VectorEnumerator;

namespace Testing;

[TestFixture]
public class VectorizationTests
{
    private static int[] Numbers = new int[124_369];

    [OneTimeSetUp]
    public void Setup()
    {
        var rand = new Random();
        for (var i = 0; i < Numbers.Length; i++)
        {
            Numbers[i] = rand.Next(0, 1_000);
        }
    }
    
    [Test]
    public void VectorizedSpanEnumeratorTest()
    {
        var copyList = new List<int>();
        int Incrementer(int z) => z + Vector<int>.Count;
        var enumerator = new VectorizedSpanEnumerator<int>(Numbers, Incrementer);
        while (enumerator.MoveNext())
        {
            var v = enumerator.Current;
            for (var i = 0; i < Vector<int>.Count; i++)
            {
                copyList.Add(v[i]);
            }
        }

        foreach (var n in enumerator.Leftovers)
        {
            copyList.Add(n);
        }

        Assert.AreEqual(Numbers, copyList.ToArray());
    }
    
    [Test]
    public void EmptyVectorizedSpanEnumeratorTest()
    {
        var copyList = new List<int>();
        int Incrementer(int z) => z + Vector<int>.Count;
        var enumerator = new VectorizedSpanEnumerator<int>(Span<int>.Empty, Incrementer);
        while (enumerator.MoveNext())
        {
            var v = enumerator.Current;
            for (var i = 0; i < Vector<int>.Count; i++)
            {
                copyList.Add(v[i]);
            }
        }
        
        foreach (var n in enumerator.Leftovers)
        {
            copyList.Add(n);
        }

        Assert.AreEqual(Array.Empty<int>(), copyList.ToArray());
    }
}