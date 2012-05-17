using Kinectitude.Core.Base;
using SlimDX.Direct2D;

namespace Kinectitude.Render
{
    public interface IRender : IUpdateable
    {
        void Render(RenderTarget renderTarget);
        void Initialize(RenderManager manager);
    }
}
