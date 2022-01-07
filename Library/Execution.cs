using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleThreadPool = SimpleThreadPool.SimpleThreadPool;

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
                 global::SimpleThreadPool.SimpleThreadPool pool_ = new global::SimpleThreadPool.SimpleThreadPool();
                 IEnumerable<IEnumerable <T>> first_data = first.getData();
                 CountdownEvent cde = new CountdownEvent(first_data.Count());
                 foreach (var el in first_data)
                 {
                     pool_.Submit(() =>
                     {
                         data.Add(new List<T> {calculation(el.First(), el.Last())});
                         cde.Signal();
                     }); 
                 }
                 cde.Wait();
                 pool_.Shutdown();
             }
             else
             {
                 global::SimpleThreadPool.SimpleThreadPool pool_ = new global::SimpleThreadPool.SimpleThreadPool();
                 IEnumerable<IEnumerable <T>> second_data = second.getData();
                 CountdownEvent cde = new CountdownEvent(second_data.Count());
                 foreach (var el in second_data)
                 {
                     pool_.Submit(() =>
                     {
                         data.Add(new List<T> {calculation(el.First(), el.Last())});
                         cde.Signal();
                     }); 
                 }
                 cde.Wait();
                 pool_.Shutdown();
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

        public static List<List<T>> reduce(Func<T, T, T> calculation, Node<T>? first, Node<T>? second) //left right Х
        { //MIN, MAX, COUNT, SUM 
            var c = (dynamic) 0;
            if (second == null && first == null)
            {
                return new List<List<T>>{ new List<T>()};
            }
            c = ParallelAlgo(WhoFrom(first, second), calculation);
            List<List<T>> data = new List<List<T>>{ new List<T>{c}};
            return data;
        }
        
        private static T ParallelAlgo(IEnumerable<IEnumerable<T>> collection, Func<T, T, T> calculation)
        {
            int batch_size = 8;
            var result_ = (dynamic) 0;
            ConcurrentBag<T> results = new ConcurrentBag<T>();
            var countdown = new CountdownEvent(8);
            int N = collection.Count() / batch_size;
            int ost = collection.Count() % batch_size;
            var threads = Enumerable.Range(0, 8).Select(
                i => new Thread(
                    () =>
                    {
                        var i1 = i;
                        var temp_sum = (dynamic) 0;
                        if (N > 0)
                        {
                            for (int j = N * i1; j < N * (i1 + 1); j++)
                            {
                                T new_ = calculation(temp_sum, collection.ElementAt(j).First());
                                var lst = temp_sum;
                                var origin = (dynamic) 0;
                                do
                                {
                                    origin = Interlocked.Exchange(ref temp_sum, new_);
                                } while (origin != lst);
                                //Console.WriteLine( "Thread " + i1 +  " Task "+j+" : New value: " + temp_sum);
                            }
                        }
                        if ((ost > 0 && i1 == (batch_size-1)) || (N == 0))
                        {
                                
                            for (int j = ost; j > 0; j--)
                            {
                                T new_ = calculation(temp_sum, collection.ElementAt(collection.Count()-j).First());
                                var lst = temp_sum;
                                var origin = (dynamic) 0;
                                do
                                {
                                    origin = Interlocked.Exchange(ref temp_sum, new_);
                                } while (origin != lst);
                                //Console.WriteLine( "Thread __ "+ i1 +  " Task "+(collection.Count()-j)+" : New value: " + temp_sum);
                            }
                        }
                        results.Add(temp_sum);
                        countdown.Signal();
                    }));
            foreach (var thread in threads)
            {
                thread.Start();
            }
            countdown.Wait();

            foreach (var res in results)
            {
                result_ = calculation(result_, res);
            }
            //Console.WriteLine("Result  : "+ result_);
            return result_;
        }

        private static IEnumerable<IEnumerable<T>> WhoFrom(Node<T>? first, Node<T>? second)
        {
            if (second == null)
            {
                return first.getData();
            }
            else
            {
                return second.getData();
            }
        }
        public static IEnumerable<IEnumerable<T>> product(Node<T>? first, Node<T>? second)
         {
             ConcurrentBag<List<T>> data = new ConcurrentBag<List<T>>(); 
             if ((second == null) && (first == null))
             {
                 return data;
             }
             global::SimpleThreadPool.SimpleThreadPool pool_ = new global::SimpleThreadPool.SimpleThreadPool();
             IEnumerable<IEnumerable<T>> second_data = second.getData();
             IEnumerable<IEnumerable<T>> first_data = first.getData();
             CountdownEvent cde = new CountdownEvent(second_data.Count());
             
             foreach (var elem  in second_data)
             {
                 pool_.Submit(() =>
                     {
                         foreach (var el in first_data)
                         {
                             data.Add(new List<T> {el.First(), elem.First()});
                         }
                         cde.Signal();
                     });
             }
             cde.Wait();
             pool_.Shutdown();
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
             global::SimpleThreadPool.SimpleThreadPool pool_ = new global::SimpleThreadPool.SimpleThreadPool();
             
             if (first.getData().Count() < second.getData().Count())
             {
                 CountdownEvent cde = new CountdownEvent(collection1.Count());
                 for (int i = 0; i < collection1.Count(); i++)
                 {
                     pool_.Submit(() =>
                     {
                         
                         data.Append(new ConcurrentBag<T>
                                 {collection1.ElementAt(i).ElementAt(0), collection2.ElementAt(i).ElementAt(0)});
                         cde.Signal();

                     });
                 }
                 cde.Wait();
             }
             else {
                 CountdownEvent cde = new CountdownEvent(collection2.Count());
                 for (int i = 0; i < collection2.Count(); i++)
                 {
                     pool_.Submit(() =>
                     {
                         data.Append(new ConcurrentBag<T>
                             {collection2.ElementAt(i).ElementAt(0), collection1.ElementAt(i).ElementAt(0)});
                         cde.Signal();
                     });
                 }
                 cde.Wait();
             }
             pool_.Shutdown();
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
