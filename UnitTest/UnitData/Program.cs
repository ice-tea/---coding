using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitData
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitDataStruct unitData = new UnitDataStruct();

            unitData.register("{\"#12\",\"kg\"}");
            unitData.register("{\"#11\",\"km\"}");
            unitData.register("{\"#11\",\"m\"}");

            unitData.register("{\"#a\",\"s\"}");
            unitData.register("{\"#b\",\"m\"}");
            unitData.register("{\"#{A}B\",\"kg\"}");

            unitData.AddScope();
            unitData.register("{\"#b\",\"km\"}");
            unitData.register("{\"#c\",\"?\"}");
            unitData.addCom("x", "A");


            string info = unitData.get("12");
            Console.WriteLine(info);

            info = unitData.get("11");
            Console.WriteLine(info);
            info = unitData.get("11");
            Console.WriteLine(info);
            info = unitData.get("b");
            Console.WriteLine(info);
            info = unitData.get("c");
            Console.WriteLine(info);

            info = unitData.getCom("x", "B");
            Console.WriteLine(info);

            unitData.MinusScope();
            info = unitData.get("b");
            Console.WriteLine(info);


        }
    }
}
