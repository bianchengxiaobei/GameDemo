using UnityEngine;
using System.Collections;
using Game;
/// <summary>
/// 服务器发送给客户端登陆结果
/// </summary>
public class CptcG2CNtf_LoginResult : CProtocol
{
    public int m_dwErrorCode;
    public CptcG2CNtf_LoginResult()
        : base(1001)
    {
        this.m_dwErrorCode = 0;
    }
    public override CByteStream DeSerialize(CByteStream bs)
    {
        return bs;
    }
    public override CByteStream Serialize(CByteStream bs)
    {
        return bs;
    }
    public override void Process()
    {
        //表示登陆成功
        if (this.m_dwErrorCode == 0)
        {
           // Singleton<LoginCtrl>.singleton
        }
        else
        {

        }
    }
}
