using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using Game;
using Utility;
/// <summary>
/// 字符串管理器
/// </summary>
public class StringConfigManager : Singleton<StringConfigManager>
{
    //所有字符缓存字典
    private Dictionary<string, string> m_oDicAllStringData;
    public StringConfigManager()
    {
        this.m_oDicAllStringData = new Dictionary<string, string>();
    }
    public void Init()
    {
        this.m_oDicAllStringData.Clear();
        XmlDocument doc = XmlResAdapter.GetXmlDocument(UnityTools.LoadFileText(SystemConfig.StringPath));
        OnLoadStringFinishedEventHandler(doc);
    }
    /// <summary>
    /// 根据字符id取得字符串内容
    /// </summary>
    /// <param name="id"></param>
    public static string GetString(string id)
    {
        return StringConfigManager.singleton.TryGetString(id);
    }
    private void OnLoadStringFinishedEventHandler(XmlDocument doc)
    {
        if (doc != null)
        {
            XmlNode root = doc.SelectSingleNode("root");
            XmlNodeList tableList = root.ChildNodes;
            foreach (var node in tableList)
            {
                XmlElement ele = (XmlElement)node;
                string id = ele.GetAttribute("id");
                string content = ele.InnerText;
                bool flag = this.m_oDicAllStringData.ContainsKey(id);
                if (flag)
                {
                    Debug.Log("字符字典已经包含该字符id");
                }
                this.m_oDicAllStringData[id] = content;
            }
        }
    }
    private string TryGetString(string id)
    {
        if (this.m_oDicAllStringData.ContainsKey(id))
        {
            return this.m_oDicAllStringData[id];
        }
        else
        {
            return id;
        }
    }
}
