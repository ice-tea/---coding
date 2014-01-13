using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitData.Util
{
    public struct ConstIntNums
    {
        public int var;
        public string name;
    }
    public struct ConstFloatNums
    {
        public float var;
        public string name;
    }

    public class ConstNum
    {
        /* 记录变量名与其对应的位置 */
        private List<ConstIntNums> intList;
        /* 记录位置id与其对应的变量名 */
        private List<ConstFloatNums> floatList;
        private int max_index = 0;
        private string prefix = "-Const-Unit-";

        public ConstNum()
        {
            intList = new List<ConstIntNums>();
            floatList = new List<ConstFloatNums>();
            max_index = 0;
        }
        public ConstNum(ConstNum  cn)
        {
            this.intList = new List<ConstIntNums>(cn.intList);
            this.floatList = new List<ConstFloatNums>(cn.floatList);
            this.max_index = cn.max_index;
        }
        public string registerInt(string left)
        {
            ConstIntNums n;
            n.var = int.Parse(left);
            n.name = prefix + max_index;
            max_index++;
            intList.Add(n);
            return n.name;
        }
        public string registerFloat(string left)
        {
            ConstFloatNums f;
            f.var = float.Parse(left);
            f.name = prefix + max_index;
            max_index++;
            floatList.Add(f);
            return f.name;
        }


        public string getConstInt(int var)
        {
            foreach (ConstIntNums n in intList)
            {
                if (n.var == var)
                {
                    deleleConst(n.name);
                    return n.name;
                }
            }
            return null;
        }
        public string getConstFloat(float var)
        {
            foreach (ConstFloatNums f in floatList)
            {
                if ((f.var - var) < 0.0000001)
                {
                    deleleConst(f.name);
                    return f.name;
                }
            }
            return null;
        }
        void deleleConst(string name)
        {
            int count = intList.Count;
            for (int i = 0; i < count; i++)
            {
                if (intList[i].name.Equals(name))
                {
                    intList.RemoveAt(i);
                    return;
                }
            }

            count = floatList.Count;
            for (int i = 0; i < count; i++)
            {
                if (floatList[i].name.Equals(name))
                {
                    floatList.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
