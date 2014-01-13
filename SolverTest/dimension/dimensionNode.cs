using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace UnitConfigure
{
    public class dimensionNode
    {
        private string name;
        public ArrayList dimension;
        public float coefficient;
        public float offset;
        public dimensionNode()
        {
            dimension = new ArrayList();
        }
        public void setDimensionName(string name)
        {
            this.name = name;

        }
        public void addDimension(float dimension)
        {
            this.dimension.Add(dimension);
        }
        public void addCoefficientAndOffset(float coefficient, float offset)
        {
            this.coefficient = coefficient;
            this.offset = offset;
        }

    }

    /*
    class dimensionNode
    {
        private string name;
        public ArrayList dimension;
        public ArrayList coefficient;
        public ArrayList offset;
        public dimensionNode()
        {
            dimension = new ArrayList();
            coefficient = new ArrayList();
            offset = new ArrayList();
        }
        public void setDimensionName(string name)
        {
            this.name = name;

        }
        public void addDimension(float dimension,float coefficient,float offset)
        {
            this.dimension.Add(dimension);
            this.coefficient.Add(coefficient);
            this.offset.Add(offset);
        }

    }
    */
}