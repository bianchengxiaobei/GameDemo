using UnityEngine;
using System.Collections;
/// <summary>
/// 所有UI物体公共的接口
/// </summary>
public interface IXUIObject
{
    Transform CacheTransform
    {
        get;
    }
    GameObject CacheGameObject
    {
        get;
    }
    IXUIObject Parent
    {
        get;
        set;
    }
    string Tip
    {
        get;
        set;
    }
    object TipParam
    {
        get;
        set;
    }
    bool IsVisible();
    void SetVisible(bool bVisible);
    IXUIObject GetUIObject(string strPath);
    void Highlight(bool bTrue);
}
