using UnityEngine;
using System.Collections.Generic;
using UILib;
using Game.Common;
[AddComponentMenu("XUI/XUIListItem")]
public class XUIListItem : XUIObject,IXUIObject,IXUIListItem
{
    private UISprite m_uiSpriteIcon;
    private UILabel[] m_uiLabels;
    private UITexture m_uiTextureIcon;
    private UIButton m_uiButton;
    private UIToggle m_uiToggle;
    private UITexture[] m_uiTextures;
    private UISprite[] m_uiSprites;
    private XUISprite m_uiSpriteFlash;
    private Dictionary<string, XUIObjectBase> m_dicId2UIObject = new Dictionary<string, XUIObjectBase>();
    protected Collider m_colldier;
    private Color m_highlightColor = Color.clear;
    public int m_unId;
    private int m_unIndex = -1;
    private long m_GUID;


    public int Id 
    {
        get { return this.m_unId; }
        set 
        { 
            this.m_unId = value;
        }
    }
    public int Index 
    {
        get { return this.m_unIndex; }
        set { this.m_unIndex = value; }
    }
    public long GUID 
    {
        get { return this.m_GUID; }
        set { this.m_GUID = value; }
    }
    public XUIList ParentXUIList 
    {
        get 
        {
            XUIList xUIlist = this.Parent as XUIList;
            if (null == xUIlist)
            {
                Debug.LogError("null == uiList");
            }
            return xUIlist;
        }
    }
    public bool IsSelected 
    {
        get 
        {
            return this.m_uiToggle != null && this.m_uiToggle.value;
        }
        set
        {
            if (this.m_uiToggle != null && this.m_uiToggle.value != value)
            {
                this.m_uiToggle.Set(value);
                if (value)
                {
                    this.ParentXUIList.SelectItem(this, false);
                }else
                {
                    this.ParentXUIList.UnSelectItem(this, false);
                }
            }
        }
    }
    public Dictionary<string, XUIObjectBase> AllXUIObject
    {
        get { return this.m_dicId2UIObject; }
    }

    public void SetSelected(bool bTrue)
    {
        if (this.m_uiToggle != null && this.m_uiToggle.value != bTrue)
        {
            this.m_uiToggle.Set(bTrue);
        }
    }
    public void SetEnableSelect(bool bEnable)
    {
        if (!bEnable && this.m_uiToggle != null)
        {
            this.m_uiToggle.value = false;
        }
        this.Highlight(bEnable);
        if (this.m_uiToggle != null)
        {
            this.m_uiToggle.enabled = bEnable;
        }
    }
    private void OnSelectStateChange()
    {
        bool bSelected = this.m_uiToggle.value;
        if (bSelected)
        {
            //List选择该item
            this.ParentXUIList.OnSelectItem(this);
        }
        else 
        {
            //List不选择该item
            this.ParentXUIList.OnUnSelectItem(this);
        }
    }
    public void SetIconSprite(string strSprite)
    {
        if (null != this.m_uiSpriteIcon)
        {
            this.m_uiSpriteIcon.spriteName = strSprite.Substring(strSprite.LastIndexOf("\\") + 1);
            this.m_uiSpriteIcon.enabled = true;
        }
        XUISprite xUISprite = this.GetUIObject("Sprite_Icon") as XUISprite;
        if (null != xUISprite)
        {
            xUISprite.SetSprite(strSprite);
        }
    }
    public void SetIconSprite(string strSprite, string strAtlas)
    {
        XUISprite xUISprite = this.GetUIObject("Sprite_Icon") as XUISprite;
        if (null != xUISprite)
        {
            xUISprite.SetSprite(strSprite, strAtlas);
        }
    }
    public void SetIconTexture(string strTexture)
    {
        XUITexture xUITexture = this.GetUIObject("Texture_Icon") as XUITexture;
        if (xUITexture != null)
        {
            xUITexture.SetTexture(strTexture);
        }
    }
    public bool SetText(string strId, string strText)
    {
        IXUILabel label = this.GetUIObject(strId) as IXUILabel;
        if (label != null)
        {
            label.SetText(strText);
            return true;
        }
        return false;
    }
    public void SetColor(Color color)
    {
        if (this.m_uiSpriteIcon != null)
        {
            this.m_uiSpriteIcon.color = color;
        }
        if (this.m_uiTextureIcon != null)
        {
            this.m_uiTextureIcon.color = color;
        }
    }
    public void SetEnable(bool bEnable)
    {
        if (this.m_colldier != null)
        {
            this.m_colldier.enabled = bEnable;
        }
        if (this.m_uiButton != null)
        {
            this.m_uiButton.enabled = bEnable;
        }
    }
    public void Clear()
    {
        this.m_unId = 0;
        UILabel[] uiLabels = this.m_uiLabels;
        for (int i = 0; i < uiLabels.Length; i++)
        {
            UILabel uILabel = uiLabels[i];
            uILabel.text = string.Empty;
        }
        if (null != this.m_uiSpriteIcon)
        {
            this.m_uiSpriteIcon.enabled = false;
        }
        if (null != this.m_uiTextureIcon)
        {
            this.m_uiTextureIcon.enabled = false;
        }
        this.Tip = string.Empty;
        this.TipParam = null;
    }
    public override void Init()
    {
        base.Init();
        WidgetFactory.FindAllUIObjects(base.transform, this, ref m_dicId2UIObject);
        foreach (var uiObject in this.m_dicId2UIObject.Values)
        {
            uiObject.Parent = this;
            if (!uiObject.IsInited)
            {
                uiObject.Init();
            }
        }
        if (null == this.m_uiSpriteIcon)
        {
            Transform tran = base.transform.FindChild("Sprite_Icon");
            if (tran != null)
            {
                this.m_uiSpriteIcon = tran.GetComponent<UISprite>();
            }
        }
        if (null == this.m_uiTextureIcon)
        {
            Transform tran1 = base.transform.FindChild("Texture_Icon");
            if (tran1 != null)
            {
                this.m_uiTextureIcon = tran1.GetComponent<UITexture>();
            }
        }
        this.m_colldier = base.GetComponent<Collider>();
        this.m_uiLabels = base.GetComponentsInChildren<UILabel>();
        this.m_uiToggle = base.GetComponent<UIToggle>();
        this.m_uiButton = base.GetComponent<UIButton>();
        this.m_uiSprites = base.GetComponentsInChildren<UISprite>(true);
        this.m_uiTextures = base.GetComponentsInChildren<UITexture>(true);
        this.m_uiSpriteFlash = this.GetUIObject("Sprite_Flash") as XUISprite;
        if (this.m_uiToggle != null)
        {
            EventDelegate.Add(this.m_uiToggle.onChange, this.OnSelectStateChange);
        }
        this.Highlight(false);
    }
    public override IXUIObject GetUIObject(string strPath)
    {
        if (strPath == null)
        {
            return null;
        }
        string key = strPath;
        int num = strPath.LastIndexOf('/');
        if (num >= 0)
        {
            key = strPath.Substring(num + 1);
        }
        XUIObjectBase result = null;
        if (this.m_dicId2UIObject.TryGetValue(key, out result))
        {
            return result;
        }
        return null;
    }
    public override void Highlight(bool bTrue)
    {
        base.Highlight(bTrue);
        if (this.m_uiSpriteFlash != null)
        {
            if (!bTrue)
            {
                this.m_uiSpriteFlash.StopFlash();
            }
            else 
            {
                this.m_uiSpriteFlash.PlayFlash(true);
            }
        }
    }
    public override void _OnClick()
    {
        base._OnClick();
        this.ParentXUIList._OnClick(this);
    }
}

