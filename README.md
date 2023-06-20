# VectorizedSpans
This library is a fine addition to your collection. Lightweight with a focus purely on performance through SIMD, vectorization of your code can be easier than ever. No third-party libraries are depended on, keeping developers building fast with apps running faster.

## What it doesTaking advantage of the generic SIMD operations provided by the Numerics namespace part of .NET, the runtime will opt for the best available SIMD ISA depending on the running architecture.

## The beef
Two ref structs are provided: `VectorizedSpan` and
`VectorizedSpanEnumerator`.

- VectorizedSpan
  - These structs are simply wrapped spans that provide vector functions instead of scalar functions. One of these structs in place of a regular span where appropriate will get developers taking advantage of vectorization.

- VectorizedSpanEnumerator
  - Yeah, it is what it says. This can be easily retrieved by a VectorizedSpan and even be used in a foreach loop. However, this enumerator is special. During construction, developers are able to implement their own functionality to the index mutator in case special functionality is needed instead of a strided pass over a span. For example, if you need a sliding window but fast then you can create your own enumerator with the appropriate three parameters.

## Implementation in your own codeHere's a simple example. Want to sum a span of numbers and pretend like .NET doesn't already make it as fast as possible itself? Here's the before:
```csharp
var sum = 0;
foreach (var n in numbers)
    sum += n;
return sum;
```
And here's the after:
```csharp
VectorizedSpan<int> vspan = numbers; // Yeah we got implicit conversions 😎
var vsum = Vector<int>.Zero;

// Cover all possible vectors until there are no more
foreach (var v in vspan)
    vsum += v;

// Add up the leftovers in case not all ints could be reached
var sum = Vector.Sum(vsum);
foreach (var n in vspan.Leftovers)
    sum += n;

return sum;
```

Now imagine that simplicity in something that could be far more complex. In fact, that's still far more simple than having to write out all of the vectorized code here and there, all over again, every time, with bound checks and all. Let's set all numbers except negatives to 0 The "challenge" here will be loading the vectors back into the span. Here's the scalar before:
```csharp
public static void NegativeIsolation(Span<int> numbers)
{
    for (var i = 0; i < numbers.Length; i++)
    {
        if (numbers[i] > 0)
            numbers[i] = 0;
    }
}
```
Wow. One comparison per number. If you're reading this, you don't like those statistics. Why else are you here? Let's see the after:
```csharp
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
```
Sure, it's a little longer. However, is it vectorized? Yes. Is it shorter than what it normally takes to vectorize too? Yes. That's what we're here for. This example also demonstrates the potential in using the enumerator directly instead of in a foreach. Namely, the ability to get the index in the span from which the vector was loaded from. That is how we loaded the integers back into the span. If the index isn't needed, it is recommended to stick with a foreach loop.

Although the examples provided are very simple, almost too simple for vectorization, they get the point across that vectorizing almost anything is made simple.

# Conclusion
It ain't much but it's (an) honest work(horse)