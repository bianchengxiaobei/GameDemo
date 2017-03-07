using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using Game.Common;
using Utility;
public class DownloadMgr 
{
    private static DownloadMgr m_oInstance;
    private WebClient m_oWebClient;
    private List<DownloadTask> m_listTasks = new List<DownloadTask>();  
    public static DownloadMgr Instance 
    {
        get 
        {
            if (m_oInstance == null)
            {
                m_oInstance = new DownloadMgr();
            }
            return m_oInstance;
        }
    }
    /// <summary>
    /// 下载任务列表
    /// </summary>
    public List<DownloadTask> Tasks 
    {
        get { return this.m_listTasks; }
        set { this.m_listTasks = value; }
    }
    /// <summary>
    /// 所以任务下载完成之后回调
    /// </summary>
    public Action AllDownloadFinished { get; set; }
    /// <summary>
    /// 进度条委托回调
    /// </summary>
    public Action<int, int, string> TaskProgress { get; set; }
    /// <summary>
    /// 文件解压回调
    /// </summary>
    public Action<bool> FileDecompress { get; set; }
    public DownloadMgr()
    {
        this.m_oWebClient = new WebClient();
        this.m_oWebClient.Encoding = Encoding.UTF8;
    }
    /// <summary>
    /// 异步下载网页文本
    /// </summary>
    /// <param name="url"></param>
    /// <param name="AsynResult"></param>
    /// <param name="onError"></param>
    public void AsynDownLoadHtml(string url, Action<string> AsynResult, Action onError)
    {
        Action action = () =>
        {
            string text = DownLoadHtml(url);
            if (string.IsNullOrEmpty(text))
            {
                if (onError != null)
                {
                    LOLGameDriver.Invoke(onError);
                }
            }
            else 
            {
                if (AsynResult != null)
                {
                    LOLGameDriver.Invoke(() => { AsynResult(text); });
                    //AsynResult(text);
                }
            }
        };
        //开始异步下载
        action.BeginInvoke(null, null);
    }
    /// <summary>
    /// 下载网页的文本
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public string DownLoadHtml(string url)
    {
        try
        {
            return this.m_oWebClient.DownloadString(url);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return string.Empty;
        }
    }
    /// <summary>
    /// 检测下载列表
    /// </summary>
    public void CheckDownloadList()
    {
        if (Tasks.Count == 0)
        {
            //下载列表为空
            return;
        }
        //已经下载完成的数目
        int finishedCount = 0;
        foreach (var task in Tasks)
        {
            if (task.bFineshed && !task.bDownloadAgain)
            {
                finishedCount++;
            }
            else 
            {
                //是否文件存放的目录已经存在，如果不存在就创建
                string dir = Path.GetDirectoryName(task.FileName);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(task.FileName))
                {
                    Directory.CreateDirectory(dir);
                }
                //开始断点下载
                ThreadDownloadBreakPoint bpDownload = new ThreadDownloadBreakPoint(this, task);
                Thread t = new Thread(bpDownload.Download);
                t.Start();
            }
            break;
        }
        if (finishedCount > m_listTasks.Count - 1)
        {
            m_listTasks.Clear();
            m_listTasks = null;
            if (AllDownloadFinished != null)
            {
                AllDownloadFinished();
                AllDownloadFinished = null;
            }
        }
    }
    /// <summary>
    /// 断点下载文件
    /// </summary>
    /// <param name="url">网站资源url</param>
    /// <param name="filePath">保存文件路径</param>
    public void DownloadFileBreakPoint(string url, string filePath)
    {
        try
        {
            var requestUrl = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            var response = (HttpWebResponse)request.GetResponse();
            long contentLength = response.ContentLength;
            response.Close();
            request.Abort();
            long leftSize = contentLength;
            long position = 0;
            if (File.Exists(filePath))
            {
                Debug.Log("需要下载的文件已经存在");
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate,FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    leftSize = contentLength - fs.Length;
                    position = fs.Length;
                }
            }
            var partRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            if (leftSize > 0)
            {
                partRequest.AddRange((int)position, (int)(position + leftSize));
                var partResponse = (HttpWebResponse)partRequest.GetResponse();
                ReadBytesFromResponseToFile(url, partResponse, position, leftSize, contentLength, filePath);
                partResponse.Close();
            }
            partRequest.Abort();
            //下载完成
            Finished(url);
        }
        catch (Exception e)
        {
            Finished(url, e);
        }
    }
    /// <summary>
    /// 从网上下载资源字节到存到文件中
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <param name="response"></param>
    /// <param name="allFilePointer"></param>
    /// <param name="length"></param>
    /// <param name="totalSize"></param>
    /// <param name="filePath"></param>
    private void ReadBytesFromResponseToFile(string requestUrl,WebResponse response,long allFilePointer,long length,long totalSize,string filePath)
    {
        try
        {
            int bufferLength = (int)length;
            byte[] buffer = new byte[bufferLength];
            //本块位置指针
            int currentChunkPointer = 0;
            //指针偏移量
            int offset = 0;
            using (Stream resStream = response.GetResponseStream())
            {
                //下载的字节数
                int receivedBytesCount;
                do
                {
                    receivedBytesCount = resStream.Read(buffer, offset, bufferLength - offset);
                    offset += receivedBytesCount;
                    if (receivedBytesCount > 0)
                    {
                        byte[] bufferCopyed = new byte[receivedBytesCount];
                        Buffer.BlockCopy(buffer, currentChunkPointer, bufferCopyed, 0, bufferCopyed.Length);
                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            fs.Position = allFilePointer;
                            fs.Write(bufferCopyed,0,bufferCopyed.Length);
                            fs.Close();
                        }
                        float progress = (allFilePointer + bufferCopyed.Length) / totalSize;
                        //执行进度委托
                        Action action = () => 
                        {
                            if (m_listTasks.Any(task => task.Url == requestUrl))
                            {
                                DownloadTask task = this.m_listTasks.FirstOrDefault(t => t.Url == requestUrl);
                                task.TotalProgress((int)(progress*100), 0, 0);
                                if (TaskProgress != null)
                                {
                                    int finishedCount = this.m_listTasks.Count(t => t.bFineshed);
                                    string filename = task.FileName.Substring(task.FileName.LastIndexOf("/") + 1);
                                    TaskProgress(m_listTasks.Count, finishedCount, filename);
                                }
                            }
                        };
                        action.Invoke();
                        currentChunkPointer += receivedBytesCount;
                        allFilePointer += receivedBytesCount;
                    }
                } while (receivedBytesCount != 0);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// 下载完成之后验证MD5码
    /// </summary>
    /// <param name="task"></param>
    private void DownloadFinishedWithMd5(DownloadTask task)
    {
        string md5 = UnityTools.BuildFileMd5(task.FileName);
        if ("123".Trim() != task.MD5.Trim())
        {
            //MD5验证失败
            if (File.Exists(task.FileName))
            {
                File.Delete(task.FileName);
            }
            task.bDownloadAgain = true;
            task.bFineshed = false;
            CheckDownloadList();
            return;
        }
        if (FileDecompress != null)
        {
            FileDecompress(true);
        }
        task.bDownloadAgain = false;
        task.bFineshed = true;
        task.OnFinished();
        if (FileDecompress != null)
        {
            FileDecompress(false);
        }
        CheckDownloadList();
    }
    private void Finished(string url, Exception e = null)
    {
        Debug.Log("下载完成！");
        DownloadTask task = this.m_listTasks.FirstOrDefault(t => t.Url == url);
        if (task != null)
        {
            if (e != null)
            {
                Debug.LogWarning("下载出错！" + e.Message);
            }
            else 
            {
                //验证MD5码
                DownloadFinishedWithMd5(task);
            }
        }
    }
}
