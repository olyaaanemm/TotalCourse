using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Library;

namespace Library
{
    public class CalculationTree<T>
    {
        protected Node<T>? head { get; set; }

        public CalculationTree(Node<T> node)
        {
            this.head = node;
        }
        
        public Node<T>? getRoot()
        {
            return this.head;
        }

        public bool pushNode(Node<T>? node)
        {
            if (node == null)
            {
                return false;
            }
            else
            {
                this.head = node;
                return true;
            }
        }
        public  void Calculate()
        {
            calculate(this.head);
        }

        private static void calculate(Node<T>? calc)
        {
            if (calc != null)
            {
                calculate(calc.getLeft());
                calculate(calc.getRight());
                calc.Execute();
            }
        }
        public  T getResult()
        {
            if (this.head == null)
            {
                return (dynamic) (- 1);
            }
            return this.head.getData().First().First();
        }
    }

    public abstract class Node<T>
    {
        protected IEnumerable<IEnumerable<T>> data { get; set; }
        protected Node<T>? left { get; set; }
        protected Node<T>? right { get; set; }
        
        public IEnumerable<IEnumerable<T>> getData()
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

        enum Type
        {
            DATA, 
            MAP, 
            ZIP, 
            PRODUCT, 
            REDUCE
        }
    }
    
    public class DataNode<T>: Node<T>
    {
        public DataNode(IEnumerable<IEnumerable<T>> value)
        {
            this.data = value;
            this.left = null;
            this.right = null;
        }
        public DataNode(IEnumerable<T> value)
        {
            var it = value.GetEnumerator();
            for (int i = 0; i < value.Count(); i++)
            {
                this.data.Append( new ConcurrentBag<T>{it.Current});
            }
            this.left = null;
            this.right = null;
        }
        public DataNode(DataNode<T> other)
        {
            var it = other.data.GetEnumerator();
            for (int i = 0; i < other.data.Count(); i++)
            {
                this.data.Append(it.Current);
            }
            this.left = null;
            this.right = null;
        }
        public override void Execute() { return; }
    }
    
    public class TaskNode<T1, T>: Node<T>
    {
        private T1 taskType;
        public TaskNode(T1 taskName, List<Node<T>?> info)
        {
            this.taskType = taskName;
            this.left = info[0];
            this.right = info[1];
        }
        public TaskNode(T1 taskName, Node<T>? info)
        {
            this.taskType = taskName;
            this.left = info;
            this.right = null;
        }

        public override void Execute()
        {
            switch (this.taskType)
            {
                case MAP.MULTIPLY: 
                    //this.data = Execution<T>.map(((x, y) =>  Operation(x,  y, OPERATION.MULTYPLY)), this.left, this.right);
                    this.data = Execution<T>.mapNative(((x, y) =>  Operation(x,  y, OPERATION.MULTYPLY)), this.left, this.right);
                    break;
                case MAP.DERIVE:
                    //this.data = Execution<T>.map(((x, y) =>  Operation(x,  y, OPERATION.DERIVE)), this.left, this.right);
                    this.data = Execution<T>.mapNative(((x, y) =>  Operation(x,  y, OPERATION.DERIVE)), this.left, this.right);
                    break;
                case MAP.POWER:
                    //this.data = Execution<T>.map(((x, y) =>  Operation(x,  y, OPERATION.POWER)), this.left, this.right);
                    this.data = Execution<T>.mapNative(((x, y) =>  Operation(x,  y, OPERATION.POWER)), this.left, this.right);
                    break;
                case REDUCE.SUM:
                    //this.data = Execution<T>.reduce((x, y) =>  Operation(x,  y, OPERATION.SUM), this.left, this.right);
                    this.data = Execution<T>.reduceNative((x, y) =>  Operation(x,  y, OPERATION.SUM), this.left, this.right);
                    break;
                case REDUCE.MIN:
                    //this.data = Execution<T>.reduce((x, y) =>  Operation(x,  y, OPERATION.LESS), this.left, this.right);
                    this.data = Execution<T>.reduceNative((x, y) => Operation(x,  y, OPERATION.LESS), this.left, this.right);
                    break;
                case REDUCE.MAX:
                    //this.data = Execution<T>.reduce((x, y) =>  Operation(x,  y, OPERATION.GREATER), this.left, this.right);
                    this.data = Execution<T>.reduceNative((x, y) => Operation(x,  y, OPERATION.GREATER), this.left, this.right);
                    break;
                case REDUCE.COUNT:
                    //this.data = Execution<T>.reduce((x, y) =>  Operation(x,  y, OPERATION.INCREMENT), this.left, this.right);
                    this.data = Execution<T>.reduceNative((x, y) => Operation(x,  y, OPERATION.INCREMENT), this.left, this.right);
                    break;
                case ZIP.TWOTOGHETHER:
                    //this.data = Execution<T>.zip(this.left, this.right);
                    this.data = Execution<T>.zipNative(this.left, this.right);
                    break;
                case PRODUCT.ONCOUNT:
                    this.data = Execution<T>.product(this.left, this.right);
                    //this.data = Execution<T>.productNative(this.left, this.right);
                    break;
            }
        }

        public T Operation (dynamic x, dynamic y, OPERATION operate) 
        {
            switch (operate)
            {
                case OPERATION.MULTYPLY: return x * y;
                case OPERATION.DERIVE: return x / y;
                case OPERATION.LESS: return (x < y) ? x : y;
                case OPERATION.GREATER: return (x > y) ? x : y;
                case OPERATION.POWER : return Math.Pow(x, y);
                case OPERATION.INCREMENT: return (dynamic)1;
                case OPERATION.SUM: return x + y;
                default: return (dynamic) 0;
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
    public enum OPERATION
    {
        MULTYPLY, 
        DERIVE, 
        POWER, 
        GREATER,
        LESS,
        INCREMENT, 
        SUM
    }
}
