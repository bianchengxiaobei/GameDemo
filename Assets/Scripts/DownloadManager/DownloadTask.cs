using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// 下载任务类
/// </summary>
public class DownloadTask
{
    public string Url { get; set; }
    public string FileName { get; set; }
    public Action<int, long, long> TotalProgress { get; set; }
    public Action<int> Progress { get; set; }
    public Action<long> TotalBytesToReceive { get; set; }
    public Action<long> BytesReceived { get; set; }
    public String MD5 { get; set; }
    public Action Finished { get; set; }
    public Action<Exception> Error { get; set; }
    public bool bFineshed = false;//文件是否下载完成
    public bool bDownloadAgain = false;//是否需要从新下载，如果下载出错的时候会从新下
    public void OnTotalBytesToReceive(long size)
    {
        if (TotalBytesToReceive != null)
            TotalBytesToReceive(size);
    }
    public void OnBytesReceived(long size)
    {
        if (BytesReceived != null)
            BytesReceived(size);
    }
    public void OnTotalProgress(int p, long totalSize, long receivedSize)
    {
        if (TotalProgress != null)
            TotalProgress(p, totalSize, receivedSize);
    }
    public void OnProgress(int p)
    {
        if (Progress != null)
            Progress(p);
    }
    public void OnFinished()
    {
        if (Finished != null)
            LOLGameDriver.Invoke(Finished);
            //Finished();
    }
    public void OnError(Exception ex)
    {
        if (Error != null)
            Error(ex);
    }
}
