using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Game.Common;
public class Logger : IXLog
{
    private string m_strTag = string.Empty;
    private static FileStream s_ofs = null;
    private static StreamWriter s_owr = null;
    private static UdpClient s_udpClient = null;
    private static int s_nLastSaveTime = 0;
    private static EnumLogLevel s_eLogLevel = EnumLogLevel.eLogLevel_Debug;
    public static bool IsInEditor
    {
        get;
        set;
    }
    public static EnumLogLevel LogLevel
    {
        get
        {
            return Logger.s_eLogLevel;
        }
        set
        {
            Logger.s_eLogLevel = value;
        }
    }
    public static bool Init(string strFilePath)
    {
        bool result;
        try
        {
            string directoryName = Path.GetDirectoryName(strFilePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            Logger.s_ofs = new FileStream(strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Logger.s_owr = new StreamWriter(Logger.s_ofs, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Logger.s_owr = null;
            UnityEngine.Debug.LogError(ex.ToString());
            if (null != Logger.s_udpClient)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ex.ToString());
                Logger.s_udpClient.Send(bytes, bytes.Length);
            }
            result = false;
            return result;
        }
        result = true;
        return result;
    }
    public static bool Init(string strRemoteIP, int nPort)
    {
        bool result;
        try
        {
            Logger.s_udpClient = new UdpClient();
            Logger.s_udpClient.Connect(strRemoteIP, nPort);
        }
        catch (Exception ex)
        {
            Logger.s_udpClient = null;
            UnityEngine.Debug.LogError(ex.ToString());
            result = false;
            return result;
        }
        result = true;
        return result;
    }
    public static void Close()
    {
        Logger.Save();
        if (null != Logger.s_owr)
        {
            Logger.s_owr.Close();
            Logger.s_owr = null;
        }
        if (null != Logger.s_ofs)
        {
            Logger.s_ofs.Close();
            Logger.s_ofs = null;
        }
    }
    private static void Save()
    {
        Logger.s_nLastSaveTime = Environment.TickCount;
        if (null != Logger.s_owr)
        {
            Logger.s_owr.Flush();
        }
        if (null != Logger.s_ofs)
        {
            Logger.s_ofs.Flush();
        }
    }
    private static void AddInfo(string strInfo)
    {
        try
        {
            if (null != Logger.s_udpClient)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", strInfo));
                Logger.s_udpClient.Send(bytes, bytes.Length);
            }
        }
        catch (Exception e)
        {
        }
        if (null != Logger.s_owr)
        {
            Logger.s_owr.WriteLine(string.Format("{0:yyyy-MM-dd HH:mm:ss,ffff} {1}", DateTime.Now, strInfo));
            if (Environment.TickCount - Logger.s_nLastSaveTime >= 5000)
            {
                Logger.Save();
            }
        }
    }
    public Logger(string strTag)
    {
        this.m_strTag = strTag;
    }
    public void Debug(object message)
    {
        if (EnumLogLevel.eLogLevel_Debug >= Logger.s_eLogLevel)
        {
            string text = string.Format("[Debug] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
            }
            else
            {
                UnityEngine.Debug.Log(text);
            }
        }
    }
    public void Info(object message)
    {
        if (EnumLogLevel.eLogLevel_Info >= Logger.s_eLogLevel)
        {
            string text = string.Format("[Info] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
            }
            else
            {
                UnityEngine.Debug.Log(text);
            }
        }
    }
    public void Error(object message)
    {
        if (EnumLogLevel.eLogLevel_Error >= Logger.s_eLogLevel)
        {
            string text = string.Format("[Error] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
            }
            else
            {
                UnityEngine.Debug.LogError(text);
            }
        }
    }
    public void Fatal(object message)
    {
        if (EnumLogLevel.eLogLevel_Fatal >= Logger.s_eLogLevel)
        {
            string text = string.Format("[Fatal] [{0}] {1}", this.m_strTag, message);
            if (!Logger.IsInEditor)
            {
                Logger.AddInfo(text);
            }
            else
            {
                UnityEngine.Debug.LogError(text);
            }
        }
    }
}