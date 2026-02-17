// using System;
// using Godot;
// using SW.Src.Global;
// using SW.Src.Input;
// using SW.Src.Timer;

// namespace SW.Src.Actor.Player;

// public class SwPlayerControls
// {
//     private readonly SwInputManager InputManager;
//     private readonly SwInputBuffer InputBuffer;
//     private readonly SwClock InputLockout;
//     private readonly Action<SwPlayer.SwState> QueueState;
//     private readonly SwClock IdleTime;
//     private Action<SwPlayerAbstract.SwState> queueState;

//     private enum SwAction
// 	{
// 		SpoonAttack,
// 		ChargeSling,
// 		Dodge,
// 		UseItem,
// 		Heal,
// 	}
//     public bool SpoonAttack{get=>InputBuffer.TryGetValue(4);}
//     public bool ChargeSling{get=>InputBuffer.TryGetValue(3);}
//     public bool Dodge{get=>InputBuffer.TryGetValue(2);}
//     public bool UseItem{get=>InputBuffer.TryGetValue(0);}
//     public bool Heal{get=>InputBuffer.TryGetValue(1);}
//     public SwPlayerControls(Action<SwPlayer.SwState> queueState, SwInputManager inputManager, SwClock inputLockout, SwClock idleTime)
//     {
//         QueueState = queueState;
//         InputLockout = inputLockout;
//         InputManager = inputManager;
//         IdleTime = idleTime;
//         // Names listed in reverse order of priority
//         InputBuffer = new(["UseItem","Heal","Dodge","ChargeSling","SpoonAttack"]);
//     }

//     public SwPlayerControls(Action<SwPlayerAbstract.SwState> queueState, SwInputManager inputManager, SwClock inputLockout, SwClock idleTime)
//     {
//         this.queueState = queueState;
//         InputManager = inputManager;
//         InputLockout = inputLockout;
//         IdleTime = idleTime;
//     }

//     public void Poll()
//     {
//         InputBuffer.Poll();
//         if(InputLockout.IsRunning()) return;
//         bool idle = false;
//         // if(false){}
//         if(SpoonAttack) QueueState(SwPlayer.SwState.Attacking);
//         else if(ChargeSling) QueueState(SwPlayer.SwState.Charging);
//         else if(Dodge) QueueState(SwPlayer.SwState.Dodging);
//         else if(Heal || UseItem) QueueState(SwPlayer.SwState.UsingItem);
//         else if(InputManager.Move.GetValue().LengthSquared() > SwConstants.EPSILON) QueueState(SwPlayer.SwState.Running);
//         else idle = true;
//         if(!idle) IdleTime.Restart();
//         // if(idle) IdleTime += dt;
//         // else IdleTime = 0;
//         // GD.Print(InputManager.Move.GetValue());
//     }
// }
