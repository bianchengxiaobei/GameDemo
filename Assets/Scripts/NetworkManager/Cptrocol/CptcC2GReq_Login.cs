using UnityEngine;
using System.Collections;
using Game;
/// <summary>
/// 客户端发送给服务器的登陆消息请求
/// </summary>
public class CptcC2GReq_Login : CProtocol
{
    public string m_strUsername;//用户名
    public string m_strPassword;//密码
    public int m_dwServerId;//选择的服务器id
    public CptcC2GReq_Login()
        : base(1000)
    {
        this.m_strUsername = "";
        this.m_strPassword = "";
        this.m_dwServerId = 0;
    }
    public override CByteStream DeSerialize(CByteStream bs)
    {
        bs.Read(ref m_strUsername);
        bs.Read(ref m_strPassword);
        bs.Read(ref m_dwServerId);
        return bs;
    }
    public override CByteStream Serialize(CByteStream bs)
    {
        bs.Write(this.m_strUsername);
        bs.Write(this.m_strPassword);
        bs.Write(this.m_dwServerId);
        return bs;
    }
    public override void Process()
    {
        
    }
}
