
namespace Kinectitude.Editor.Models.Base
{
    internal interface IAction
    {
        IActionContainer Parent { get; set; }
    }
}
