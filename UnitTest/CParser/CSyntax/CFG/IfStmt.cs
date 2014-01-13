using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CFrontendParser.CSyntax.Expression;
using UnitData;

namespace CFrontendParser.CSyntax.CFG
{
    /* if 条件语句 */
    class IfStmt : CStmt
    {
        private CExpr _condition;
        private CStmt _trueBranch;
        private CStmt _falseBranch;

        public IfStmt(CExpr c, CStmt t, CStmt f)
        {
            this._condition = c;
            this._trueBranch = t;
            this._falseBranch = f;
        }

        /* 分支条件 */
        public CExpr Condition { get { return this._condition; } }

        /* 真分支 */
        public CStmt TrueBranch { get { return this._trueBranch; } }

        /* 假分支 */
        public CStmt FalseBranch { get { return this._falseBranch; } }

        public override string ToString()
        {
            string str = "";
            str += this.GetTab(this.Depth) + "if";
            str += "(" + this._condition.ToString() + ")" + "\n";
            this._trueBranch.Depth = this.Depth;
            str += this._trueBranch.ToString() + "\n";
            if (this._falseBranch != null)
            {
                str += this.GetTab(this.Depth) + "else" + "\n";
                this._falseBranch.Depth = this.Depth;
                str += this._falseBranch.ToString() + "\n";
            }
            return str;
        }

        public override void Travel(Process process,ref UnitDataStruct parameter, object result)
        {
            process(this.Condition,ref parameter, result);

            UnitDataStruct tud = new UnitDataStruct(parameter);
            UnitDataStruct fud = new UnitDataStruct(parameter);
            if (this.TrueBranch != null)
            {
                tud.AddScope();
                this.TrueBranch.Travel(process, ref tud, result);
            }
            if (this.FalseBranch != null)
            {
                fud.AddScope();
                this.FalseBranch.Travel(process, ref fud, result);
            }
            fud.Union(tud);
            parameter = fud;
            Console.WriteLine("完成一次分支探索");
            Console.WriteLine(parameter.ToString());
        }
    }
}