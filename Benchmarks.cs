using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;

namespace csharp_linq_vs_enumerator_benchmark
{
    [CoreJob]
    [MemoryDiagnoser, RankColumn]
    public class Benchmarks
    {
        List<string> numbers;
        List<string> hasTwo;
        List<string> one;
        List<string> two;

        public Benchmarks()
        {
            this.numbers = new List<string>(){
               "1", "2", "4", "8", "16", "20"
            };

            this.hasTwo = new List<string>() { "1", "2" };
            this.one = new List<string>() { "1" };
            this.two = new List<string>() { "2" };
        }

        // [Benchmark]
        // public IEnumerable<int?> Linq()
        // {
        //     return this.numbers
        //         .Select(x => (int?)int.Parse(x))
        //         .Where(x => x > 4)
        //         .Except(new int?[] { 16, 18 });
        // }

        // [Benchmark]
        // public IEnumerable<int?> Optimized()
        // {
        //     return this.numbers.OptimizedPipe(p =>
        //        p.Select(x => (int?)int.Parse(x))
        //        .Where(x => x > 4)
        //        .Except(new int?[] { 16, 18 })
        //     );
        // }

        // [Benchmark]
        // public IEnumerable<int?> ListWithTwoValues()
        // {
        //     return this.hasTwo
        //        .Select(x => (int?)int.Parse(x))
        //        .Where(x => x > 4);
        // }

        // [Benchmark]
        // public IEnumerable<int?> TwoSingleValuedListsPiped()
        // {
        //     yield return this.one
        //         .Select(x => (int?)int.Parse(x))
        //         .Where(x => x > 4)
        //         .FirstOrDefault();

        //     yield return this.two
        //         .Select(x => (int?)int.Parse(x))
        //         .Where(x => x > 4)
        //         .FirstOrDefault();
        // }

        [Benchmark]
        public IEnumerable<string> MapString()
        {
            return Map(this.numbers, n => n.ToUpper());
        }

        [Benchmark]
        public IEnumerable<string> MapStringOptimized()
        {
            foreach (var item in this.numbers)
            {
                yield return Map(new string[] { item }, n => n.ToUpper()).FirstOrDefault();
            }
        }


        private IEnumerable<Tout> Map<T, Tout>(IEnumerable<T> list, Func<T, Tout> projection)
        {
            foreach (var item in list)
                yield return projection(item);
        }
    }
}