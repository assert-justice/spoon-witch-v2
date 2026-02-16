using System;

namespace SW.Src.Input;

public interface ISwInput<T>
{
    public abstract void Poll();
    public abstract void Clear();
    public abstract void AddFn(Func<T> fn);
}
