using System.Numerics;

namespace VectorizedSpans;
public ref struct VectorizedSpanEnumerator<T>
    where T : unmanaged
{
    public readonly VectorizedSpan<T> VSpan;
    public int Index { get; private set; }
    private readonly Func<int, int> _indexIncrementer;

    public Vector<T> Current => VSpan.TryVectorAt(Index, out _);
    public (Vector<T>, int) CurrentAndIndex => (Current, Index);
    public Span<T> Leftovers => VSpan.Leftovers;

    public VectorizedSpanEnumerator()
    {
        VSpan = new VectorizedSpan<T>(Span<T>.Empty);
        Index = 0;
        _indexIncrementer = _ => 0;
    }

    public VectorizedSpanEnumerator(VectorizedSpan<T> span, Func<int, int> indexIncrementer)
    {
        VSpan = span;
        Index = -indexIncrementer(0);
        _indexIncrementer = indexIncrementer;
    }

    public bool MoveNext()
    {
        var i = _indexIncrementer(Index);
        if (i < 0 || i > VSpan.Span.Length - Vector<T>.Count)
            return false;

        Index = i;
        
        return true;
    }

    public void Reset()
    {
        Index = 0;
    }
}