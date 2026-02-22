using SW.Src.Global;

namespace SW.Src.Utils;

public struct SwDirtyWrapper<T>(T value = default, bool isDirty = true)
{
    private T Value_ = value;
    public T Value
    {
        readonly get =>Value_; 
        set
        {
            if(SwStatic.IsEqual(Value_, value)) return;
            Value_ = value;
            IsDirty_ = true;
        }
    }
    private bool IsDirty_ = isDirty;
    public readonly bool IsDirty(){return IsDirty_;}
}
