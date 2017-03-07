using UnityEngine;
using System.Collections;
using Game.Common;
/// <summary>
/// 提示参数类
/// </summary>
public class TipParam 
{
    //提示信息
    public string Tip
    {
        get;
        set;
    }
    //提示类型，有普通提示（技能装备等）和标题提示（没有带复杂内容，只有一行字）
    public TipEnumType TipType
    {
        get;
        set;
    }
    public TipParam()
    {
        TipType = TipEnumType.eTipType_Common;
    }
}
