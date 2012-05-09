using System.Windows;
using System.Windows.Media;
using SlimDX.Direct2D;
using Kinectitude.Core.Base;

namespace Kinectitude.Render
{
    public interface IRender : IUpdateable
    {
        void Render(RenderTarget renderTarget);
        void Initialize(RenderManager manager);
    }
}
