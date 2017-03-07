using UnityEngine;
using System.Collections;
using Utility;
namespace Game
{
    public class CNetObserver : INetObserver
    {
        public CNetProcessor oProc = null;
        public void OnConnect(bool bSuccess)
        {
            if (!bSuccess)
            {
            }
            else
            {
                XLog.Log.Debug("Connect Success!");
            }
        }
        public void OnClosed(NetErrCode nErrCode)
        {
            XLog.Log.Error(string.Format("OnClosed:{0}", nErrCode.ToString()));
        }
        public void OnReceive(int unType, int nLen)
        {
            Singleton<PerformanceAnalyzer>.singleton.OnRecevie((uint)nLen);
        }
        public void OnSend(int dwType, int nLen)
        {
            Singleton<PerformanceAnalyzer>.singleton.OnSend((uint)nLen);
        }
    }
}