namespace SW.Src.Entity;

public abstract class SwComponent<TParent, TApi>(TParent parent, TApi api)
{
    protected readonly TParent Parent = parent;
    protected readonly TApi Api = api;
    public virtual void Tick(float dt){}
}
