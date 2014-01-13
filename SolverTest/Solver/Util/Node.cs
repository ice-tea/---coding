using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver.Util
{
    public class Node
    {
        public double Value { set; get; }

        public Node(double v)
        {
            this.Value = v;
        }

        public Node(Node node)
        {
            this.Value = node.Value;
        }

        public static Node operator +(Node x, Node y)
        {
            return new Node(x.Value + y.Value);
        }

        public static Node operator -(Node x, Node y)
        {
            return new Node(x.Value - y.Value);
        }

        public static Node operator *(Node n, double d)
        {
            return new Node(n.Value * d);
        }

        public static Node operator *(double d, Node n)
        {
            return n * d;
        }

        public static Node operator /(Node n, double d)
        {
            return new Node(n.Value / d);
        }

        public static Node operator /(double d, Node n)
        {
            return n / d;
        }

        public bool IsZero() { return Utility.IsZero(this.Value); }


        public override string ToString()
        {
            return "<" + this.Value + ">";
        }
    }
}
