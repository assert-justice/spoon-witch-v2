using System;
using System.Collections.Generic;
using Godot;

namespace SW.Src.Input;

public class SwInputManager
{
	private readonly Dictionary<string, SwVButton> Buttons = [];
	private readonly Dictionary<string, SwVAxis> Axes = [];
	private readonly Dictionary<string, SwVAxis2> Axes2 = [];
	public SwVButton AddButton(string name, SwVButton button = null)
	{
		button??=new();
		Buttons.Add(name, button); 
		return button;
	}
	public SwVAxis AddAxis(string name, SwVAxis axis = null)
	{
		axis??=new();
		Axes.Add(name, axis);
		return axis;
	}
	public SwVAxis2 AddAxis2(string name, SwVAxis2 axis2 = null)
	{
		axis2??=new();
		Axes2.Add(name, axis2);
		return axis2;
	}
	private const float PulseCooldown = 0.25f;
	private const float PulseDelay = 0.2f;
	public readonly SwVAxis2 Move;
	public readonly SwVAxis2 Aim;
	public readonly SwVAxis2 UiMove;
	public readonly SwVButton UiConfirm;
	public readonly SwVButton UiCancel;
	public readonly SwVButton UiUp;
	public readonly SwVButton UiDown;
	public readonly SwVButton UiLeft;
	public readonly SwVButton UiRight;
	public readonly SwVButton UiPrevious;
	public readonly SwVButton UiNext;
	public readonly SwVButton ChargeSling;
	public readonly SwVButton SpoonAttack;
	public readonly SwVButton Dodge;
	public readonly SwVButton Heal;
	public readonly SwVButton Interact;
	public readonly SwVButton UseItem;
	public readonly SwVButton PreviousItem;
	public readonly SwVButton NextItem;
	public readonly SwVButton PlaceCauldron;
	public readonly SwVButton PlayInstrument;
	private readonly SwVAxis RightTrigger;
	private readonly SwVAxis LeftTrigger;
	public readonly SwVButton Map;
	public readonly SwVButton Inventory;
	public readonly SwVButton Journal;
	public readonly SwVButton Submenu;
	public readonly SwVButton Pause;
	public SwInputManager()
	{
		Move = AddAxis2("Move");
		Aim = AddAxis2("Aim");
		UiMove = AddAxis2("UiMove");
		UiConfirm = AddButton("UiConfirm");
		UiCancel = AddButton("UiCancel");
		UiUp = AddButton("UiUp", new(PulseCooldown, PulseDelay));
		UiDown = AddButton("UiDown", new(PulseCooldown, PulseDelay));
		UiLeft = AddButton("UiLeft", new(PulseCooldown, PulseDelay));
		UiRight = AddButton("UiRight", new(PulseCooldown, PulseDelay));
		UiNext = AddButton("UiNext", new(PulseCooldown, PulseDelay));
		UiPrevious = AddButton("UiPrevious", new(PulseCooldown, PulseDelay));
		ChargeSling = AddButton("ChargeSling");
		SpoonAttack = AddButton("SpoonAttack");
		Dodge = AddButton("Dodge");
		Heal = AddButton("Heal");
		Interact = AddButton("Interact");
		UseItem = AddButton("UseItem");
		PreviousItem = AddButton("PreviousItem");
		NextItem = AddButton("NextItem");
		PlaceCauldron = AddButton("PlaceCauldron");
		PlayInstrument = AddButton("PlayInstrument");
		RightTrigger = AddAxis("RightTrigger");
		LeftTrigger = AddAxis("LeftTrigger");
		Map = AddButton("Map");
		Journal = AddButton("Journal");
		Inventory = AddButton("Inventory");
		Submenu = AddButton("Submenu");
		Pause = AddButton("Pause");
	}
	public void Tick(float dt)
	{
		foreach (var button in Buttons.Values)
		{
			button.Poll();
			button.Tick(dt);
		}
		foreach (var axis in Axes.Values)
		{
			axis.Poll();
		}
		foreach (var axis2 in Axes2.Values)
		{
			axis2.Poll();
		}
	}
	public void ClearInputs()
	{
		foreach (var item in Buttons.Values)
		{
			item.Clear();
		}
		foreach (var item in Axes.Values)
		{
			item.Clear();
		}
		foreach (var item in Axes2.Values)
		{
			item.Clear();
		}
	}
	public bool TryGetButton(string name, out SwVButton button)
	{
		return Buttons.TryGetValue(name, out button);
	}
	public bool TryGetAxis(string name, out SwVAxis axis)
	{
		return Axes.TryGetValue(name, out axis);
	}
	public bool TryGetAxis2(string name, out SwVAxis2 axis2)
	{
		return Axes2.TryGetValue(name, out axis2);
	}
	public void BindDefaults()
	{
		ClearInputs();
		Move.AddPhysicalKeys(Key.A, Key.D, Key.W, Key.S);
		Move.AddJoyAxes(JoyAxis.LeftX, JoyAxis.LeftY);
		Aim.AddJoyAxes(JoyAxis.RightX, JoyAxis.RightY);
		UiConfirm.AddPhysicalKey(Key.Space);
		UiConfirm.AddPhysicalKey(Key.Enter);
		UiConfirm.AddJoyButton(JoyButton.A);
		UiCancel.AddPhysicalKey(Key.Escape);
		UiCancel.AddJoyButton(JoyButton.B);
		UiMove.AddPhysicalKeys(Key.A, Key.D, Key.W, Key.S);
		UiMove.AddPhysicalKeys(Key.Left, Key.Right, Key.Up, Key.Down);
		UiMove.AddJoyAxes(JoyAxis.LeftX, JoyAxis.LeftY);
		UiMove.AddJoyAxes(JoyAxis.RightX, JoyAxis.RightY);
		UiLeft.AddAxisNeg(()=>UiMove.GetValue().X);
		UiRight.AddAxisPos(()=>UiMove.GetValue().X);
		UiUp.AddAxisNeg(()=>UiMove.GetValue().Y);
		UiDown.AddAxisPos(()=>UiMove.GetValue().Y);
		UiPrevious.AddPhysicalKey(Key.Key1);
		UiPrevious.AddJoyButton(JoyButton.LeftShoulder);
		UiNext.AddPhysicalKey(Key.Key2);
		UiNext.AddJoyButton(JoyButton.RightShoulder);
		RightTrigger.AddJoyAxis(JoyAxis.TriggerRight);
		LeftTrigger.AddJoyAxis(JoyAxis.TriggerLeft);
		ChargeSling.AddMouseButton(MouseButton.Right);
		ChargeSling.AddAxisPos(RightTrigger.GetValue);
		SpoonAttack.AddMouseButton(MouseButton.Left);
		Dodge.AddPhysicalKey(Key.Space);
		Dodge.AddJoyButton(JoyButton.A);
		Dodge.AddJoyButton(JoyButton.LeftShoulder);
		Heal.AddPhysicalKey(Key.Q);
		Heal.AddJoyButton(JoyButton.B);
		Interact.AddPhysicalKey(Key.E);
		Interact.AddJoyButton(JoyButton.Y);
		UseItem.AddPhysicalKey(Key.F);
		UseItem.AddJoyButton(JoyButton.DpadUp);
		NextItem.AddPhysicalKey(Key.Key2);
		NextItem.AddJoyButton(JoyButton.DpadRight);
		PlaceCauldron.AddPhysicalKey(Key.R);
		PlaceCauldron.AddJoyButton(JoyButton.DpadDown);
		PlayInstrument.AddPhysicalKey(Key.Ctrl);
		PlayInstrument.AddAxisPos(LeftTrigger.GetValue);
		Map.AddPhysicalKey(Key.M);
		Inventory.AddPhysicalKey(Key.I);
		Journal.AddPhysicalKey(Key.J);
		Submenu.AddPhysicalKey(Key.Tab);
		Submenu.AddJoyButton(JoyButton.Back);
		Pause.AddPhysicalKey(Key.Escape);
		Pause.AddJoyButton(JoyButton.Start);
	}
	public bool TryAddBind(SwInputBind inputBind)
	{
		switch (inputBind.InputType)
		{
			case SwInputBind.SwInputType.Button:
				if(TryGetButton(inputBind.InputName, out var button)) return button.TryAddBind(inputBind);
				return false;
			case SwInputBind.SwInputType.Axis:
				if(TryGetAxis(inputBind.InputName, out var axis)) return axis.TryAddBind(inputBind);
				return false;
			case SwInputBind.SwInputType.Axis2:
				if(TryGetAxis2(inputBind.InputName, out var axis2)) return axis2.TryAddBind(inputBind);
				return false;
			default:
				throw new Exception("Should be unreachable");
		}
	}
}
