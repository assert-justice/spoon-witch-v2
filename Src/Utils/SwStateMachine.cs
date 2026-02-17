using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;

namespace SW.Src.Utils;
public class SwStateMachine<T>
{
	private readonly Dictionary<T, SwStateData<T>> States = [];
	private readonly Queue<T> StateQueue = new();
	public bool LogStates = false;
	private SwStateData<T> CurrentStateData;
	public SwStateMachine(){}
	public SwStateMachine(T initialState)
	{
		QueueStateUnchecked(initialState);
	}
	public void AddState(SwStateData<T> stateData)
	{
		// Add throws an exception when we try to add duplicate states fyi. Intended behavior.
		States.Add(stateData.State, stateData);
	}
	public bool RemoveState(T stateId)
	{
		return States.Remove(stateId);
	}
	private void SetState(T nextState)
	{
		if(!States.TryGetValue(nextState, out var nextStateData)) throw new Exception($"Invalid state '{nextState}'");
		T lastStateId = default;
		if(CurrentStateData is not null)
		{
			if(SwGlobal.IsEqual(nextState, CurrentStateData.State)) return;
			CurrentStateData.OnExitState(nextState);
			lastStateId = CurrentStateData.State;
		}
		CurrentStateData = nextStateData;
		CurrentStateData.OnEnterState(lastStateId);
		if(LogStates) GD.Print(CurrentStateData.State);
	}
	public T GetState(){return (CurrentStateData is not null) ? CurrentStateData.State : default;}
	public void QueueState(T nextStateId)
	{
		if(!States.ContainsKey(nextStateId)) throw new Exception($"Invalid state id '{nextStateId}'");
		StateQueue.Enqueue(nextStateId);
	}
	private void QueueStateUnchecked(T nextStateId)
	{
		StateQueue.Enqueue(nextStateId);
	}
	public void Tick(float dt)
	{
		while(StateQueue.TryDequeue(out T nextStateId))
		{
			SetState(nextStateId);
		}
		CurrentStateData?.OnTick(dt);
	}
}

public record class SwStateData<T>
{
	public required T State{get; init;}
    public Action<T> OnEnterState{get; init;} = (lastState)=>{};
    public Action<T> OnExitState{get; init;} = (nextState)=>{};
    public Action<float> OnTick{get; init;} = (dt)=>{};
}
