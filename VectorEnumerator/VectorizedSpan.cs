using System.Numerics;
using System.Runtime.CompilerServices;

namespace VectorEnumerator;

public ref struct VectorizedSpan<T>
    where T : unmanaged
{
    public Span<T> Span { get; }
    public Span<T> Leftovers => Span[^(Span.Length % Vector<T>.Count)..];

    public VectorizedSpan()
    {
        Span = Span<T>.Empty;
    }

    public VectorizedSpan(Span<T> span)
    {
        Span = span;
    }

    public Vector<T> TryVectorAt(int index, out bool succeeded)
    {
        var v = Vector<T>.Zero;
        
        if (TooLarge(index))
        {
            succeeded = false;
            return v;
        }

        v = new Vector<T>(Span[index..]);
        succeeded = true;
        
        return v;
    }

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