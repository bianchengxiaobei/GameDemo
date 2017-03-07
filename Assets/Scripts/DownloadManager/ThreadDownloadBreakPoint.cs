using UnityEngine;
using System.Collections;
/// <summary>
/// 断点下载类
/// </summary>
public class ThreadDownloadBreakPoint
{
    public DownloadMgr Mgr { get; set; }
    public DownloadTask Task { get; set; }
    public ThreadDownloadBreakPoint()
    {
 
    }
    public ThreadDownloadBreakPoint(DownloadMgr mgr, DownloadTask task)
    {
        Mgr = mgr;
        Task = task;
    }
    public void Download()
    {
        Mgr.DownloadFileBreakPoint(Task.Url, Task.FileName);
    }
}
