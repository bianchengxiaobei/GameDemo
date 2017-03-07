using UnityEngine;
using System.Collections.Generic;
using Game.Common;
/// <summary>
/// UI界面加载类
/// </summary>
public class LoadUIResource
{
    /// <summary>
    /// 加载过的缓存字典
    /// </summary>
    public static Dictionary<string, GameObject> m_LoadResDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 实例化资源
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameObject LoadRes(Transform parent, string path)
    {
        if (CheckResInDic(path))
        {
            GameObject asset = null;
            m_LoadResDic.TryGetValue(path, out asset);
            if (asset != null)
            {
                return asset;
            }
            else 
            {
                m_LoadResDic.Remove(path);
            }
        }
        GameObject obj = null;
        ResourceUnit objUnit = ResourceManager.Instance.LoadImmediate(path, ResourceType.PREFAB);
        if (objUnit == null || objUnit.Asset == null)
        {
            Debug.LogError("加载资源失败：" + path);
            return null;
        }
        obj = GameObject.Instantiate(objUnit.Asset) as GameObject;
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        m_LoadResDic.Add(path, obj);
        return obj;
    }
    /// <summary>
    /// 销毁资源
    /// </summary>
    /// <param name="obj"></param>
    public static void DestroyLoad(GameObject obj)
    {
        if (m_LoadResDic.Count == null || obj == null)
        {
            return;
        }
        foreach (var key in m_LoadResDic.Keys)
        {
            GameObject objLoad;
            if (m_LoadResDic.TryGetValue(key, out objLoad) && obj == objLoad)
            {
                GameObject.DestroyImmediate(obj);
                m_LoadResDic.Remove(key);
                break;
            }
        }
    }
    /// <summary>
    /// 检查是否已经包含该资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool CheckResInDic(string path)
    {
        if (m_LoadResDic == null && m_LoadResDic.Count == 0)
        {
            return false;
        }
        return m_LoadResDic.ContainsKey(path);
    }
}
