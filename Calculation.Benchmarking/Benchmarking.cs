using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Library;

namespace Calculation.Benchmarking
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.Throughput, warmupCount: 5, targetCount: 5)]
    public class Benchmarking
    {
        [Params(5, 10, 25, 50, 100, 150, 250, 300, 450, 500, 1000, 1500)] public int Dimension;
        Random rnd = new Random();
        private CalculationTree<int> tree;
        [GlobalSetup]
        public void GlobalSetup()
        {
            var list = new ConcurrentBag<ConcurrentBag<int>>();
            for (var i = 0; i < Dimension; i++)
            {
                list.Add(new ConcurrentBag<int>{rnd.Next()});
            }
            
            var counts = new DataNode<int>(list);
            var two = new DataNode<int>(new ConcurrentBag<ConcurrentBag<int>>{new ConcurrentBag<int>{2}});

            tree = Execution<int>.Compution(Execution<int>.reduceNode(REDUCE.SUM,
                Execution<int>.mapNode(MAP.MULTIPLY,
                    Execution<int>.productNode(PRODUCT.ONCOUNT, new List<Node<int>> {counts, two}))));
        }
    
        [Benchmark]
        public void OneThreaded() { tree.Calculate(); }

    }
}