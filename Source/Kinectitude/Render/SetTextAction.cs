using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Render
{
    [Plugin("Set text", "")]
    public class SetTextAction : Action
    {
        [Plugin("Value", "")]
        public IExpressionReader Value { get; set; }

        public SetTextAction() { }

        public override void Run()
        {
            TextRenderComponent tx = GetComponent<TextRenderComponent>();
            tx.OnSetTextAction(this);
        }
    }
}
