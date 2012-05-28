using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using Factory = SlimDX.DirectWrite.Factory;

namespace Kinectitude.Render
{    
    public class RenderManager : Manager<IRender>
    {

        private TextFormat textFormat;
        private static RenderService renderService = null;
        public TextFormat TextFormat
        {
            get { return textFormat; }
        }

        public RenderManager(): base()
        {
            if (null == renderService)
            {
                renderService = GetService<RenderService>();
            }
            Factory factory = renderService.Factory;
            textFormat = factory.CreateTextFormat("Arial", FontWeight.Regular, FontStyle.Normal, FontStretch.Normal, 36.0f, "en-us");
            textFormat.FlowDirection = FlowDirection.TopToBottom;
            textFormat.IncrementalTabStop = textFormat.FontSize * 4.0f;
            textFormat.ParagraphAlignment = ParagraphAlignment.Near;
            textFormat.ReadingDirection = ReadingDirection.LeftToRight;
            textFormat.TextAlignment = TextAlignment.Leading;
            textFormat.WordWrapping = WordWrapping.NoWrap;
        }

        protected override void OnAdd(IRender item)
        {
            item.Initialize(this);
        }

        public void OnRender(RenderTarget renderTarget)
        {
            foreach (IRender render in Children)
            {
                render.Render(renderTarget);
            }
        }

        protected override void OnStart()
        {
            if (null == renderService)
            {
                renderService = GetService<RenderService>();
            }
            renderService.RenderTargetAction = OnRender;
        }

        protected override void OnStop()
        {
            renderService.RenderTargetAction = null;
        }
    }
}
