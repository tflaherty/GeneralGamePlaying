using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.Utilities.TFTree
{
    public class TFForest<T> where T:class
    {
        public List<TFTree<T>> Trees = null;

        public TFTree<T> AddNewTree()
        {
            return AddTree(new TFTree<T>());
        }

        public TFTree<T> AddTree(TFTree<T> tfTree)
        {
            if (Trees == null)
            {
                Trees = new List<TFTree<T>>();
            }
            Trees.Add(tfTree);
            return tfTree;
        }

        public bool RemoveTree(TFTree<T> tree)
        {
            if (Trees != null && Trees.Contains(tree))
            {
                Trees.Remove(tree);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
