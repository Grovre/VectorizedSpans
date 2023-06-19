using System.Numerics;

namespace VectorEnumerator;
public ref struct VectorizedSpanEnumerator<T>
    where T : unmanaged
{
    private readonly Span<T> _span;
    private int _index;
    private readonly Func<int, int> _indexIncrementer;

    public Vector<T> Current => new Vector<T>(_span[_index..]);
    public Span<T> Leftovers => _span[^(_span.Length % Vector<T>.Count)..];

    public VectorizedSpanEnumerator()
    {
        _span = Span<T>.Empty;
        _index = 0;
        _indexIncrementer = _ => 0;
    }

    public VectorizedSpanEnumerator(Span<T> span, Func<int, int> indexIncrementer)
    {
        _span = span;
        _index = -indexIncrementer(0);
        _indexIncrementer = indexIncrementer;
    }

    public bool MoveNext()
    {
        var i = _indexIncrementer(_index);
        if (i < 0 || i > _span.Length - Vector<T>.Count)
            return false;

        _index = i;
        
        return true;
    }

    public void Reset()
    {
        _index = 0;
    }
}