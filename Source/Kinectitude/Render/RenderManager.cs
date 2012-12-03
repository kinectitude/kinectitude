using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX;
using SlimDX.Direct2D;

namespace Kinectitude.Render
{
    [Plugin("Render Manager", "")]
    public class RenderManager : Manager<IRender>
    {
        private readonly RenderService renderService;

        public SlimDX.DirectWrite.Factory DirectWriteFactory
        {
            get { return renderService.DirectWriteFactory; }
        }

        public RenderManager()
        {
            renderService = GetService<RenderService>();

            SlimDX.DirectWrite.Factory factory = renderService.DirectWriteFactory;
        }

        public void Render(RenderTarget renderTarget)
        {
            foreach (IRender render in Children)
            {
                render.Render(renderTarget);
            }
        }

        protected override void OnStart()
        {
            renderService.RenderAction += Render;
        }

        protected override void OnStop()
        {
            renderService.RenderAction -= Render;
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color, float opacity)
        {
            return renderService.GetSolidColorBrush(color, opacity);
        }

        public SolidColorBrush GetSolidColorBrush(string color, float opacity)
        {
            return renderService.GetSolidColorBrush(RenderService.ColorFromString(color), opacity);
        }

        public Bitmap GetBitmap(string image)
        {
            return renderService.GetBitmap(image);
        }

        public override void OnUpdate(float frameDelta)
        {
            foreach (IRender render in Children)
            {
                render.OnUpdate(frameDelta);
            }
        }
    }
}
