using SW.Src.Global;

namespace SW.Src.Utils;

public class SwDirtyWrapper<T>(T value = default, bool isDirty = true) : ISwIsDirty
{
    private T Value_ = value;
    public T Value
    {
        get =>Value_; 
        set
        {
            if(SwStatic.IsEqual(Value_, value)) return;
            Value_ = value;
            IsDirty_ = true;
        }
    }
    private bool IsDirty_ = isDirty;
    public bool IsDirty(){return IsDirty_;}
}
