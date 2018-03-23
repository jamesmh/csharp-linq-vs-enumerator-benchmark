using System;
using System.Collections.Generic;
using System.Linq;

namespace csharp_linq_vs_enumerator_benchmark
{
    public static class Pipe
    {
        public static IEnumerable<Tout> OptimizedPipe<T, Tout>(this IEnumerable<T> list, Func<IEnumerable<T>, IEnumerable<Tout>> pipes)
         {
            foreach(T item in list)
            {
                Tout result = pipes(new T[] { item }).FirstOrDefault();
                if(result != null)
                    yield return result;
            };
        }
    }
}