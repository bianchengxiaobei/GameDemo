using UnityEngine;
using System.Collections;
[AddComponentMenu("XUI/XUITexture")]
public class XUITexture : XUIObject,IXUIObject,IXUITexture
{
    private UITexture m_uiTexture;
    private string m_strTextureFile = string.Empty;
    private ResourceUnit m_resourceUnit;
    public Color Color 
    {
        get 
        {
            if (this.m_uiTexture != null)
            {
                return this.m_uiTexture.color;
            }
            return Color.white;
        }
        set 
        {
            if (this.m_uiTexture != null)
            {
                this.m_uiTexture.color = value;
            }
        }
    }
    public void SetTexture(Texture texture)
    {
        if (this.m_uiTexture != null)
        {
            this.m_uiTexture.mainTexture = texture;
        }
    }
    public void SetTexture(string strTextureFile)
    {
        if (string.IsNullOrEmpty(strTextureFile))
        {
            return;
        }
        if (this.m_uiTexture != null && !this.m_strTextureFile.Equals(strTextureFile))
        {
            this.m_strTextureFile = strTextureFile;
            this.m_resourceUnit = ResourceManager.Instance.LoadImmediate(this.m_strTextureFile, Game.Common.ResourceType.ASSET);
            if (null == this.m_resourceUnit)
            {
                return;
            }
            Texture texture = this.m_resourceUnit.Asset as Texture;
            if (null == texture)
            {
                return;
            }
            this.m_uiTexture.mainTexture = texture;
            this.m_uiTexture.enabled = true;
        }
    }
    private void OnDestroy()
    {
        if (this.m_resourceUnit != null)
        {
            this.m_resourceUnit.Dispose();
            this.m_resourceUnit = null;
        }
    }
}
