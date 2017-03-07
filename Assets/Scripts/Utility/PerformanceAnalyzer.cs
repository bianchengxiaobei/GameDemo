using UnityEngine;
using System.Collections;
using Game;
namespace Utility
{
    public class PerformanceAnalyzer : Singleton<PerformanceAnalyzer>
    {
        public float m_fUpdateInterval = 1f;
        private float m_fLastInterval;
        private int m_nFrames = 0;
        private float m_fFps;
        private uint m_unReceivedSize = 0u;
        private uint m_unSentSize = 0u;
        private uint m_unRecevieRate = 0u;
        private uint m_unSendRate = 0u;
        public bool Enable
        {
            get;
            set;
        }
        public uint FPS
        {
            get
            {
                return (uint)this.m_fFps;
            }
        }
        public uint ReceiveRate
        {
            get
            {
                return this.m_unRecevieRate;
            }
        }
        public uint SendRate
        {
            get
            {
                return this.m_unSendRate;
            }
        }
        public void Init()
        {
            this.m_fLastInterval = Time.realtimeSinceStartup;
            this.m_nFrames = 0;
        }
        public void Update()
        {
            this.m_nFrames++;
            if (Time.realtimeSinceStartup > this.m_fLastInterval + this.m_fUpdateInterval)
            {
                float num = Time.realtimeSinceStartup - this.m_fLastInterval;
                this.m_fFps = (float)this.m_nFrames / num;
                this.m_nFrames = 0;
                this.m_unRecevieRate = (uint)(this.m_unReceivedSize / num);
                this.m_unSendRate = (uint)(this.m_unSentSize / num);
                this.m_unReceivedSize = 0u;
                this.m_unSentSize = 0u;
                this.m_fLastInterval = Time.realtimeSinceStartup;
            }
        }
        public void OnRecevie(uint unSize)
        {
            this.m_unReceivedSize += unSize;
        }
        public void OnSend(uint unSize)
        {
            this.m_unSentSize += unSize;
        }
    }
}
