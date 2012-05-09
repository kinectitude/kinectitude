using Kinectitude.Core;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using Kinectitude.Core.Base;

namespace Kinectitude.Render
{    
    public class RenderManager : Manager<IRender>
    {
        private readonly TextFormat textFormat;

        public TextFormat TextFormat
        {
            get { return textFormat; }
        }
        
        public RenderManager(Game game) : base(game)
        {
            SlimDX.DirectWrite.Factory factory = game.GetService<SlimDX.DirectWrite.Factory>();
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
            foreach (IRender render in children)
            {
                render.Render(renderTarget);
            }
        }

        public override void OnStart()
        {
            game.OnRender = new RenderDelegate(OnRender);
        }

        public override void OnStop()
        {
            game.OnRender = null;
        }
    }
}
