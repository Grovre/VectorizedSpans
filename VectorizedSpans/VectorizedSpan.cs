using System.Numerics;
using System.Runtime.CompilerServices;

namespace VectorizedSpans;

public readonly ref struct VectorizedSpan<T>
    where T : unmanaged
{
    public readonly Span<T> Span;
    public Span<T> Leftovers => Span[LeftoversIndex..];
    public int LeftoversIndex => Span.Length - (Span.Length % Vector<T>.Count);

    public static implicit operator Span<T>(VectorizedSpan<T> vspan) => vspan.Span;
    public static implicit operator VectorizedSpan<T>(Span<T> span) => new VectorizedSpan<T>(span);
    public static implicit operator VectorizedSpan<T>(T[] array) => new VectorizedSpan<T>(array.AsSpan());
    public Span<T> this[Range range] => Span[range];

    public VectorizedSpan()
    {
        Span = Span<T>.Empty;
    }

    public VectorizedSpan(Span<T> span)
    {
        Span = span;
    }

    public bool TryVectorAt(int index, out Vector<T> v)
    {
        v = Vector<T>.Zero;
        
        if (TooLarge(index))
            return false;

        v = new Vector<T>(Span[index..]);
        return true;
    }

    public Vector<T> VectorAt(int index)
    {
        VectorAt(index, out var v);
        return v;
    }

    public void VectorAt(int index, out Vector<T> v)
        => v = new Vector<T>(Span[index..]);

    public Vector<T> TryNthVector(int n, out bool succeeded)
    {
        var v = Vector<T>.Zero;
        var i = n * Vector<T>.Count;
        
        if (TooLarge(i))
        {
            succeeded = false;
            return v;
        }

        v = new Vector<T>(Span[i..]);
        succeeded = true;
        
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TooLarge(int index)
    {
        return Span.Length - index < Vector<T>.Count;
    }

    public VectorizedSpanEnumerator<T> GetEnumerator()
    {
        return new VectorizedSpanEnumerator<T>(this, i => i + Vector<T>.Count);
    }
}