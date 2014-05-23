using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace API.Utilities.TFTree
{
    public class TFTree<T> : IXmlSerializable where T:class
    {
        public string XSLTFile { get; set; }
        public TFTreeNode<T> Root;
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Root != null)
            {
                if (!String.IsNullOrEmpty(XSLTFile))
                {
                    writer.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"" + XSLTFile + "\"?>");                    
                }

                Root.WriteXml(writer);
            }
        }
    }
}
