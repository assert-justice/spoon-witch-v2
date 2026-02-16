using System.Collections.Generic;
using SW.Src.Global;

namespace SW.Src.Utils;

public struct SwDirty<T>
{
    private T Value_ = default;
    private bool Dirty = true;
    public T Value
    {
        get
        {
            Dirty = false;
            return Value_;
        }
        set
        {
            if(SwGlobal.IsEqual(value, Value_)) return;
            Dirty = true;
            Value_ = value;
        }
    }
    public SwDirty(){}
    public SwDirty(T value){Value = value;}
    public readonly bool IsDirty(){return Dirty;}
}
