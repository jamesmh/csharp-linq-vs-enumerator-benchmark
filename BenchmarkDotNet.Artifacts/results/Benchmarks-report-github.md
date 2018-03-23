``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i7-4710HQ CPU 2.50GHz (Haswell), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2435775 Hz, Resolution=410.5470 ns, Timer=TSC
.NET Core SDK=2.1.102
  [Host] : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  Core   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                    Method |       Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|-------------------------- |-----------:|----------:|----------:|-----:|-------:|----------:|
|                      Linq | 109.010 ns | 2.2610 ns | 2.7767 ns |    4 | 0.0813 |     256 B |
|                 Optimized |  17.659 ns | 0.4403 ns | 0.6172 ns |    2 | 0.0229 |      72 B |
|         ListWithTwoValues |  77.771 ns | 1.5946 ns | 2.0734 ns |    3 | 0.0407 |     128 B |
| TwoSingleValuedListsPiped |   9.995 ns | 0.2752 ns | 0.2826 ns |    1 | 0.0127 |      40 B |
