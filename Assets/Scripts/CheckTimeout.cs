using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 检测网络是否超时类
/// </summary>
public class CheckTimeout 
{
    /// <summary>
    /// 是否网络超时，这里使用百度做测试
    /// </summary>
    /// <param name="AsynResult"></param>
    public void AsynIsNetworkTimeout(Action<bool> AsynResult)
    {
        TryAsynDownloadHtml("http://www.baidu.com", AsynResult);
    }
    private void TryAsynDownloadHtml(string url, Action<bool> AsynResult)
    {
        DownloadMgr.Instance.AsynDownLoadHtml(url, (text) => 
        {
            if (string.IsNullOrEmpty(text))
            {
                AsynResult(false);
            }
            else 
            {
                AsynResult(true);
            }
        }, () => { AsynResult(false); });
    }
}
