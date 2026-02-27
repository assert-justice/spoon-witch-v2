namespace SW.Src.Utils;

public class SwDirtyFlag(bool isDirty = true) : ISwIsDirty
{
    private bool IsDirty_ = isDirty;
    public bool IsDirty(){return IsDirty_;}
    public void SetDirty(){IsDirty_ = true;}
    public void Clean(){IsDirty_ = false;}
}
