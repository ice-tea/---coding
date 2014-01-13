using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2CFG.FlowChartElement
{
    public class Node
    {
        //////////////////////////////////////////////////////
        // 成员变量 
        /* 节点编号，唯一 */
        private int id;
        public int ID
        { get { return this.id; } }

        /* 节点上的文字信息 */
        public string Text
        { get; set; }

        /* 后继节点列表，重载 */
        private List<Tuple<string, Node>> successors;
        public List<Tuple<string, Node>> NextEdges
        { get { return this.successors; } }
        /* 节点宽度，由节点性质以及节点内容多少决定 */
        private double width;
        public double Width
        { get { return this.width; } }

        /* 节点高度，同宽度，由节点性质以及节点内容多少决定 */
        private double height;
        public double Height
        { get { return this.height; } }

        /* 偏移量x，相对于父亲节点在X轴上的偏移量 */
        private double offset_x;
        public double Offset_X
        { get { return this.offset_x; } }

        /* 偏移量y，相对于父亲节点在Y轴上的偏移量 */
        private double offset_y;
        public double Offset_Y
        { get { return this.offset_y; } }
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         */
        //TODO: width和height初始化的值需再议！！！！！！！！！！！！！！！！
        public Node(int id, string txt = "")
        {
            this.id = id;
            this.Text = txt;

            this.successors = new List<Tuple<string, Node>>();
            this.offset_x = this.offset_y = 0;
            this.width = 1;
            this.height = 0.5;
        }

        /* 添加后继节点，实质上为加边
         */
        public void AddNode(Node node, string txt = "skip")
        {
            if (node == null) return;

            this.successors.Add(new Tuple<string, Node>(txt, node));
        }

        /* 删除后继边
         */
        public void RemoveEdge(int nextNodeID)
        {
            foreach (Tuple<string, Node> edge in this.successors)
            {
                if (edge.Item2.ID == nextNodeID)
                {
                    this.successors.Remove(edge);
                    break;
                }
            }
        }

        /* 设置偏移量 */
        public void SetOffset(double x, double y)
        {
            this.offset_x = x;
            this.offset_y = y;
        }

        /* 添加偏移量 */
        public void AddOffset(double x, double y)
        {
            this.offset_x += x;
            this.offset_y += y;
        }

        /* 重载ToString函数
         */
        public override string ToString()
        {
            return this.ID + ". " + this.Text + "\n";
        }
        /* 重载GetHashCode函数，哈希用
         */
        public override int GetHashCode()
        {
            return this.ID; //因为ID是唯一的
        }
        //
        //////////////////////////////////////////////////////
    }
}
