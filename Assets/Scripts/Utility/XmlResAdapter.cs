using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
namespace Utility
{
    /// <summary>
    /// xml加载器
    /// </summary>
    public static class XmlResAdapter
    {
        public static XmlDocument GetXmlDocument(string xmlStirng)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStirng);
            return doc;
        }
        /// <summary>
        /// 将xml内容转换成map
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<string, string>> LoadXMLToMap(XmlDocument doc, string content)
        {
            var result = new Dictionary<int, Dictionary<string, string>>();
            int index = 0;
            foreach (XmlNode item in doc.SelectSingleNode("root").ChildNodes)
            {
                index++;
                if (item.ChildNodes == null || item.ChildNodes.Count == 0)
                {
                    continue;
                }
                int key = int.Parse(item.ChildNodes[0].InnerText);
                
                if (result.ContainsKey(key))
                {
                    continue;
                }
                var children = new Dictionary<string, string>();
                result.Add(key, children);
                for (int i = 1; i < item.ChildNodes.Count; i++)
                {
                    XmlNode node = item.ChildNodes[i];
                    string tag = null;
                    if (node.Name.Length < 3)
                    {
                        tag = node.Name;
                    }
                    else 
                    {
                        string tagTial = node.Name.Substring(node.Name.Length - 2, 2);
                        if (tagTial == "_i" || tagTial == "_s" || tagTial == "_f" || tagTial == "_l" || tagTial == "_k" || tagTial == "_m")
                        {
                            tag = node.Name.Substring(0, node.Name.Length - 2);
                        }
                        else 
                        {
                            tag = node.Name;
                        }
                    }
                    if (node != null && !children.ContainsKey(tag))
                    {
                        if (string.IsNullOrEmpty(node.InnerText))
                        {
                            children.Add(tag, "");
                        }
                        else
                        {
                            children.Add(tag, node.InnerText.Trim());
                        }
                    }
                }
            }
            return result;
        }
    }
}