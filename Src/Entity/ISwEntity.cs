using SW.Src.Utils;

namespace SW.Src.Entity;

public interface ISwEntity
{
    public abstract bool TryInit(SwJsonDb db);
}
