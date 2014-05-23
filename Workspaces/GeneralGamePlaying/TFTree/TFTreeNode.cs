using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace API.Utilities.TFTree
{
    public class TFTreeNode<T> : IXmlSerializable where T:class
    {
        // This empty list is return by all nodes that have no childen.
        // It's a singleton so it only exists once and so it's use prevents
        // each node with no children from having its own empty child list
        // and wasting lots of memory!!
        static protected readonly List<TFTreeNode<T>> EmptyList = new List<TFTreeNode<T>>();

        public TFTreeNode<T> Parent { get; set; }
        public T Data { get; set; }

        protected List<TFTreeNode<T>> _Children = null;
        public List<TFTreeNode<T>> Children
        {
            get { return _Children ?? EmptyList; }
        }    

        public TFTreeNode(T data)
        {
            Data = data;
        }

        public TFTreeNode<T> AddChild(TFTreeNode<T> child)
        {
            if (_Children == null)
            {
                _Children = new List<TFTreeNode<T>>();
            }
            Children.Add(child);
            return child;            
        }

        public TFTreeNode<T> AddNewChild(T data)
        {
            return AddChild(new TFTreeNode<T>(data));
        }

        public bool RemoveChild(TFTreeNode<T> child)
        {
            if (Children.Contains(child))
            {
                Children.Remove(child);
                if (!Children.Any())
                {
                    _Children = null;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public IEnumerable<TFTreeNode<T>> DepthFirstTopDown(int maxLevel)
        {
            yield return this;
            if (maxLevel > 0)
            {
                foreach (TFTreeNode<T> child in Children)
                {
                    foreach (TFTreeNode<T> node in child.DepthFirstTopDown(maxLevel-1))
                    {
                        yield return node;
                    }
                }                
            }
        }

        public IEnumerable<TFTreeNode<T>> DepthFirstTopDown()
        {
            yield return this;
            foreach (TFTreeNode<T> child in Children)
            {
                foreach (TFTreeNode<T> node in child.DepthFirstTopDown())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<TFTreeNode<T>> BreadthFirstTopDown()
        {
            var queue = new Queue<TFTreeNode<T>>();
            queue.Enqueue(this);
            while (queue.Any())
            {
                var node = queue.Dequeue();
                yield return node;

                foreach (TFTreeNode<T> child in node.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<TFTreeNode<T>> BreadthFirstTopDown(int level)
        {
            var queue = new Queue<TFTreeNode<T>>();
            queue.Enqueue(this);
            while (queue.Any())
            {
                var node = queue.Dequeue();
                yield return node;

                if (level > 0)
                {
                    foreach (TFTreeNode<T> child in node.Children)
                    {
                        queue.Enqueue(child);
                    }
                    level--;
                }
            }
        }

        public IEnumerable<TFTreeNode<T>> DepthFirstTopDownRightRecursiveCollapse(Func<TFTreeNode<T>, bool> isNodeTypeToCollapse)
        {
            if (!isNodeTypeToCollapse(this))
            {
                yield return this;
            }

            foreach (TFTreeNode<T> child in Children)
            {
                foreach (TFTreeNode<T> node in child.DepthFirstTopDownRightRecursiveCollapse(isNodeTypeToCollapse))
                {
                    yield return node;
                }
            }
        }


        // ToDo: Make this return a list of TFTreeNodes in case the collapsed tree has a node
        // at the top that we throw out but has more than one child!
        public TFTreeNode<T> CreateCollapsedTree(Func<TFTreeNode<T>, bool> isNodeTypeToCollapse)
        {
            TFTreeNode<T> root = null;
            TFTreeNode<T> lowerRoot = null;

            if (!isNodeTypeToCollapse(this))
            {
                root = new TFTreeNode<T>(this.Data);
            }

            bool isFirst = true;
            foreach (TFTreeNode<T> child in Children)
            {
                var tmp = child.CreateCollapsedTreeRecursive(isNodeTypeToCollapse, root);
                if (isFirst)
                {
                    isFirst = false;
                    lowerRoot = tmp;
                }
            }

            return root ?? lowerRoot;
        }


        protected TFTreeNode<T> CreateCollapsedTreeRecursive(Func<TFTreeNode<T>, bool> isNodeTypeToCollapse, TFTreeNode<T> parentNode)
        {
            TFTreeNode<T> retNode = parentNode;

            if (!isNodeTypeToCollapse(this))
            {
                var copiedNode = new TFTreeNode<T>(this.Data);

                if (parentNode == null)
                {
                    retNode = copiedNode;
                }
                else
                {
                    parentNode.AddChild(copiedNode);                    
                }

                foreach (TFTreeNode<T> child in Children)
                {
                    child.CreateCollapsedTreeRecursive(isNodeTypeToCollapse, copiedNode);
                }
            }
            else
            {
                bool isFirst = true;
                foreach (TFTreeNode<T> child in Children)
                {
                    var tmp = child.CreateCollapsedTreeRecursive(isNodeTypeToCollapse, parentNode);
                    if (isFirst)
                    {
                        isFirst = false;
                        retNode = tmp;
                    }
                }
            }

            return retNode;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Node");
            if (Data != null)
            {
                var dataXML = Data as IXmlSerializable;
                if (dataXML != null)
                {
                    dataXML.WriteXml(writer);                    
                }
            }

            if (Children != null)
            {
                foreach (TFTreeNode<T> tfTreeNode in Children)
                {
                    tfTreeNode.WriteXml(writer);
                }
            }
            writer.WriteEndElement();
        }
    }
}
