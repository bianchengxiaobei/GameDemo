using UnityEngine;
using System.Collections;
namespace Game
{
    internal class CRegister
    {
        /// <summary>
        /// 注册消息协议
        /// </summary>
        public static void RegistProtocol() 
        {
            CProtocol.Register(new CptcG2CNtf_LoginResult());//1001，注册登录消息
        }
    }
}