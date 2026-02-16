using SW.Src.Global;

namespace SW.Src.Utils;

public struct SwDelta<T>
{
    private T Value_ = default;
    private T LastValue = default;
    private bool Dirty = true;
    public T Value
    {
        readonly get =>Value_;
        set
        {
            LastValue = Value_;
            Value_ = value;
            Dirty = !SwGlobal.IsEqual(LastValue, Value_);
        }
    }
    public readonly bool IsDirty(){return Dirty;}
    public SwDelta(){}
    public SwDelta(T initialValue){Value_ = initialValue;}
}
