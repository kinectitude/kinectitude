using System.Windows;
using Kinectitude.Core;
using System.Windows.Media;
using SlimDX.Direct2D;

namespace Kinectitude.Render
{
    public interface IRender : IUpdateable
    {
        void Render(RenderTarget renderTarget);
        void Initialize(RenderManager manager);
    }
}
