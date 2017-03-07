using UnityEngine;
using System.Collections;
using Game.Common;
[AddComponentMenu("XUI/XUISprite")]
public class XUISprite : XUIObject,IXUIObject,IXUISprite
{
    private string m_spriteNameCached = string.Empty;
    private string m_atlasNameCached = string.Empty;
    private UISprite m_uiSprite;
    private UISpriteAnimation m_uiSpriteAnimation;
    private ResourceUnit m_oAltasUnit;
    private IXLog m_log = XLog.GetLog<XUISprite>();
    public override float Alpha
    {
        get
        {
            if (null != this.m_uiSprite)
            {
                return this.m_uiSprite.alpha;
            }
            return 0f;
        }
        set
        {
            if (null != this.m_uiSprite)
            {
                this.m_uiSprite.alpha = value;
            }
        }
    }
    public Color Color 
    {
        get 
        {
            if (this.m_uiSprite != null)
            {
                return this.m_uiSprite.color;
            }
            return Color.white;
        }
        set 
        {
            if (this.m_uiSprite != null)
            {
                this.m_uiSprite.color = value;
            }
        }
    }
    public string SpriteName
    {
        get
        {
            if (null != this.m_uiSprite)
            {
                return this.m_uiSprite.spriteName;
            }
            return null;
        }
    }
    public IXUIAtlas UIAtlas 
    {
        get 
        {
            if (null == this.m_uiSprite)
            {
                return null;
            }
            if (null == this.m_uiSprite.atlas)
            {
                return null;
            }
            return this.m_uiSprite.atlas.GetComponent<XUIAtlas>();
        }
    }


    public override void Init()
    {
        base.Init();
        this.m_uiSprite = base.GetComponent<UISprite>();
        if (null == this.m_uiSprite)
        {
            Debug.LogError("null == m_uiSprite");
        }
        this.m_uiSpriteAnimation = base.GetComponent<UISpriteAnimation>();
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="bLoop"></param>
    /// <returns></returns>
    public bool PlayFlash(bool bLoop)
    {
        if (null == this.m_uiSpriteAnimation)
        {
            return false;
        }
        this.m_uiSpriteAnimation.loop = bLoop;
        this.m_uiSpriteAnimation.ResetToBeginning();
        this.m_uiSpriteAnimation.enabled = true;
        this.m_uiSprite.enabled = true;
        return true;
    }
    /// <summary>
    /// 停止动画
    /// </summary>
    /// <returns></returns>
    public bool StopFlash()
    {
        if (null == this.m_uiSpriteAnimation)
        {
            return false;
        }
        this.m_uiSpriteAnimation.enabled = false;
        return true;
    }
    /// <summary>
    /// 设置UISprite
    /// </summary>
    /// <param name="strSprite"></param>
    /// <param name="strAltas"></param>
    /// <returns></returns>
    public bool SetSprite(string strSprite, string strAltas)
    {
        if (null == this.m_uiSprite)
        {
            return false;
        }
        //如果缓存图集已经存在了，那么就直接设置
        if (this.m_atlasNameCached.Equals(strAltas))
        {
            this.m_spriteNameCached = strSprite;
            this.m_uiSprite.spriteName = strSprite;
            this.m_uiSprite.enabled = true;
            return true;
        }
        this.m_spriteNameCached = strSprite;
        this.m_atlasNameCached = strAltas;
        this.PrepareAtlas(strAltas, strSprite);
        return true;
    }
    /// <summary>
    /// 设置默认图集的UISprite
    /// </summary>
    /// <param name="strSpriteName"></param>
    /// <returns></returns>
    public bool SetSprite(string strSpriteName)
    {
        if (null == this.m_uiSprite)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(this.m_atlasNameCached))
        {
            this.SetSprite(this.m_atlasNameCached, strSpriteName);
        }
        else 
        {
            this.m_uiSprite.spriteName = strSpriteName;
        }
        return true;
    }
    public void SetEnable(bool bEnable)
    {
        Collider compent = base.GetComponent<Collider>();
        if (compent != null)
        {
            compent.enabled = bEnable;
        }
    }
    private void OnDestroy()
    {
        if (this.m_oAltasUnit != null)
        {
            this.m_oAltasUnit.Dispose();
            this.m_oAltasUnit = null;
        }
    }
    private void PrepareAtlas(string strAltas, string strSprite)
    {
        m_oAltasUnit = ResourceManager.Instance.LoadImmediate(strAltas, ResourceType.PREFAB);
        GameObject obj = m_oAltasUnit.Asset as GameObject;
        UIAtlas uiAtals = obj.GetComponent<UIAtlas>();
        if (null == uiAtals)
        {
            this.m_log.Error("加载图集的时候出错!!");
            return;
        }
        this.m_uiSprite.atlas = uiAtals;
        this.m_uiSprite.spriteName = strSprite;
        this.m_uiSprite.enabled = true;
    }
}
