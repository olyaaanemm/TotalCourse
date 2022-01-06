using System;
using BenchmarkDotNet.Running;

namespace Calculation.Benchmarking
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<Benchmarking>();
        }
    }
}