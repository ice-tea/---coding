using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solver.Util;

namespace Solver
{
    public class GauseSolver/*<T>*/ //: ICloneable
    //where T : DNode
    //where T : Double
    {
        /* 记录变量名与其对应的位置 */
        private Dictionary<string, int> para_pos = new Dictionary<string, int>();
        /* 记录位置id与其对应的变量名 */
        private Dictionary<int, string> pos_para = new Dictionary<int, string>();
        private int max_index = 0;
        /* 约束矩阵描述 
          +---+-----+    +---+-----+               +-------+
          | i | Coe | -> | j | Coe | ...    <=>    | const |      <=>     对应信息
          +---+-----+    +---+-----+               +-------+
          ...
          ...
         */
        private LinkedList<WTuple<LinkedList<WTuple<int, double>>, DNode, string>> C = new LinkedList<WTuple<LinkedList<WTuple<int, double>>, DNode, string>>();

        public GauseSolver(){}

        public GauseSolver(GauseSolver solver)
        {
            foreach (KeyValuePair<string, int> pair in solver.para_pos)
                this.para_pos[pair.Key] = pair.Value;
            foreach (KeyValuePair<int, string> pair in solver.pos_para)
                this.pos_para[pair.Key] = pair.Value;
            this.max_index = solver.max_index;
            foreach (WTuple<LinkedList<WTuple<int, double>>, DNode, string> c in solver.C)
            {
                LinkedList<WTuple<int, double>> list = new LinkedList<WTuple<int, double>>();
                this.C.AddLast(new WTuple<LinkedList<WTuple<int, double>>, DNode, string>(list, new DNode(c.Item2), c.Item3));
                foreach (WTuple<int, double> pair in c.Item1)
                    this.C.Last.Value.Item1.AddLast(new WTuple<int, double>(pair.Item1, pair.Item2));
            }
        }

        /// <summary>
        /// 添加约束 a1*x1+a2*x2+...+an*xn = constant
        /// </summary>
        /// <param name="constraint">[集合]变量名 => 系数</param>
        /// <param name="constant">常数项（于等式右）</param>
        /// <param name="info">该约束对应的信息</param>
        /// <param name="ambiguous">是否接受二义性</param>
        public void AddConstraint(ICollection<WTuple<string, double>> constraint, DNode constant, string info = "", bool ambiguous = false)
        {
            WTuple<int, double>[] arr = new WTuple<int, double>[constraint.Count];
            int i = 0;
            foreach (WTuple<string, double> item in constraint)
            {   //添加变量并给其对应id
                string s = item.Item1.Trim();
                if (s == null || s == "")
                    continue;
                if (!para_pos.ContainsKey(s))
                {
                    para_pos[s] = max_index;
                    pos_para[max_index] = s;
                    max_index++;
                }
                arr[i++] = new WTuple<int, double>(para_pos[s], item.Item2);
            }
            //按id排序
            Array.Sort<WTuple<int, double>>(arr);
            //去重,合并同变量系数
            int t = 0;
            for (i = 1; i < arr.Length; ++i)
            {
                if (arr[t].Item1 == arr[i].Item1)
                {
                    arr[t].Item2 += arr[i].Item2;
                }
                else
                {
                    arr[++t] = arr[i];
                }
            }

            LinkedList<WTuple<int, double>> list = new LinkedList<WTuple<int, double>>(arr);
            for (++t; t < i; ++t)
                list.RemoveLast();
            C.AddLast(new WTuple<LinkedList<WTuple<int, double>>, DNode, string>(list, constant, info));

            LinkedListNode<WTuple<LinkedList<WTuple<int, double>>, DNode, string>> last = C.Last;
            LinkedListNode<WTuple<LinkedList<WTuple<int, double>>, DNode, string>> cur = C.First;
            while (cur != last)
            {
                Sub(cur, ref last);
                if (last.Value.Item1.Count == 0)
                {   //判冲突
                    if (!last.Value.Item2.IsZero())
                    {
                        if (ambiguous)
                        {   //接受二义性
                            Message.WarningAppendLine("waring: " + info);
                        }
                        else
                        {   //不接受二义性
                            Message.MessageAppendLine("error: " + info);
                        }
                    }
                    C.RemoveLast();
                    return;
                }
                cur = cur.Next;
            }
            double d = last.Value.Item1.First.Value.Item2;
            foreach (WTuple<int, double> item in last.Value.Item1)
            {
                item.Item2 /= d;
            }
            last.Value.Item2 /= d;
            cur = C.First;
            while (cur != last)
            {
                Sub(last, ref cur);
                cur = cur.Next;
            }
        }

        /// <summary>
        /// 约束矩阵合并（分支合并用）
        /// </summary>
        /// <param name="another">另一个约束</param>
        /// <returns></returns>
        public GauseSolver Union(GauseSolver another)
        {
            //将所有约束依次加入求解器中
            GauseSolver ret = new GauseSolver();

            LinkedList<WTuple<string, double>> list = new LinkedList<WTuple<string, double>>();
            foreach (WTuple<LinkedList<WTuple<int, double>>, DNode, string> con in this.C)
            {
                list.Clear();
                foreach (WTuple<int, double> item in con.Item1)
                {
                    list.AddLast(new WTuple<string, double>(this.pos_para[item.Item1], item.Item2));
                }
                ret.AddConstraint(list, con.Item2, con.Item3);
            }

            foreach (WTuple<LinkedList<WTuple<int, double>>, DNode, string> con in another.C)
            {
                list.Clear();
                foreach (WTuple<int, double> item in con.Item1)
                {
                    list.AddLast(new WTuple<string, double>(another.pos_para[item.Item1], item.Item2));
                }
                ret.AddConstraint(list, con.Item2, con.Item3, true);
            }
            return ret;
        }

        /// <summary>
        /// 将此实例的值转换为 System.String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder ret = new StringBuilder();
            foreach (WTuple<LinkedList<WTuple<int, double>>, DNode, string> c in C)
            {
                bool first = true;
                foreach (WTuple<int, double> item in c.Item1)
                {
                    if (!first) ret.Append('+');
                    first = false;
                    ret.Append(item.Item2).Append('*');
                    ret.Append(pos_para[item.Item1]);

                }
                ret.Append('=').Append(c.Item2);
                ret.Append('[').Append(c.Item3).Append("]\n");

            }
            return ret.ToString();
        }

        /// <summary>
        /// 两约束相减，维护上三角矩阵
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        private void Sub(LinkedListNode<WTuple<LinkedList<WTuple<int, double>>, DNode, string>> node1, ref LinkedListNode<WTuple<LinkedList<WTuple<int, double>>, DNode, string>> node2)
        {
            if (node1 == null || node2 == null)
                return;
            LinkedListNode<WTuple<int, double>> p1 = node1.Value.Item1.First;
            LinkedListNode<WTuple<int, double>> p2 = node2.Value.Item1.First;
            LinkedListNode<WTuple<int, double>> tmp;
            for (tmp = p2; tmp != null; tmp = tmp.Next)
                if (tmp.Value.Item1 == p1.Value.Item1)
                    break;
            if (tmp == null) return;
            double d = tmp.Value.Item2 / p1.Value.Item2;

            while (p1 != null && p2 != null)
            {
                if (p1.Value.Item1 == p2.Value.Item1)
                {
                    //p2.Value = new WTuple<int, double>(p1.Value.Item1, p2.Value.Item2 - d * p1.Value.Item2);
                    p2.Value.Item2 -= d * p1.Value.Item2;
                    tmp = p2;
                    p1 = p1.Next;
                    p2 = p2.Next;
                    if (Utility.IsZero(tmp.Value.Item2))
                        tmp.List.Remove(tmp);
                }
                else if (p1.Value.Item1 > p2.Value.Item1)
                {
                    p2 = p2.Next;
                }
                else
                {
                    p2.List.AddBefore(p2, new WTuple<int, double>(p1.Value.Item1, -p1.Value.Item2 * d));
                    p1 = p1.Next;
                }
            }
            for (; p1 != null; p1 = p1.Next)
                node2.Value.Item1.AddLast(new WTuple<int, double>(p1.Value.Item1, -p1.Value.Item2 * d));

            node2.Value.Item2 -= d * node1.Value.Item2;

        }

    }
}
