using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Global;

namespace SW.Src.Utils;

public class SwStateMachine<TParent, TState>
{
	public abstract class SwStateData(TParent parent, TState state)
	{
		protected readonly TParent Parent = parent;
		public readonly TState State = state;
		public virtual void EnterState(TState lastState){}
		public virtual void ExitState(TState lastState){}
		public virtual void Tick(float dt){}
	}
	private readonly Dictionary<TState, SwStateData> States = [];
	private readonly Queue<TState> StateQueue = new();
	private SwStateData CurrentStateData;
	public bool LogStates = false;
	public SwStateMachine(){}
	public SwStateMachine(TState initialState){QueueStateUnchecked(initialState);}
	private void SetState(TState nextState)
	{
		if(!States.TryGetValue(nextState, out var nextStateData)) throw new Exception($"Invalid state '{nextState}'");
		TState previousState = default;
		if(CurrentStateData is not null)
		{
			if(SwStatic.IsEqual(nextState, CurrentStateData.State)) return;
			CurrentStateData.ExitState(nextState);
			previousState = CurrentStateData.State;
		}
		CurrentStateData = nextStateData;
		CurrentStateData.EnterState(previousState);
		if(LogStates) SwStatic.Log(nextState);
	}
	public void QueueStateUnchecked(TState nextState)
	{
		StateQueue.Enqueue(nextState);
	}
	public void QueueState(TState nextState)
	{
		if(!States.ContainsKey(nextState)) throw new Exception($"Invalid state '{nextState}'");
		StateQueue.Enqueue(nextState);
	}
	public void AddState(SwStateData stateData)
	{
		// Add throws an exception when we try to add duplicate states fyi. Intended behavior.
		States.Add(stateData.State, stateData);
	}
	public bool RemoveState(TState state)
	{
		return States.Remove(state);
	}
	public void Tick(float dt)
	{
		while(StateQueue.TryDequeue(out var state)) SetState(state);
		CurrentStateData.Tick(dt);
	}
	public bool TryGetState(out TState state)
	{
		state = default;
		if(CurrentStateData is null) return false;
		state = CurrentStateData.State;
		return true;
	}
	public bool IsInState(TState state){return TryGetState(out var currentState) && SwStatic.IsEqual(state, currentState);}
}
