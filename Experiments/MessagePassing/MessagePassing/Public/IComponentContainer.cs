
namespace MessagePassing.Public
{
    public interface IComponentContainer
    {
        T GetManager<T>() where T : class, IManager, new();
    }
}
