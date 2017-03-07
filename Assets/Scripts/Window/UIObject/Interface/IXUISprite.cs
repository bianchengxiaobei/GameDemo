using UnityEngine;
using System.Collections;

public interface IXUISprite : IXUIObject
{
    Color Color
    {
        get;
        set;
    }
    string SpriteName
    {
        get;
    }
    IXUIAtlas UIAtlas
    {
        get;
    }
    /// <summary>
    /// 播放UI动画
    /// </summary>
    /// <param name="bLoop"></param>
    /// <returns></returns>
    bool PlayFlash(bool bLoop);
    void SetEnable(bool bEnable);
    /// <summary>
    /// 设置UISprite
    /// </summary>
    /// <param name="strSpriteName"></param>
    /// <returns></returns>
    bool SetSprite(string strSpriteName);
    bool SetSprite(string strSpriteName, string strAtlas);
    /// <summary>
    /// 停止UI动画
    /// </summary>
    /// <returns></returns>
    bool StopFlash();
}
