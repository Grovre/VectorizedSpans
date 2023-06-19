using System.Numerics;

namespace VectorEnumerator;
public ref struct VectorizedSpanEnumerator<T>
    where T : unmanaged
{
    private readonly VectorizedSpan<T> _vspan;
    private int _index;
    private readonly Func<int, int> _indexIncrementer;

    public readonly Vector<T> Current => _vspan.TryVectorAt(_index, out _);
    public Span<T> Leftovers => _vspan.Leftovers;

    public VectorizedSpanEnumerator()
    {
        _vspan = new VectorizedSpan<T>(Span<T>.Empty);
        _index = 0;
        _indexIncrementer = _ => 0;
    }

    public VectorizedSpanEnumerator(VectorizedSpan<T> span, Func<int, int> indexIncrementer)
    {
        _vspan = span;
        _index = -indexIncrementer(0);
        _indexIncrementer = indexIncrementer;
    }

    public bool MoveNext()
    {
        var i = _indexIncrementer(_index);
        if (i < 0 || i > _vspan.Span.Length - Vector<T>.Count)
            return false;

        _index = i;
        
        return true;
    }

    public void Reset()
    {
        _index = 0;
    }
}