using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace QuickModel
{
    /// <summary>
    /// 容差字典扩展
    /// </summary>
    public class ToleranceDic : Dictionary<string, double>, IXmlSerializable
    {
        private const string m_useNodeName = "DicNode";

        private const string m_useKeyNodeName = "KeyNode";

        private const string m_useValueNodeName = "ValueNode";

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 读取方法
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(double));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {

                reader.ReadStartElement(m_useNodeName);

                reader.ReadStartElement(m_useKeyNodeName);
                string key = (string)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement(m_useValueNodeName);
                double value = (double)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);
                reader.ReadEndElement();

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// 写出方法
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(double));
            foreach (string key in this.Keys)
            {
                writer.WriteStartElement(m_useNodeName);

                writer.WriteStartElement(m_useKeyNodeName);
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement(m_useValueNodeName);
                double value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
