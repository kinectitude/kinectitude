
namespace Kinectitude.Core.Data
{
    /// <summary>
    /// Used to get the value of an expression
    /// </summary>
    public interface IExpressionReader : IChangeNotify
    {
        /// <summary>
        /// Gets the value of an expression
        /// </summary>
        /// <returns>The string value of an expression</returns>
        string GetValue();
    }
}
