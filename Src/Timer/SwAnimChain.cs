// using System.Collections.Generic;

// namespace SW.Src.Timer;
// public class SwAnimChain : SwTimer
// {
//     private readonly Queue<SwClock> Chain = new();
//     private SwClock Current = null;
//     public SwClock AddLink(float duration)
//     {
//         SwClock link = new(duration);
//         Chain.Enqueue(link);
//         return link;
//     }
//     public bool TryGetCurrent(out SwClock link)
//     {
//         link = Current;
//         return link is not null;
//     }
//     protected override void TickInternal(float dt)
//     {
//         if(Current is SwClock link)
//         {
//             link.Tick(dt);
//             if(link.IsFinished()) Current = null;
//         }
//         else Chain.TryDequeue(out Current);
//         base.TickInternal(dt);
//     }
//     public override bool IsFinished()
//     {
//         return Current is null && Chain.Count == 0;
//     }
// }
