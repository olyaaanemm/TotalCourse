using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Library;
using Structures;
namespace Structures
{
    class Program
    {
        static void Main(string[] args)
        {
            var counts = new DataNode<int>(new List<List<int>>{new List<int>{3}, new List<int>{2}, new List<int>{5}, new List<int>{5}, new List<int>{5}});
            var two = new DataNode<int>(new List<List<int>>{new List<int>{2}});
            
            var tree = Execution<int>.Compution(
                Execution<int>.reduceNode(REDUCE.SUM,
                Execution<int>.mapNode(MAP.MULTIPLY,
                    Execution<int>.productNode(PRODUCT.ONCOUNT, new List<Node<int>> {counts, two}))));
            tree.Calculate();
            Console.WriteLine(tree.getResult());
        }
    }
}