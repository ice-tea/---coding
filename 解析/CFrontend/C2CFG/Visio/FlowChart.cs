/***********************************************************************
 * 类描述   ： 入口类 ： 图 -> Visio流程图
 *
 * 作者     : snowingsea
 * 完成时间 ： 2012年09月18日
 ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Office.Interop.Visio;
using C2CFG.FlowChartElement;
using C2CFG.GraphElement;

namespace C2CFG.Visio
{
    public class FlowChart
    {
        //////////////////////////////////////////////////////
        // 成员变量
        /* 记录已访问的节点id */
        private Dictionary<int, Shape> visited;
        /* Visio流程图 */
        private VisioChart chart;
        /* 流程图信息 */
        private Flow flow;
        /* 画第一个点，即起点*/
        private bool first;
        /* 结尾节点ID */
        private int endID;
        private Dictionary<int, Tuple<double, double>> positions;
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         */
        public FlowChart()
        {
            this.visited = new Dictionary<int, Shape>();
            this.chart = new VisioChart();
            this.flow = new Flow();
            this.first = true;
            this.positions = new Dictionary<int, Tuple<double, double>>();
        }

        /* 画流程图，主入口
         */
        public void Draw(Graph g)
        {
            Block b = flow.Build(g);
            this.endID = flow.AddingID;
            chart.New();
            this.DrowNode(b.Start, 0.5, 0.5);

            foreach (KeyValuePair<int, List<Tuple<string, int>>> edge in flow.UpRemovedEdges)
            {   // 添加删除的向上的环边
                foreach (Tuple<string, int> tuple in edge.Value)
                {
                    chart.DrowEdge(visited[edge.Key], visited[tuple.Item2], (short)Direction.Right, (short)Direction.Right, tuple.Item1);
                }
            }

            foreach (KeyValuePair<int, List<Tuple<string, int>>> edge in flow.DownRemovedEdges)
            {   // 添加删除的跳转边
                foreach (Tuple<string, int> tuple in edge.Value)
                {
                    chart.DrowEdge(visited[edge.Key], visited[tuple.Item2], (short)Direction.Left, (short)Direction.Left, tuple.Item1);
                }
            }
        }

        /* DFS画点，并连接起来
         */
        private void DrowNode(Node node, double x, double y)
        {
            if (!this.visited.ContainsKey(node.ID))
            {
                x += node.Offset_X;
                y += node.Offset_Y;
                Shape shape;

                int count = node.NextEdges.Count;
                if (flow.DownRemovedEdges.ContainsKey(node.ID))
                    count += flow.DownRemovedEdges[node.ID].Count;
                if (flow.UpRemovedEdges.ContainsKey(node.ID))
                    count += flow.UpRemovedEdges[node.ID].Count;

                if (this.first)
                    shape = chart.DrowNode(MasterItem._start_end, x, y, "开始");
                else if (count < 1)
                    shape = chart.DrowNode(MasterItem._start_end, x, y, "结束");
                else if (count < 2)
                    shape = chart.DrowNode(MasterItem._process, x, y, node.Text);
                else
                    shape = chart.DrowNode(MasterItem._decision, x, y, node.Text);
                first = false;
                visited.Add(node.ID, shape);
                this.positions.Add(node.ID, new Tuple<double, double>(x, y));

                foreach (Tuple<string, Node> tuple in node.NextEdges)
                {
                    Node tmp = tuple.Item2;
                    this.DrowNode(tmp, x, y);
                    Tuple<double, double> offset = null;
                    if (node.NextEdges.Count > 1 || tmp.ID < 0 && tmp.ID != this.endID)
                    {
                        double offsetX = positions[tmp.ID].Item1 - positions[node.ID].Item1;
                        double offsetY = positions[tmp.ID].Item2 - positions[node.ID].Item2 + (tmp.Height + node.Height + Block.VERTICAL_SPACING) / 2;
                        offset = new Tuple<double, double>(offsetY, offsetX);
                    }
                    if (!flow.AddingEdgeIDs.Contains(new Tuple<int, int>(node.ID, tmp.ID)))
                        chart.DrowEdge(shape, visited[tmp.ID], (short)Direction.Down, (short)Direction.Up, tuple.Item1, offset);
                }
            }
        }

        //
        //////////////////////////////////////////////////////
    }
}
