using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX;
using SlimDX.Direct2D;

namespace Kinectitude.Render
{
    [Plugin("Render Manager", "")]
    public class RenderManager : Manager<IRender>
    {
        private RenderService renderService;
        private Matrix3x2 cameraTransform;
        private float cameraX;
        private float cameraY;
        private float width;
        private float height;

        public SlimDX.DirectWrite.Factory DirectWriteFactory
        {
            get { return renderService.DirectWriteFactory; }
        }

        [PluginProperty("Camera X", "", 0)]
        public float CameraX
        {
            get { return cameraX; }
            set
            {
                if (cameraX != value)
                {
                    cameraX = value;
                    UpdateCameraTransform();
                    Change("CameraX");
                }
            }
        }

        [PluginProperty("Camera Y", "", 0)]
        public float CameraY
        {
            get { return cameraY; }
            set
            {
                if (cameraY != value)
                {
                    cameraY = value;
                    UpdateCameraTransform();
                    Change("CameraY");
                }
            }
        }

        [PluginProperty("Scene Width", "", 0)]
        public float Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    Change("Width");
                }
            }
        }

        [PluginProperty("Scene Height", "", 0)]
        public float Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    Change("Height");
                }
            }
        }

        public RenderManager()
        {
            renderService = GetService<RenderService>();
            UpdateCameraTransform();
        }

        private void UpdateCameraTransform()
        {
            cameraTransform = Matrix3x2.Translation(-CameraX, -CameraY);
        }

        public void Render(RenderTarget renderTarget)
        {
            foreach (IRender render in Children)
            {
                Matrix3x2 oldTransform = renderTarget.Transform;
                if (!render.FixedPosition)
                {
                    renderTarget.Transform = Matrix3x2.Multiply(oldTransform, cameraTransform);
                }
                render.Render(renderTarget);
                renderTarget.Transform = oldTransform;
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
