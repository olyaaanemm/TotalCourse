using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    public static class Execution<T>
    {
        
        private static CalculationTree<T> tree = new CalculationTree<T>(null);

        public static CalculationTree<T> Compution( Node<T>? node)
        {
            var tree_ = tree;
            tree = null;
            return tree_;
        }
        public static  Node<T>? dataNode(ConcurrentBag<T> data)
        {
            var info = new DataNode<T>(data);
            tree.pushNode(info);
            return info;
        }
        public static Node<T>? dataNode(DataNode<T> info )
        {
            var data = new DataNode<T>(info);
            tree.pushNode(data);
            return data;

        }
        public static  Node<T>? mapNode(MAP taskName, List<Node<T>> info)
        {
            var task = new TaskNode<MAP, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? mapNode(MAP taskName, Node<T> info)
        {
            var task = new TaskNode<MAP, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? zipNode(ZIP taskName, Node<T> info)
        {
            var task = new TaskNode<ZIP, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? zipNode(ZIP taskName, List<Node<T>> info)
        {
            var task = new TaskNode<ZIP, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? reduceNode(REDUCE taskName, Node<T> info)
        {
            var task = new TaskNode<REDUCE, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? reduceNode(REDUCE taskName, List<Node<T>> info)
        {
            var task = new TaskNode<REDUCE, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? productNode(PRODUCT taskName, Node<T> info)
        {
            var task = new TaskNode<PRODUCT, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        public static  Node<T>? productNode(PRODUCT taskName, List<Node<T>> info)
        {
            var task = new TaskNode<PRODUCT, T>(taskName, info);
            tree.pushNode(task);
            return task; 
        }
        
        public static ConcurrentBag<List<T>> map(Func<T, T, T> calculation, Node<T>? first, Node<T>? second)
         {
             ConcurrentBag<List<T>> data = new ConcurrentBag<List<T>>();
             if (first != null)
             {
                 var c = new CountdownEvent(first.getData().Count());
                 IEnumerable<IEnumerable<T>> coll = first.getData();
                 var tasks = coll.Select(
                     elem => new Task(
                         () =>
                         {
                             data.Add(new List<T>{calculation(elem.First(), elem.Last())});
                             c.Signal();
                         }));
                 foreach (var task in tasks)
                 {
                     task.Start();
                 }
                 c.Wait();
             }
             else
             {
                 IEnumerable<IEnumerable<T>> coll = second.getData();
                 var c = new CountdownEvent(second.getData().Count());
                 var tasks = coll.Select(
                     elem => new Task(
                         () =>
                         {
                             data.Add(new List<T>{calculation(elem.First(), elem.Last())});
                             c.Signal();
                         }));
                 foreach (var task in tasks)
                 {
                     task.Start();
                 }
                 c.Wait();
             }
             return data;
         }
        public static List<List<T>> mapNative(Func<T, T, T> calculation, Node<T>? first, Node<T>? second)
        {
            List<List<T>> data = new List<List<T>>();
            
            if (first != null)
            {
                IEnumerable<IEnumerable <T>> first_data = first.getData();
                foreach (var el in first_data)
                {
                    data.Add(new List<T>{calculation(el.First(), el.Last())});
                }
            }
            else
            {
                IEnumerable<IEnumerable <T>> second_data = second.getData();
                foreach (var el in second_data)
                {
                    data.Add(new List<T>{calculation(el.First(), el.Last())});
                }
            }

            return data;
        }

        public static List<List<T>> reduceNative(Func<T, T, T> calculation, Node<T>? first, Node<T>? second)  //left right
        {
             T c = (dynamic)0;
             if (second == null && first == null)
             {
                 return new List<List<T>>{ new List<T>()};
             }
             if (second == null)
             {
                 IEnumerable<IEnumerable<T>> first_data = first.getData();
                 foreach (var el in first_data)
                 {
                     c = calculation(c, el.First());
                 }
             }
             else
             {
                 IEnumerable<IEnumerable<T>> second_data = second.getData();
                 if (first == null)
                 {
                     foreach (var el in second_data)
                     {
                         c = calculation(c, el.First());
                     }
                 }
             }
             List<List<T>> data = new List<List<T>>{ new List<T>{c}};
             return data;
         }
         public static IEnumerable<IEnumerable<T>> product(Node<T>? first, Node<T>? second)
         {
             ConcurrentBag<List<T>> data = new ConcurrentBag<List<T>>();
             var second_data = second.getData();
             var first_data = first.getData();
             var c = new CountdownEvent(second.getData().Count());
             var tasks = second_data.Select(
                 el2 => new Task(
                     () =>
                     {
                         var m = new CountdownEvent(first_data.Count());
                         var tasks = first_data.Select(
                             el1 => new Task(
                                 () =>
                                {
                                    data.Add(new List<T>{el2.First(),el1.First()});
                                    m.Signal();
                                }));
                         foreach (var task in tasks)
                         {
                            task.Start();
                         }
                         m.Wait();
                         c.Signal();
                     }));
             foreach (var task in tasks)
             {
                 task.Start();
             }
             c.Wait();
             return data;
         }
         public static IEnumerable<IEnumerable<T>> productNative(Node<T>? first, Node<T>? second)
         {
             ConcurrentBag<ConcurrentBag<T>> data = new ConcurrentBag<ConcurrentBag<T>>();
             if (second == null && first == null)
             {
                 return data;
             }
             IEnumerable<IEnumerable<T>> second_data = second.getData();
             IEnumerable<IEnumerable<T>> first_data = first.getData();

             foreach (var elem  in second_data)
             {
                 foreach (var el in first_data)
                 {
                     data.Add(new ConcurrentBag<T>{elem.First(), el.First()});
                 }
                 
             }
             return data;
         }
         
         public static ConcurrentBag<ConcurrentBag<T>> zip(Node<T>? first, Node<T>? second)
         {
             ConcurrentBag<ConcurrentBag<T>> data = new ConcurrentBag<ConcurrentBag<T>>();
             var collection1 = first.getData();
             var collection2 = second.getData();
             if (collection1.Count() < collection2.Count())
             {
                 var c = new CountdownEvent(collection1.Count());
                 var tasks = Enumerable.Range(0, collection1.Count()).Select(
                     i => new Task(
                         () =>
                         {
                             data.Add(new ConcurrentBag<T> {collection1.ElementAt(i).ElementAt(0), collection2.ElementAt(i).ElementAt(0)});
                             c.Signal();
                     }));
                 foreach (var task in tasks)
                 {
                     task.Start();
                 }
                 c.Wait();
             }
             else
             {
                 var c = new CountdownEvent(collection2.Count());
                 var tasks = Enumerable.Range(0, collection2.Count()).Select(
                     i => new Task(
                         () =>
                         {
                             data.Add(new ConcurrentBag<T> {collection1.ElementAt(i).ElementAt(0), collection2.ElementAt(i).ElementAt(0)});
                             c.Signal();
                     }));
                 foreach (var task in tasks)
                 {
                     task.Start();
                 }
                 c.Wait();
             }
             return data;
         }
         
         public static IEnumerable<IEnumerable<T>> zipNative(Node<T>? first, Node<T>? second)
         {
             IEnumerable<IEnumerable<T>> data = new List<List<T>>();
             var collection1 = first.getData();
             var collection2 = second.getData();
             if (first.getData().Count() < second.getData().Count())
             {
                 for (int i = 0; i < collection1.Count(); i++)
                 {
                     data.Append(new ConcurrentBag<T> {collection1.ElementAt(i).ElementAt(0), collection2.ElementAt(i).ElementAt(0)});
                 }
             }
             else
             {
                 for (int i = 0; i < collection2.Count(); i++)
                 {
                     data.Append(new ConcurrentBag<T> {collection2.ElementAt(i).ElementAt(0), collection1.ElementAt(i).ElementAt(0)});
                 }
             }
             return data;
         }
         
    }
}