using UnityEngine;
using System.Collections.Generic;
using Game;
using Game.Common;
/// <summary>
/// 游戏状态管理器
/// </summary>
public class GameStateManager : Singleton<GameStateManager>
{
    public Dictionary<GameStateType, IGameState> m_dicGameStates;
    IGameState m_oCurrentState;

    public IGameState CurrentState 
    {
        get { return this.m_oCurrentState; }
    }
    public GameStateManager()
    {
        m_dicGameStates = new Dictionary<GameStateType, IGameState>();
        m_dicGameStates.Add(GameStateType.GS_Login, new LoginState());
    }
    /// <summary>
    /// 改变游戏状态
    /// </summary>
    /// <param name="stateType"></param>
    public void ChangeGameStateTo(GameStateType stateType)
    {
        if (m_oCurrentState != null && m_oCurrentState.GetStateType() != GameStateType.GS_Loading && m_oCurrentState.GetStateType() == stateType)
            return;

        if (m_dicGameStates.ContainsKey(stateType))
        {
            if (m_oCurrentState != null)
            {
                m_oCurrentState.Exit();//先退出上个状态
            }

            m_oCurrentState = m_dicGameStates[stateType];
            m_oCurrentState.Enter();//进入这个状态
        }
    }
    /// <summary>
    /// 进入默认状态，默认为登陆状态
    /// </summary>
    public void EnterDefaultState()
    {
        ChangeGameStateTo(GameStateType.GS_Login);
    }
    public void FixedUpdate(float fixedDeltaTime)
    {
        if (m_oCurrentState != null)
        {
            m_oCurrentState.FixedUpdate(fixedDeltaTime);
        }
    }
    public void Update(float fDeltaTime)
    {
        GameStateType nextStateType = GameStateType.GS_Continue;
        if (m_oCurrentState != null)
        {
            nextStateType = m_oCurrentState.Update(fDeltaTime);
        }

        if (nextStateType > GameStateType.GS_Continue)
        {
            ChangeGameStateTo(nextStateType);
        }
    }
    public IGameState getState(GameStateType type)
    {
        if (!m_dicGameStates.ContainsKey(type))
        {
            return null;
        }
        return m_dicGameStates[type];
    }
}
