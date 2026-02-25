namespace SW.Src.Utils;

public class SwQueueOne<T>
{
    private bool HasValue = false;
    private T Value = default;
    public void Enqueue(T value)
    {
        Value = value;
        HasValue = true;
    }
    public bool TryDequeue(out T value)
    {
        if (HasValue)
        {
            value = Value;
            HasValue = false;
            return true;
        }
        value = default;
        return false;
    }
}
