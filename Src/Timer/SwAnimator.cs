// using System;
// using System.Collections.Generic;

// namespace SW.Src.Timer;
// public partial class SwAnimator : SwTimer
// {
//     private readonly Dictionary<string, SwAnimChain> Chains = [];
//     private readonly Queue<string> FreeQueue = new();
//     public SwAnimChain AddChain(string name = null)
//     {
//         name ??= Guid.NewGuid().ToString();
//         SwAnimChain chain = new();
//         Chains.Add(name, chain);
//         return chain;
//     }
//     public bool TryGetChain(string name, out SwAnimChain chain)
//     {
//         return Chains.TryGetValue(name, out chain);
//     }
//     protected override void TickInternal(float dt)
//     {
//         foreach (var (name, chain) in Chains)
//         {
//             chain.Tick(dt);
//             if(chain.IsFinished()) FreeQueue.Enqueue(name);
//         }
//         while(FreeQueue.TryDequeue(out var name)) Chains.Remove(name);
//         base.TickInternal(dt);
//     }
//     public override bool IsFinished()
//     {
//         return Chains.Count > 0;
//     }
// }
