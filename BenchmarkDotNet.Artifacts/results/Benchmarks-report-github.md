``` ini

BenchmarkDotNet=v0.10.13, OS=ubuntu 17.10
Intel Core i7-4702HQ CPU 2.20GHz (Haswell), 1 CPU, 8 logical cores and 4 physical cores
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT
  Core   : .NET Core 2.0.5 (CoreCLR 4.6.0.0, CoreFX 4.6.26018.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|             Method |     Mean |     Error |    StdDev | Rank |  Gen 0 | Allocated |
|------------------- |---------:|----------:|----------:|-----:|-------:|----------:|
|          MapString | 18.99 ns | 0.3591 ns | 0.2998 ns |    2 | 0.0229 |      72 B |
| MapStringOptimized | 13.90 ns | 0.4044 ns | 0.5928 ns |    1 | 0.0203 |      64 B |
