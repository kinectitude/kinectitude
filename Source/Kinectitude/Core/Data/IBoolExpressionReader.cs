
namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to get bool values
    /// </summary>
    public interface IBoolExpressionReader : IChangeNotify
    {
        /// <summary>
        /// Gets the value of an expression
        /// </summary>
        /// <returns>The bool value of an expression</returns>
        bool GetValue();
    }
}
