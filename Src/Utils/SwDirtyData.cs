namespace SW.Src.Utils;

public class SwDirtyData<T>
{
    private T Value;
    private int Hash = 0;
    private bool HasValue = false;
    private string Path;
    public SwDirtyData(){}
}
