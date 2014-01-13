using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax
{
    public enum Modifier
    {
        _extern,
        _static,
        _const,
        _volatile,
    }

    public abstract class CEntity
    {
        public CEntity Parent { get; set; }

        /* 名称 */
        public string Name { set; get; }

        /* 标识符 */
        public abstract string Identifier();

        /* 三种修饰符 */
        private HashSet<Modifier> modifiers = new HashSet<Modifier>();

        public bool IsExtern
        {
            get
            {
                return this.modifiers.Contains(Modifier._extern);
            }
            set
            {
                if (value)
                    this.modifiers.Add(Modifier._extern);
                else
                    this.modifiers.Remove(Modifier._extern);
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.modifiers.Contains(Modifier._static);
            }
            set
            {
                if (value)
                    this.modifiers.Add(Modifier._static);
                else
                    this.modifiers.Remove(Modifier._static);
            }
        }

        public bool IsConst
        {
            get
            {
                return this.modifiers.Contains(Modifier._const);
            }
            set
            {
                if (value)
                    this.modifiers.Add(Modifier._const);
                else
                    this.modifiers.Remove(Modifier._const);
            }
        }

        public bool IsVolatile
        {
            get
            {
                return this.modifiers.Contains(Modifier._volatile);
            }
            set
            {
                if (value)
                    this.modifiers.Add(Modifier._volatile);
                else
                    this.modifiers.Remove(Modifier._volatile);
            }
        }
    }
}
