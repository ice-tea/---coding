using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFrontendParser.CSyntax.util
{
    public class CEntityCollection<TT>
        where TT: CEntity
    {
        private List<TT> list = new List<TT>();
        private Dictionary<string, TT> dict = new Dictionary<string, TT>();

        public int Count { get { return this.dict.Count; } }

        public bool CEntityExists(string id)
        {
            if (dict.ContainsKey(id))
                return true;
            else return false;
        }

        public TT GetCEntity(string id)
        {
            TT entity;
            if (dict.TryGetValue(id, out entity))
            {
                return entity;
            }
            else
            {
                return null;
            }
        }

        public void RemoveCEntity(string id)
        {
            TT item = this.GetCEntity(id);
            if (item != null)
            {
                this.dict.Remove(id);
                this.list.Remove(item);
            }
        }

        public void AddCEntity(TT item)
        {
            if (this.list.Contains(item) == false)
                this.list.Add(item);
            string key = item.Identifier();
            if (dict.ContainsKey(key) == false)
                dict.Add(item.Identifier(), item);
        }

        public List<TT> CEntityList
        {
            get
            {
                return this.list;
            }
        }

        public HashSet<TT> CEntities
        {
            get
            {
                return new HashSet<TT>(this.dict.Values);
            }
        }        
    }
}
