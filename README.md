# Benchmarking C# iterator optimization

This is a little experiment to see if we can speed up linq queries by using the functional `pipe` technique.

By "piping" linq queries, we can avoid the inherent issue with linq whereby each query will issue __a whole iteration over the collection__. This optimization allows us to issue the equivalent of __one iteration__ and pass each element through the entire method chain.

Benchmarking 3 linq queries proves to be 6X faster in mean execution time.

```markdown
|    Method |      Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|---------- |----------:|----------:|----------:|-----:|-------:|----------:|
|      Linq | 103.69 ns | 0.7379 ns | 0.6903 ns |    2 | 0.0813 |     256 B |
| Optimized |  17.15 ns | 0.3958 ns | 0.5147 ns |    1 | 0.0229 |      72 B |
```

# The Optimization

The optimized method used in the benchmarks (only for types returning `null` as their default value) looks like this:

```c#
public static IEnumerable<Tout> OptimizedPipe<T, Tout>(this IEnumerable<T> list, Func<IEnumerable<T>, IEnumerable<Tout>> pipes)
{
    foreach(T item in list)
    {
        Tout result = pipes(new T[] { item }).FirstOrDefault();
        if(result != null)
            yield return result;
    };
}
```

You can use this to run only certain linq queries (`Select`, `Where`, `Except`, `Intersect`, `TypeOf`) due to the nature of the optimization.

Side Note: You can change the method above to handle non-nullable types as well... but let's keep it simple for now :)

Here is that method used in the benchmark:

```
 return this.numbers.OptimizedPipe(p =>
    p.Select(x => int.Parse(x))
    .Where(x => x > 4)
    .Except(new int[] { 16, 18 })
);
```

The equivalent linq queries without the `OptimizedPipe()` is what we are benchmarking against.

# What Happened?

By iterating over the collection __once__ and passing each value into the method chain as __a one valued IEnumerable__, there is an optimzation being done under the covers. In other words, the overhead of iterating over a single valued collection is somehow optimized to just avoid the iteration (it seems).

# More Proof

Consider the assignments:

```c#
this.hasTwo = new List<string>() { "1", "2" };
this.one = new List<string>(){ "1" };
this.two = new List<string>(){ "2" };
```

Now consider the benchmarks:

```c#
[Benchmark]
public IEnumerable<int> ListWithTwoValues(){
        return this.hasTwo
        .Select(x => int.Parse(x))
        .Where(x => x > 4);
}

// Vs.

[Benchmark]
public IEnumerable<int> TwoSingleValuedListsPiped(){
    yield return this.one
        .Select(x => int.Parse(x))
        .Where(x => x > 4)
        .FirstOrDefault();

    yield return this.two
        .Select(x => int.Parse(x))
        .Where(x => x > 4)
        .FirstOrDefault();
}
```

They will take the same amount of time - right? Plus, the second case is performing an extra call to `FirstOrDefault()`... so maybe it will be even slower?

The first one will iterate two times (Select and Where) over two items in the collection. So, 2 * 2 = 4 iterations.

The second one will iterate 2 times (Select and Where) per collection (each having one value). So, for two collections having one value each, thats 2 * 2 = 4 iterations.

But, that's not __really__ correct:

```markdown
|                    Method |      Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|-------------------------- |----------:|----------:|----------:|-----:|-------:|----------:|
|         ListWithTwoValues |  76.29 ns | 0.2022 ns | 0.1689 ns |    3 | 0.0407 |     128 B |
| TwoSingleValuedListsPiped |  10.72 ns | 0.0659 ns | 0.0550 ns |    1 | 0.0127 |      40 B |
```

The next question we need to ask is: Is this a __linq only__ optimization? Or is it something that is really being optimized by the enumerator?

# Can we optimize a foreach?

To find out, we need to use a non-linq operation (over a collection) with and without the optimization.

For this, I created a `Map` function to use for our test operation:

```c#
private IEnumerable<Tout> Map<T, Tout>(IEnumerable<T> list, Func<T, Tout> projection)
{
    foreach (var item in list)
        yield return projection(item);
}
```

The two benchmarks look like this:

```c#
[Benchmark]
public IEnumerable<string> MapFilterString()
{
    return Map(this.numbers, n => n.ToUpper());
}

[Benchmark]
public IEnumerable<string> MapFilterStringOptimized()
{
    foreach (var item in this.numbers)
    {
        var result = Map(this.numbers, n => n.ToUpper()).FirstOrDefault();
        if (result != null)
            yield return item;
    }
}
```
Again, it looks like the first benchmkark should be faster. But it's not:

```markdown
|             Method |     Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|------------------- |---------:|----------:|----------:|-----:|-------:|----------:|
|          MapString | 18.99 ns | 0.3591 ns | 0.2998 ns |    2 | 0.0229 |      72 B |
| MapStringOptimized | 13.90 ns | 0.4044 ns | 0.5928 ns |    1 | 0.0203 |      64 B |

```

# Conclusion

So, there you have it! At run-time, c# optimizes single valued collections so you can pipe them through a chain of functions - each expecting and returning a collection. This seems to be done at a lower level - probably in the `Enumerator` class for the collections. 

The performance gain of doing this is significant if chaining multiple methods / operations. What does this mean practically? 

- You could create functors in c# very easily without worrying about performance hits due to the nature of iterating over collections.

- If using `Select`, `Where`, `Intersect` and `Except` queries in linq, you can use this method to make linq even faster.

- Creating your own `pipe` method can avoid the overhead of issuing an iteration per operation by passing in each element through the entire chain once.
