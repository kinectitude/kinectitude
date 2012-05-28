
using System.Collections.Generic;
namespace MessagePassing.Public
{
    public interface IComponentContainer
    {
        string Name { get; }
        IDictionary<string, object> Variables { get; }
        
        T GetManager<T>() where T : class, IManager, new();
        bool MatchesType(string type);
    }
}
