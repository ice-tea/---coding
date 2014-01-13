using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solver.Util
{
    public class WTuple<T1, T2> : IComparable
        where T1 : IComparable
        where T2 : IComparable
    {
        public WTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public T1 Item1 { set; get; }
        public T2 Item2 { set; get; }

        public override bool Equals(object obj)
        {
            if (obj is WTuple<T1, T2>)
            {
                WTuple<T1, T2> other = (WTuple<T1, T2>)obj;
                return this.Item1.Equals(other.Item1) && this.Item2.Equals(other.Item2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return "(" + this.Item1 + ", " + this.Item2 + ")";
        }

        public int CompareTo(object obj)
        {
            if (obj is WTuple<T1, T2>)
            {
                WTuple<T1, T2> other = (WTuple<T1, T2>)obj;
                int t = this.Item1.CompareTo(other.Item1);
                if (t == 0) return this.Item2.CompareTo(other.Item2);
                return t;
            }
            return 1;
        }
    }

    public class WTuple<T1, T2, T3>/* : IComparable
        where T1 : IComparable
        where T2 : IComparable
        where T3 : IComparable*/
    {
        public WTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
        public T1 Item1 { set; get; }
        public T2 Item2 { set; get; }
        public T3 Item3 { set; get; }

        public override bool Equals(object obj)
        {
            if (obj is WTuple<T1, T2, T3>)
            {
                WTuple<T1, T2, T3> other = (WTuple<T1, T2, T3>)obj;
                return this.Item1.Equals(other.Item1) && this.Item2.Equals(other.Item2) && this.Item3.Equals(other.Item3);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return "(" + this.Item1 + ", " + this.Item2 + ", " + this.Item3 + ")";
        }

        //public int CompareTo(object obj)
        //{
        //    if (obj is WTuple<T1, T2>)
        //    {
        //        WTuple<T1, T2> other = (WTuple<T1, T2>)obj;
        //        int t1 = this.Item1.CompareTo(other.Item1);
        //        int t2 = this.Item2.CompareTo(other.Item2);
        //        if (t1 == 0 && t2 == 0) return this.Item2.CompareTo(other.Item2);
        //        if (t1 == 0) return t2;
        //        return t1;
        //    }
        //    return 1;
        //}
    }
}
