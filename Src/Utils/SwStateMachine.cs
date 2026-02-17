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
			if(SwGlobal.IsEqual(nextState, CurrentStateData.State)) return;
			CurrentStateData.ExitState(nextState);
			previousState = CurrentStateData.State;
		}
		CurrentStateData = nextStateData;
		CurrentStateData.EnterState(previousState);
		if(LogStates) GD.Print(nextState);
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
}
// using System;
// using System.Collections.Generic;
// using Godot;
// using SW.Src.Global;

// namespace SW.Src.Utils;
// public class SwStateMachine<T>
// {
// 	private readonly Dictionary<T, SwStateData<T>> States = [];
// 	private readonly Queue<T> StateQueue = new();
// 	public bool LogStates = false;
// 	private SwStateData<T> CurrentStateData;
// 	public SwStateMachine(){}
// 	public SwStateMachine(T initialState)
// 	{
// 		QueueStateUnchecked(initialState);
// 	}
// 	public void AddState(SwStateData<T> stateData)
// 	{
// 		// Add throws an exception when we try to add duplicate states fyi. Intended behavior.
// 		States.Add(stateData.State, stateData);
// 	}
// 	public bool RemoveState(T stateId)
// 	{
// 		return States.Remove(stateId);
// 	}
// 	private void SetState(T nextState)
// 	{
// 		if(!States.TryGetValue(nextState, out var nextStateData)) throw new Exception($"Invalid state '{nextState}'");
// 		T lastStateId = default;
// 		if(CurrentStateData is not null)
// 		{
// 			if(SwGlobal.IsEqual(nextState, CurrentStateData.State)) return;
// 			CurrentStateData.OnExitState(nextState);
// 			lastStateId = CurrentStateData.State;
// 		}
// 		CurrentStateData = nextStateData;
// 		CurrentStateData.OnEnterState(lastStateId);
// 		if(LogStates) GD.Print(CurrentStateData.State);
// 	}
// 	public T GetState(){return (CurrentStateData is not null) ? CurrentStateData.State : default;}
// 	public void QueueState(T nextStateId)
// 	{
// 		if(!States.ContainsKey(nextStateId)) throw new Exception($"Invalid state id '{nextStateId}'");
// 		StateQueue.Enqueue(nextStateId);
// 	}
// 	private void QueueStateUnchecked(T nextStateId)
// 	{
// 		StateQueue.Enqueue(nextStateId);
// 	}
// 	public void Tick(float dt)
// 	{
// 		while(StateQueue.TryDequeue(out T nextStateId))
// 		{
// 			SetState(nextStateId);
// 		}
// 		CurrentStateData?.OnTick(dt);
// 	}
// }

// public record class SwStateData<T>
// {
// 	public required T State{get; init;}
//     public Action<T> OnEnterState{get; init;} = (lastState)=>{};
//     public Action<T> OnExitState{get; init;} = (nextState)=>{};
//     public Action<float> OnTick{get; init;} = (dt)=>{};
// }
