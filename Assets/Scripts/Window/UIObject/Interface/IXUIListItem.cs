using UnityEngine;
using System.Collections.Generic;
public interface IXUIListItem :IXUIObject
{
    int Id
    {
        get;
        set;
    }
    long GUID
    {
        get;
        set;
    }
    int Index
    {
        get;
        set;
    }
    bool IsSelected
    {
        get;
        set;
    }
    Dictionary<string, XUIObjectBase> AllXUIObject
    {
        get;
    }
    bool SetText(string strId,string strContent);
    void SetSelected(bool bTrue);
    void SetEnableSelect(bool bEnable);
    void SetIconSprite(string strSprite);
    void SetIconSprite(string strSprite, string strAtlas);
    void SetIconTexture(string strTexture);
    void SetColor(Color color);
    void SetEnable(bool bEnable);
    void Clear();
}
