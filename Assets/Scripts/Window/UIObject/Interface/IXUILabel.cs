using UnityEngine;
using System.Collections;

public interface IXUILabel : IXUIObject
{
    Color Color
    {
        get;
        set;
    }
    int LineWidth
    {
        get;
        set;
    }
    string GetText();
    void SetText(string strText);
}
