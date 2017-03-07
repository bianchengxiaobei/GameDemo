using UnityEngine;
using System.Collections;

public class XUIAtlas : MonoBehaviour,IXUIAtlas
{
    private UIAtlas m_uiAtlas;
    public UIAtlas Atlas 
    {
        get
        {
            return this.m_uiAtlas;
        }
        set
        {
            this.m_uiAtlas = value;
        }
    }
    private void Awake()
    {
        this.m_uiAtlas = base.GetComponent<UIAtlas>();
        if (null == this.m_uiAtlas)
        {
            Debug.LogError("null == m_uiAtlas");
        }
    }
}
