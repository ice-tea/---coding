/***********************************************************************
 * 类描述   ： 块信息，块类型只有顺序和分支，支持块的合并，包括顺序块的纵向合并
 *            以及分支块的横向合并
 *            
 *            每个块含有一个起始节点、结束节点以及宽高
 *
 * 作者     : snowingsea
 * 修改时间 ： 2012年9月18日
 ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2CFG.FlowChartElement
{
    public class Block
    {
        //////////////////////////////////////////////////////
        // 成员变量
        /* 垂直间距 */
        public readonly static double VERTICAL_SPACING = 0.5;
        /* 水平间距 */
        public readonly static double HORIZONTAL_SPACING = 1;

        /* 块的起始节点 */
        private Node start;
        public Node Start
        { get { return this.start; } }

        /* 块的结束节点 */
        private Node end;
        public Node End
        { get { return this.end; } }

        /* 块宽，由里面的节点大小多少决定 */
        private double width;
        public double Width
        { get { return this.width; } }

        /* 块高，同宽，由里面的节点大小多少决定 */
        private double height;
        public double Height
        { get { return this.height; } }
        //
        //////////////////////////////////////////////////////

        //////////////////////////////////////////////////////
        // 成员函数

        /* 构造函数
         */
        public Block()
        {
            this.start = this.end = null;
            this.height = this.width = 0;
        }

        /* 将一个节点加入到Block中，txt为边文字信息
         */
        public void AddNode(Node node, string txt = "")
        {
            if (node == null) return;

            if (this.Start == null)
            {   // 此块为空，填充之
                this.start = this.end = node;
                this.height = node.Height;
                this.width = node.Width;
            }
            //else if (this.End.ID < 0)
            //{
            //    node.SetOffset(0, this.End.Offset_Y);

            //    this.height += node.Height - this.End.Height;

            //    double offsetX = 0;
            //    if (this.Width < node.Width)
            //    {
            //        offsetX = (node.Width - this.Width) / 2;
            //        this.width = node.Width;
            //    }
            //    double offsetY = node.Height - this.End.Height;
            //    this.start.AddOffset(offsetX, offsetY);
            //    this.end = node;
            //}
            else
            {
                double offsetY = (this.end.Height + node.Height) / 2 + Block.VERTICAL_SPACING;
                node.SetOffset(0, -offsetY);

                this.height += Block.VERTICAL_SPACING + node.Height;
                double offsetX = 0;
                if (this.Width < node.Width)
                {
                    offsetX = (node.Width - this.Width) / 2;
                    this.width = node.Width;
                }
                offsetY = node.Height + Block.VERTICAL_SPACING;
                this.start.AddOffset(offsetX, offsetY);
                this.end.AddNode(node, txt);
                this.end = node;
            }
        }

        /* 添加顺序后继块，添加一个表示添加的块和原块为顺序关系
         * txt为连接边的文字信息
         */
        public void AddBlock(Block block, string txt = "")
        {
            if (this.Start == null)
            {   // 此块为空，同添加节点，填充之
                this.start = block.start;
                this.end = block.end;
                this.height = block.Height;
                this.width = block.Width;
            }
            else
            {
                double offsetY = (this.end.Height + block.Start.Height) / 2 + Block.VERTICAL_SPACING;
                block.Start.SetOffset(0, -offsetY);

                offsetY = block.Height + Block.VERTICAL_SPACING;
                double offsetX = 0;
                if (this.Width < block.Width)
                {
                    offsetX = (block.Width - this.Width) / 2;
                    this.width = block.Width;
                }
                this.height += offsetY;
                this.start.AddOffset(offsetX, offsetY);
                this.end.AddNode(block.Start, txt);
                this.end = block.End;
            }
        }

        /* 添加分支后继块
         * 将所有分支合并并加入到原块中，最后指向一个新建的节点，其id为endID，
         * 作为分支的汇聚点
         */
        public void AddBlocks(List<Tuple<string, Block>> blocks, int endID)
        {
            Node endNode = new Node(endID);
            double w = -Block.HORIZONTAL_SPACING;
            double h = 0;
            foreach (Tuple<string, Block> tuple in blocks)
            {
                w += tuple.Item2.Width + Block.HORIZONTAL_SPACING;
                h = Math.Max(h, tuple.Item2.Height);
            }
            double offsetX = 0;
            double offsetY = h + Block.VERTICAL_SPACING * 2 + endNode.Height;
            double tmpWidth = Math.Max(w, endNode.Width);
            if (this.Width < tmpWidth)
            {
                offsetX = (tmpWidth - this.Width) / 2;
                this.width = tmpWidth;
            }
            this.height += offsetY;
            //更新原块起点的偏移量
            this.start.AddOffset(offsetX, offsetY);

            offsetX = -w / 2;
            bool first = true;
            foreach (Tuple<string, Block> tuple in blocks)
            {   //更新每个分支块起点偏移量并加入到原块中
                Block b = tuple.Item2;
                offsetX += b.Width / 2;
                offsetY = (this.End.Height + b.Start.Height) / 2 + Block.VERTICAL_SPACING;
                b.Start.SetOffset(offsetX, -offsetY);
                this.End.AddNode(b.Start, tuple.Item1);

                offsetY = h - b.height + (b.End.Height + endNode.Height) / 2 + Block.VERTICAL_SPACING;
                if (first)  //只在第一次更新汇聚点的偏移量，即其绝对位置由第一个块决定
                    endNode.SetOffset(-offsetX, -offsetY);
                first = false;
                b.End.AddNode(endNode);

                offsetX += b.Width / 2 + Block.HORIZONTAL_SPACING;
            }

            this.end = endNode;
        }
        //
        //////////////////////////////////////////////////////
    }
}
