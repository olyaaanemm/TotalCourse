using System;
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
            var counts = new DataNode<int>(new List<List<int>>{new List<int>{3}, new List<int>{2}, new List<int>{5}});
            var two = new DataNode<int>(new List<List<int>>{new List<int>{2}});
            var product = new TaskNode<PRODUCT, int>(PRODUCT.ONCOUNT, new List<Node<int>> {counts, two});
            var map = new TaskNode<MAP, int>(MAP.MULTIPLY, product);
            var reduce = new TaskNode<REDUCE, int>(REDUCE.SUM, map);

            var tree = new CalculationTree<int>(reduce);
            tree.Calculate(tree.getRoot());
            Console.WriteLine(tree.getRoot().getData()[0][0]);
        }
        //привет, я здесь/ чтож/ мы постараемся все сдать тогда
    }
}