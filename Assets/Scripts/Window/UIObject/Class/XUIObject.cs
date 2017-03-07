using UnityEngine;
using System.Collections;

public abstract class XUIObject : XUIObjectBase
{
    public override void OnAwake()
    {
        base.OnAwake();
    }
    public override void OnStart()
    {
        base.OnStart();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void Init()
    {
        base.Init();
    }
    public override void SetVisible(bool bVisible)
    {
        NGUITools.SetActiveSelf(base.gameObject, bVisible);
    }
}
