using System.Numerics;
using NUnit.Framework;
using VectorizedSpans;

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
        var vspan = new VectorizedSpan<int>(Numbers);
        foreach (var v in vspan)
        {
            for (var i = 0; i < Vector<int>.Count; i++)
            {
                copyList.Add(v[i]);
            }
        }

        foreach (var n in vspan.Leftovers)
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
        var vspan = new VectorizedSpan<int>(Span<int>.Empty);
        foreach (var v in vspan)
        {
            for (var i = 0; i < Vector<int>.Count; i++)
            {
                copyList.Add(v[i]);
            }
        }

        foreach (var n in vspan.Leftovers)
        {
            copyList.Add(n);
        }

        Assert.AreEqual(Array.Empty<int>(), copyList.ToArray());
    }

    [Test]
    public void VectorizedSpanTests()
    {
        VectorizedSpan<int> vspan = Numbers;
        var index = -1;
        var rand = new Random();

        var expected = new int[Vector<int>.Count];
        var actual = new int[Vector<int>.Count];
        for (var i = 0; i < 100; i++)
        {
            index = rand.Next(0, Numbers.Length - Vector<int>.Count);
            var succeeded = vspan.TryVectorAt(index, out var v);
            Assert.True(succeeded);
            Numbers.AsSpan(index, Vector<int>.Count).CopyTo(expected);
            v.TryCopyTo(actual);

            Assert.AreEqual(expected, actual);
        }

        for (var i = 0; i < Vector<int>.Count - 1; i++)
        {
            var s = vspan.TryVectorAt(Numbers.Length - i, out _);
            Assert.False(s);
        }

        var blocks = Numbers.Length / Vector<int>.Count;
        var block = 0;
        for (; block < blocks; block++)
        {
            vspan.TryNthVector(block, out var s);
            Assert.True(s);
        }

        vspan.TryNthVector(block, out var suc);
        Assert.False(suc);
    }
}