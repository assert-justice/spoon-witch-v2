namespace SW.Src.Utils;

public struct SwDirtyFlag(bool isDirty = true)
{
    private bool IsDirty_ = isDirty;
    public readonly bool IsDirty(){return IsDirty_;}
    public void SetDirty(){IsDirty_ = true;}
    public void Clean(){IsDirty_ = false;}
}
