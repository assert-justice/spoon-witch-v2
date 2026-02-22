using System;
using SW.Src.Global;

namespace SW.Src.Utils;

public struct SwOnChange<T>
{
    private T Value_;
    public T Value
    {
        readonly get => Value_;
        set
        {
            if(IsEqual(Value_, value)) return;
            Value_ = OnChange(Value_, value);
        }
    }
    private readonly Func<T,T,T> OnChange;
    private readonly Func<T,T,bool> IsEqual = (l,n)=>SwStatic.IsEqual(l,n);
    public SwOnChange(Action callback, T value = default, Func<T,T,bool> isEqual = null)
    {
        T fn(T lastValue, T nextValue)
        {
            callback();
            return nextValue;
        }
        OnChange = fn;
        Value_ = value;
        if(isEqual is not null) IsEqual = isEqual;
    }
    public SwOnChange(Action<T> callback, T value = default, Func<T,T,bool> isEqual = null)
    {
        T fn(T lastValue, T nextValue)
        {
            callback(nextValue);
            return nextValue;
        }
        OnChange = fn;
        Value_ = value;
        if(isEqual is not null) IsEqual = isEqual;
    }
    public SwOnChange(Action<T,T> callback, T value = default, Func<T,T,bool> isEqual = null)
    {
        T fn(T lastValue, T nextValue)
        {
            callback(lastValue, nextValue);
            return nextValue;
        }
        OnChange = fn;
        Value_ = value;
        if(isEqual is not null) IsEqual = isEqual;
    }
    public SwOnChange(Func<T,T,T> callback, T value = default, Func<T,T,bool> isEqual = null)
    {
        OnChange = callback;
        Value_ = value;
        if(isEqual is not null) IsEqual = isEqual;
    }
}
