using UnityEngine;
using System.Collections;
[AddComponentMenu("XUI/XUILabel")]
public class XUILabel : XUIObject,IXUIObject,IXUILabel
{
    private UILabel m_uiLabel;
    public override float Alpha
    {
        get
        {
            if (this.m_uiLabel != null)
            {
                return this.m_uiLabel.alpha;
            }
            return base.Alpha;
        }
        set
        {
            if (this.m_uiLabel != null)
            {
                this.m_uiLabel.alpha = value;
            }
        }
    }
    public Color Color 
    {
        get 
        {
            if (this.m_uiLabel != null)
            {
                return this.m_uiLabel.color;
            }
            return Color.white;
        }
        set 
        {
            if (this.m_uiLabel != null)
            {
                this.m_uiLabel.color = value;
            }
        }
    }
    public int LineWidth 
    {
        get 
        {
            if (this.m_uiLabel != null)
            {
                return this.m_uiLabel.width;
            }
            return 0;
        }
        set 
        {
            if (this.m_uiLabel != null)
            {
                this.m_uiLabel.width = value;
            }
        }
    }
    public string GetText()
    {
        return this.m_uiLabel.text;
    }
    public void SetText(string strText)
    {
        this.m_uiLabel.text = strText;
    }
    public override void Init()
    {
        base.Init();
        this.m_uiLabel = base.GetComponent<UILabel>();
        if (null == this.m_uiLabel)
        {
            Debug.LogError("null == m_uiLabel");
        }
    }
}
