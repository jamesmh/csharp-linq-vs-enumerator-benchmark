using System;
using BenchmarkDotNet.Running;

namespace csharp_linq_vs_enumerator_benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
           var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
