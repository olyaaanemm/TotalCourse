using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Library;

namespace Library
{
    public class CalculationTree<T>
    {
        protected Node<T>? head;

        public CalculationTree(Node<T> node)
        {
            this.head = node;
        }
        
        public Node<T>? getRoot()
        {
            return this.head;
        }
        public void  Calculate(Node<T>? node)
        {
            if (node != null)
            {
                Calculate(node.getLeft());
                Calculate(node.getRight());
                node.Execute();
            }
            //return node.getData();
        }
    }

    public abstract class Node<T>
    {
        protected List<List<T>> data;
        protected Node<T>? left;
        protected Node<T>? right;
        
        public List<List<T>> getData()
        {
            return this.data;
        }
        public Node<T>? getRight() {
            return this.right;
        }
        public Node<T>? getLeft() {
            return this.left;
        }

        public abstract void Execute();
    }
    
    public class DataNode<T>: Node<T>
    {
        public DataNode(List<List<T>> value)
        {
            this.data = value;
            this.left = null;
            this.right = null;
        }
        public DataNode(DataNode<T> other)
        {
            for (int i = 0; i < other.data.Count; i++)
            {
                this.data.Add(other.data[i]);
            }
            this.left = null;
            this.right = null;
        }
        public override void Execute() { return; }
    }
    
    public class TaskNode<T1, T>: Node<T>
    {
        private T1 taskType;
        public TaskNode(T1 taskName, List<Node<T>> info)
        {
            this.taskType = taskName;
            this.left = info[0];
            this.right = info[1];
        }
        public TaskNode(T1 taskName, Node<T> info)
        {
            this.taskType = taskName;
            this.left = info;
            this.right = null;
        }
        
         public override void Execute ()
        {
            switch (this.taskType)
            {
                case MAP.MULTIPLY:
                    map((x, y) => (dynamic) x * (dynamic) y);
                    break;
                case MAP.DERIVE:
                    map((x, y) => (dynamic) x / (dynamic) y);
                    break;
                case MAP.POWER:
                    map((x, y) => Math.Pow((dynamic)x, (dynamic)y));
                    break;
                case REDUCE.SUM:
                    reduce((x, y) => (dynamic)x + (dynamic)y);
                    break;
                case REDUCE.MIN:
                    reduce((x, y) => ((dynamic)x < (dynamic)y )? x : y);
                    break;
                case REDUCE.MAX:
                    reduce((x, y) => ((dynamic)x > (dynamic)y) ? x : y );
                    break;
                case REDUCE.COUNT:
                    reduce((x, y) => (dynamic)0 + (dynamic)1 );
                    break;
                case ZIP.TWOTOGHETHER:
                    zip();
                    break;
                case PRODUCT.ONCOUNT:
                    product();
                    break;
            }
        }

         void map(Func<T, T, T> calculation)
         {
             this.data = new List<List<T>>();
             if (this.left != null)
             {
                 for(var i = 0; i < this.left.getData().Count; i++) 
                 {
                     this.data.Add(new List<T>{calculation(this.left.getData()[i][0], this.left.getData()[i][1])});
                 }
             }
             else
             {
                 for(var i = 0; i < this.right.getData().Count; i++) 
                 {
                     this.data.Add(new List<T>{calculation(this.right.getData()[i][0], this.right.getData()[i][1])});
                 }
             }
             
         }
         void reduce(Func<T, T, T> calculation)
         {
             this.data = new List<List<T>>{new List<T>{(dynamic)0}};
             if (this.left != null)
             {
                 Parallel.ForEach(
                     Enumerable.Range(0, this.left.getData().Count),
                     i =>
                     {
                         this.data[0][0] = calculation(this.data[0][0],this.left.getData()[i][0]) ;
                     });
             }
             else
             {
                 Parallel.ForEach(
                     Enumerable.Range(0, this.right.getData().Count),
                     i =>
                     {
                         this.data[0][0] = calculation(this.data[0][0],this.right.getData()[i][0]) ;
                     });
             }
         }
         void product()
         {
             this.data = new List<List<T>>();
             for (var i = 0; i < this.right.getData().Count; i++)
             {
                 for (var j = 0; j < this.left.getData().Count; j++)
                 {
                     this.data.Add(new List<T>{this.right.getData()[i][0], this.left.getData()[j][0]});
                 }
             }
         }
         void zip()
         {
             this.data = new List<List<T>>();
             if (this.left.getData().Count < this.right.getData().Count)
             {
                 for (int i = 0; i < this.left.getData().Count; i++)
                 {
                     this.data.Add(new List<T>{this.left.getData()[i][0], this.right.getData()[i][0]});
                 }
             }
             else
             {
                 for (int i = 0; i < this.right.getData().Count; i++)
                 {
                     this.data.Add(new List<T>{this.right.getData()[i][0], this.left.getData()[i][0]});
                 }
             }

         }
    }

    public enum MAP
    {
        MULTIPLY,
        DERIVE,
        POWER
    }
    public enum REDUCE
    {
        SUM,
        COUNT,
        MIN,
        MAX, 
        PRODUCT
    }
    public enum ZIP
    {
        TWOTOGHETHER,
        FROMONE,
    }
    public enum PRODUCT
    {
        ONCOUNT
    }
}