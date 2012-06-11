
namespace Kinectitude.Core.Base
{
    public interface IEntity
    {
        /// <summary>
        /// Returns the unique id of the enity.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the user-set name of the entity
        /// </summary>
        string Name { get; }
    }
}
