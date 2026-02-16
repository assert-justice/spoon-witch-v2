using System;

namespace SW.Src.StateMachine;
public class SwState<T>(T stateId, 
    Action<T> onEnterState = null, 
    Action<T> onExitState = null, 
    Action<float> onTick = null)
{
    public readonly T StateId = stateId;
    public readonly Action<T> OnEnterState = onEnterState ?? ((lastState)=>{});
    public readonly Action<T> OnExitState = onExitState ?? ((nextState)=>{});
    public readonly Action<float> OnTick = onTick ?? ((dt)=>{});
}
