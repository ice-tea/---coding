/***********************************************************************
 * 类描述   ： 流程信息类，为流程图的描绘做准备
 * 
 *
 * 作者     : snowingsea
 * 修改时间 ： 2012年9月18日
 ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using C2CFG.GraphElement;

/*---------------------------------------------------------------------------------
 * 我们这里定义不规则边(irregular edge)为
 *   1、所有循环回边（包括循环末尾以及while或for中的continue），流程图表现为向上的边；
 *   2、所有空分支以及跳出语句（包括while和for中的break和dowhile中的break和continue），
 *      流程图中表现为向下的边
 *   
 * 因此，现在的做法是先去除这些不规则边，并存起来，处理完剩下的规则边后再加入去除的不规则边。
 * 
 * 观察后可以发现：
 *     第一种不规则边即为DFS时指向那些已经被遍历节点的边；
 *     第二种不规则边在去除第一种不规则边后即为起始点出度大于1，终止点入读大于1的边
 *     
 ----------------------------------------------------------------------------------*/

namespace C2CFG.FlowChartElement
{
    public class Flow
    {

        //////////////////////////////////////////////////////
        // 成员变量
        /* 记录已访问的节点 */
        private Dictionary<int, Node> visitedNode;
        public Dictionary<int, Node> VisitedNode
        { get { return this.visitedNode; } }

        /* 记录路径信息 */
        private HashSet<int> path;

        /* 构建流程图时会先删除环，去掉其中的一条返回边，即第一种不规则边 */
        /* 其中Tuple中的Item分别为边文字信息，终点id           */
        private Dictionary<int, List<Tuple<string, int>>> upRemovedEdges;
        public Dictionary<int, List<Tuple<string, int>>> UpRemovedEdges
        { get { return this.upRemovedEdges; } }

        /* 构建流程图时然后会删除跳转边，即第二种不规则边     */
        /* 其中Tuple中的Item分别为边文字信息，终点id */
        private Dictionary<int, List<Tuple<string, int>>> downRemovedEdges;
        public Dictionary<int, List<Tuple<string, int>>> DownRemovedEdges
        { get { return this.downRemovedEdges; } }

        /* 画图需要，先虚拟添加一些边                                     */
        /* 将循环中存在一个点有唯一后继边为回边，将其与循环外的第一个节点相连 */
        private HashSet<Tuple<Vertex, Vertex>> addingEdges;
        private HashSet<Tuple<int, int>> addingEdgeIDs;
        public HashSet<Tuple<int, int>> AddingEdgeIDs
        { get { return this.addingEdgeIDs; } }

        /* 添加的节点ID，负数，分支合并的时候将新建一个节点作为汇聚点 */
        private int addingID;
        public int AddingID
        { get { return this.addingID; } }
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         */
        public Flow()
        {
            this.visitedNode = new Dictionary<int, Node>();
            this.path = new HashSet<int>();
            this.upRemovedEdges = new Dictionary<int, List<Tuple<string, int>>>();
            this.downRemovedEdges = new Dictionary<int, List<Tuple<string, int>>>();
            this.addingEdges = new HashSet<Tuple<Vertex, Vertex>>();
            this.addingEdgeIDs = new HashSet<Tuple<int, int>>();
            this.addingID = 0;
        }

        /* 建立流程，此类功能入口
         */
        public Block Build(Graph g)
        {
            if (g != null)
            {
                // 删除不规则边
                this.RemoveLoop(g.Start);
                foreach (Tuple<Vertex, Vertex> edge in this.addingEdges)
                {
                    g.AddEdge(edge.Item1, edge.Item2);
                }
                this.RemoveJump(g.Start);

                // 建立块信息
                Block block = new Block();
                Tuple<string, Vertex> e = new Tuple<string, Vertex>("", g.Start);
                while (e != null)
                {
                    block.AddBlock(this.BuildCompound(ref e));
                }
                block.AddNode(new Node(--this.addingID));

                foreach (Tuple<Vertex, Vertex> edge in this.addingEdges)
                {
                    this.addingEdgeIDs.Add(new Tuple<int, int>(edge.Item1.ID, edge.Item2.ID));
                }

                return block;
            }
            else
            {
                return null;
            }
        }

        /* DFS删除第一类不规则边
         * 同时添加辅助边
         */
        private void RemoveLoop(Vertex v)
        {
            if (!this.visitedNode.ContainsKey(v.ID))
            {
                this.path.Add(v.ID);
                List<Tuple<string, Vertex>> toRemove = new List<Tuple<string, Vertex>>();
                foreach (Tuple<string, Vertex> e in v.NextEdges)
                {
                    if (path.Contains(e.Item2.ID))
                    {   //发现环
                        if (!this.upRemovedEdges.ContainsKey(v.ID))
                            this.upRemovedEdges.Add(v.ID, new List<Tuple<string, int>>());
                        this.upRemovedEdges[v.ID].Add(new Tuple<string, int>(e.Item1, e.Item2.ID));
                        toRemove.Add(e);
                        if (v.OutDegree == 1 && e.Item2.OutDegree > 1)
                        {
                            foreach (Tuple<string, Vertex> ee in e.Item2.NextEdges)
                            {
                                if (!path.Contains(ee.Item2.ID))
                                {
                                    this.addingEdges.Add(new Tuple<Vertex, Vertex>(v, ee.Item2));
                                }
                            }
                        }
                    }
                    else
                    {   //递归
                        this.RemoveLoop(e.Item2);
                    }
                }
                foreach (Tuple<string, Vertex> e in toRemove)
                {   //删除Loop边
                    v.RemoveEdge(e);
                }
                this.path.Remove(v.ID);
                this.visitedNode.Add(v.ID, new Node(v.ID, v.Text));
            }
        }

        /* BFS删除第二类不规则边,此之前已删除回边
         */
        private void RemoveJump(Vertex start)
        {
            this.visitedNode.Clear();
            Queue<Vertex> queue = new Queue<Vertex>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Vertex v = queue.Dequeue();
                if (!this.VisitedNode.ContainsKey(v.ID))
                {
                    this.visitedNode.Add(v.ID, new Node(v.ID, v.Text));
                    List<Tuple<string, Vertex>> toRemove = new List<Tuple<string, Vertex>>();
                    foreach (Tuple<string, Vertex> e in v.NextEdges)
                    {
                        if (v.OutDegree > 1 && e.Item2.InDegree > 1)
                        {   //发现跳转语句
                            toRemove.Add(e);
                        }
                        queue.Enqueue(e.Item2);
                    }

                    foreach (Tuple<string, Vertex> e in toRemove)
                    {
                        if (v.OutDegree <= 1)
                            break;
                        if (!this.downRemovedEdges.ContainsKey(v.ID))
                            this.downRemovedEdges.Add(v.ID, new List<Tuple<string, int>>());
                        this.downRemovedEdges[v.ID].Add(new Tuple<string, int>(e.Item1, e.Item2.ID));
                        v.RemoveEdge(e);
                    }
                }
            }
        }


        /* 建立串行结构块
         * 结束标志：末尾节点，汇聚节点
         */
        private Block BuildCompound(ref Tuple<string, Vertex> e)
        {
            Block block = new Block();

            Tuple<string, Vertex> now = e;
            do
            {
                switch (now.Item2.OutDegree)
                {
                    case 0:     //到达末尾节点
                        block.AddNode(this.visitedNode[now.Item2.ID], now.Item1);
                        now = null;
                        break;
                    case 1:     //有唯一后继节点
                        block.AddNode(this.visitedNode[now.Item2.ID], now.Item1);
                        now = now.Item2.NextEdges[0];
                        break;
                    default:    //分支
                        block.AddBlock(this.BuildChoice(ref now));
                        break;
                }
                //末尾节点      汇聚节点
            } while (now != null && now.Item2.InDegree < 2);
            e = now;

            return block;
        }

        /* 建立分支结构块
         */
        private Block BuildChoice(ref Tuple<string, Vertex> e)
        {
            Block block = new Block();

            Node node = this.visitedNode[e.Item2.ID];
            block.AddNode(node, e.Item1);

            List<Tuple<string, Block>> nextBlocks = new List<Tuple<string, Block>>();
            Tuple<string, Vertex> tmpEdge = null;
            foreach (Tuple<string, Vertex> edge in e.Item2.NextEdges)
            {
                tmpEdge = edge;
                Block b = this.BuildCompound(ref tmpEdge);
                nextBlocks.Add(new Tuple<string, Block>(edge.Item1, b));
            }
            e = tmpEdge;
            block.AddBlocks(nextBlocks, --this.addingID);
            return block;
        }

        //
        //////////////////////////////////////////////////////
    }
}
