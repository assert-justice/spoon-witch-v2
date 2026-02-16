using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Utils;

namespace SW.Src.Input;

public partial class SwVAxis : SwInput, ISwInput<float>
{
	private readonly List<Func<float>> Fns = [];
	private float Value = 0;
	private Func<float, float> Filter;
	SwMaxAvg Avg = new();
	public SwVAxis(float deadzone = 0.25f)
	{
		SetDeadzone(deadzone);
	}
	public void AddFn(Func<float> fn)
	{
		Fns.Add(fn);
	}
	public void AddPhysicalKeys(Key neg, Key pos)
	{
		Fns.Add(()=>(GetPhysicalKey(pos)?1:0) - (GetPhysicalKey(neg)?1:0));
	}
	public void AddJoyButtons(JoyButton neg, JoyButton pos, int device = -1)
	{
		Fns.Add(()=>(GetJoyButton(pos,device)?1:0) - (GetJoyButton(neg,device)?1:0));
	}
	public void AddJoyAxis(JoyAxis axis, int device = -1)
	{
		Fns.Add(()=>GetJoyAxis(axis, device));
	}
	public void Clear()
	{
		Fns.Clear();
	}
	public void Poll()
	{
		Avg.Reset();
		foreach (var fn in Fns)
		{
			Avg.AddValue(fn());
		}
		Value = Filter(Avg.GetValue());
	}
	public float GetValue(){return Value;}
	public void SetFilter(Func<float,float> filter){Filter = filter;}
	public void SetDeadzone(float deadzone)
	{
		float fn(float value)
		{
			float abs = Mathf.Abs(value);
			if(abs < deadzone) return 0;
			float sign = Mathf.Sign(value);
			if(abs > 1) return sign;
			abs -= deadzone;
			abs /= 1 - deadzone;
			return abs * sign;
		}
		Filter = fn;
	}

	public override bool TryAddBind(SwInputBind inputBind)
	{
		throw new NotImplementedException();
	}
}
