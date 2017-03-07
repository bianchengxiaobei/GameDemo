using UnityEngine;
using System.Collections.Generic;
using Game;
using Game.Common;
/// <summary>
/// 资源加载管理器
/// </summary>
public class ResourceManager : UnitySingleton<ResourceManager>
{
    public bool UsedAssetBundle = false;//是否使用ab加载

    private bool m_Init = false;
    private Dictionary<string, ResourceUnit> m_LoadedResourceUnit = new Dictionary<string,ResourceUnit>();

    public void Init()
    {
        if (UsedAssetBundle)
        {

        }
        this.m_Init = true;
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public ResourceUnit LoadImmediate(string filePath,ResourceType type)
    {
        if (UsedAssetBundle)
        {
            return null;
        }
        else
        {
            Object asset = Resources.Load(filePath);
            ResourceUnit resource = new ResourceUnit(null,0,asset,null,type);
            return resource;
        }
    }

    public void Update()
    {
        if (!this.m_Init)
        {
            return ;
        }
    }
}
