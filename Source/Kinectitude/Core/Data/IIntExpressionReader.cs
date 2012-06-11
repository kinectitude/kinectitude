
namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to get int values
    /// </summary>
    public interface IIntExpressionReader : IChangeNotify
    {
        /// <summary>
        /// Gets the value of an expression
        /// </summary>
        /// <returns>The int value of an expression</returns>
        int GetValue();
    }
}
