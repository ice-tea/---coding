using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using CFrontendParser.CSyntax;
using CFrontendParser.CSyntax.CFG;
using C2CFG.GraphElement;
using C2CFG.Visio;

namespace C2CFG
{
    public class DrawCFG
    {
        // 待绘制的控制流图
        private CStmt cfg;

        public DrawCFG(CFunction function)
        {
            this.cfg = function.Body;
        }

        public void Draw()
        {
            Graph graph = this.CFG2Graph();
            FlowChart chart = new FlowChart();
            chart.Draw(graph);
        }

        private Graph CFG2Graph()
        {
            int id = 1;
            return this.CFG2Graph(this.cfg, ref id);
        }

        private Graph CFG2Graph(CStmt stmt, ref int id)
        {
            if (stmt is CompoundStmt)
            {
                return this.CFG2Graph((CompoundStmt)stmt, ref id);
            }
            else if (stmt is SimpleStmt || stmt is ExprStmt || stmt is GoStmt || stmt is ReturnStmt)
            {
                int sid = id;
                Graph g = new Graph(sid);
                id++;
                Vertex v1 = new Vertex(id, stmt.ToString());
                id++;
                Vertex v0 = g.GetVertex(sid);
                g.AddEdge(v0, v1, "");
            }
            else if (stmt is SwitchStmt)
            {

            }
            else if (stmt is ForStmt)
            {

            }
            else if (stmt is WhileStmt)
            {

            }
            return null;
        }

        private Graph CFG2Graph(CompoundStmt stmt, ref int id)
        {
            Graph result = this.CFG2Graph(stmt[0], ref id);
            for (int i = 1; i < stmt.StmtCount; i++)
            {
                Graph gi = this.CFG2Graph(stmt[i], ref id);
                result = new Graph(result, gi, true);
            }
            return result;
        }
    }
}
