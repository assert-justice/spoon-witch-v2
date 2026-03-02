using System.Collections.Generic;
using Godot;

namespace SW.Src.Global;

public static class SwMath
{
    public static bool IsEqual<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }
    public static int GetRandomIndex(int length)
    {
        return Mathf.FloorToInt(GD.Randf() * length);
    }
    public static bool TryChooseRandom<T>(IReadOnlyList<T> values, out T value)
    {
        value = default;
        if(values.Count == 0) return false;
        int idx = Mathf.FloorToInt(GD.Randf() * values.Count);
        value = values[idx];
        return true;
    }
    public static T ChooseRandom<T>(IReadOnlyList<T> values, string errorMessage)
    {
        if(TryChooseRandom(values, out T value)) return value;
        SwStatic.LogError(errorMessage);
        return default;
    }
    public static float NormalizeAngle(float angle)
	{
		return angle >= 0 ? angle : angle + Mathf.Tau;
	}
    public static float SegmentAngle(float angle, int segments, float offset = 0)
	{
        angle = NormalizeAngle(angle);
		float mul = segments / Mathf.Tau;
		return Mathf.Round((angle + offset) * mul) / mul;
	}
	public static int RoundAngleToInt(float angle, int segments, float offset = 0)
	{
        angle = NormalizeAngle(angle);
		float mul = segments / Mathf.Tau;
		return Mathf.RoundToInt((angle + offset) * mul) % segments;
	}
}
