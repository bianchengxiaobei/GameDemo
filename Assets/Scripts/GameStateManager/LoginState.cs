using UnityEngine;
using System.Collections;
using Game.Common;
using Game;
/// <summary>
/// 登陆游戏状态
/// </summary>
public class LoginState : IGameState
{
    public const string LoginAudioBG = "Audio/EnvironAudio/mus_fb_login_lp.mp3";
    GameStateType m_eStateTo;
    public LoginState()
    {
 
    }
    public GameStateType GetStateType() 
    {
        return GameStateType.GS_Login;
    }
    public void SetStateTo(GameStateType stateType)
    {
        this.m_eStateTo = stateType;
    }
    public void Enter()
    {
        SetStateTo(GameStateType.GS_Continue);
        LoginCtrl.singleton.Enter();
        //播放登陆音乐
        ResourceUnit audioUnit = ResourceManager.Instance.LoadImmediate(LoginAudioBG,ResourceType.ASSET);
        AudioClip clip = audioUnit.Asset as AudioClip;
        AudioManager.singleton.PlayBgAudio(clip);
    }
    public void Exit()
    {
        LoginCtrl.singleton.Exit();
    }
    public void FixedUpdate(float fixedDeltaTime)
    {
 
    }
    public GameStateType Update(float fDeltaTime)
    {
        return m_eStateTo;
    }
}
