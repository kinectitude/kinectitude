using Kinectitude.Core.Base;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using System;

namespace Kinectitude.Render
{    
    public class RenderManager : Manager<IRender>
    {

        private TextFormat textFormat;

        public TextFormat TextFormat
        {
            get { return textFormat; }
        }

        public RenderManager(Game game): base(game)
        {
            SlimDX.DirectWrite.Factory factory = Game.GetService<SlimDX.DirectWrite.Factory>();
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

        protected override void OnStart()
        {
            Action<RenderTarget> onRender = OnRender;
            Game.AddService(onRender);
        }

        protected override void OnStop()
        {
            Game.RemoveService(typeof(Action<RenderTarget>));
        }
    }
}
