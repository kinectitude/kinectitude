using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;

namespace Kinectitude.Render
{
    [Requires(typeof(TransformComponent))]
    public interface IRender : IUpdateable
    {
        void Render(RenderTarget renderTarget);
    }
}
