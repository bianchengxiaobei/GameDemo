using UnityEngine;
using System.Collections;
/// <summary>
/// debug接口
/// </summary>
public interface IXLog
{
    void Debug(object message);
    void Info(object message);
    void Error(object message);
    void Fatal(object message);
}
