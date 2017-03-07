using UnityEngine;
using System.Collections;
/// <summary>
/// 继MOnoBehavior的UI物体
/// </summary>
public abstract class XUIObjectBase : MonoBehaviour,IXUIObject 
{
    private bool m_bInit = false;
    public string m_strTipText = "";
    protected float m_fAlpha = 1f;
    private Transform m_cacheTransform;
    private GameObject m_cacheGameObject;
    private IXUIObject m_parent;
    public Transform CacheTransform
    {
        get 
        {
            if (this.m_cacheTransform == null) 
            {
                this.m_cacheTransform = base.transform;
            }
            return this.m_cacheTransform;
        }
    }
    public GameObject CacheGameObject
    {
        get 
        {
            if (this.m_cacheGameObject == null)
            {
                this.m_cacheGameObject = base.gameObject;
            }
            return this.m_cacheGameObject;
        }
    }
    public object TipParam
    {
        get;
        set;
    }
    public string Tip 
    {
        get { return this.m_strTipText; }
        set { this.m_strTipText = value; }
    }
    public virtual float Alpha 
    {
        get 
        {
            return this.m_fAlpha;
        }
        set 
        {
            this.m_fAlpha = value;
        }
    }
    public virtual IXUIObject Parent 
    {
        get 
        {
            return this.m_parent;
        }
        set 
        {
            this.m_parent = value;
        }
    }
    public bool IsInited 
    {
        get { return this.m_bInit; }
        set { this.m_bInit = value; }
    }
    public virtual void Init()
    {
        this.m_bInit = true;
    }
    public bool IsVisible()
    {
        return base.gameObject.activeInHierarchy;
    }
    public virtual void SetVisible(bool bVisible)
    {

    }
    public virtual void Highlight(bool bTrue)
    {
 
    }
    public virtual IXUIObject GetUIObject(string strPath) 
    {
        return null;
    }
    public virtual void OnAwake()
    {

    }
    public virtual void OnStart()
    {
 
    }
    public virtual void OnUpdate()
    {
 
    }
    public virtual void _OnClick()
    {
 
    }
    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        if (!this.m_bInit)
        {
            Init();
        }
        OnStart();
    }
    private void Update()
    {
        OnUpdate();
    }
    private void OnClick()
    {
        this._OnClick();
    }
    private void OnEnable()
    {
 
    }
    private void OnDisable()
    {
 
    }
}
