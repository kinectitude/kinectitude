using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using Kinectitude.Core.Attributes;
using Kinectitude.Core.Components;
using Kinectitude.Core.ComponentInterfaces;

namespace Kinectitude.Render
{
    [Requires(typeof(TransformComponent))]
    public interface IRender : IUpdateable
    {
        bool FixedPosition { get; }

        void Render(RenderTarget renderTarget);
    }
}
