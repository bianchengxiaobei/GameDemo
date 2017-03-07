using UnityEngine;
using System.Collections;
using Game.Common;
/// <summary>
/// 游戏状态接口
/// </summary>
public interface IGameState
{
    GameStateType GetStateType();//获取实现类的状态类型
    void SetStateTo(GameStateType gsType);//设置实现类的状态类型（改变状态）
    void Enter();//进入实现类的状态，做主要状态的逻辑处理
    GameStateType Update(float fDeltaTime);//状态改变，返回改变后的状态
    void FixedUpdate(float fixedDeltaTime);//固定更新状态
    void Exit();//退出该状态
}
