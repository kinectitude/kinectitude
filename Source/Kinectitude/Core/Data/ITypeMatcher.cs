using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to match types
    /// </summary>
    public interface ITypeMatcher
    {
        /// <summary>
        /// Checks for a match with an entity, and sets the entity if it matches.
        /// Once an entity is matched, it may be used by an Expression if it refers to it by name.
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns>If the entity matches the type that ITypeMatcher holds</returns>
        bool MatchAndSet(IDataContainer entity);
    }
}
