using System;
using System.Collections.Generic;

namespace SW.Src.StateMachine;
public class SwStateMachine<T>
{
	private readonly Dictionary<T, SwState<T>> States = [];
	private readonly Queue<T> StateQueue = new();
	private SwState<T> CurrentState;
	public SwStateMachine(){}
	public SwStateMachine(T initialStateId)
	{
		QueueState(initialStateId);
	}
	public void AddState(SwState<T> state)
	{
		// Add throws an exception when we try to add duplicate states fyi. Intended behavior.
		States.Add(state.StateId, state);
	}
	public bool RemoveState(T stateId)
	{
		return States.Remove(stateId);
	}
	private void SetState(T nextStateId)
	{
		if(!States.TryGetValue(nextStateId, out var nextState)) throw new Exception($"Invalid state '{nextStateId}'");
		T lastStateId = default;
		if(CurrentState is not null)
		{
			if(EqualityComparer<T>.Default.Equals(nextStateId, nextState.StateId)) return;
			CurrentState.OnExitState(nextStateId);
			lastStateId = CurrentState.StateId;
		}
		CurrentState = nextState;
		CurrentState.OnEnterState(lastStateId);
	}
	public T GetState(){return (CurrentState is not null) ? CurrentState.StateId : default;}
	public void QueueState(T nextStateId)
	{
		StateQueue.Enqueue(nextStateId);
	}
	public void Tick(float dt)
	{
		while(StateQueue.TryDequeue(out T nextStateId))
		{
			SetState(nextStateId);
		}
		CurrentState?.OnTick(dt);
	}
}
