using Kinectitude.Attributes;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;

namespace Kinectitude.Render
{
    [Plugin("Set text", "")]
    public class SetTextAction : Action
    {
        [Plugin("Value", "")]
        public SpecificReadable Value { get; set; }

        public SetTextAction() { }

        public override void Run()
        {
            TextRenderComponent tx = Event.Entity.GetComponent(typeof(TextRenderComponent)) as TextRenderComponent;
            tx.OnSetTextAction(this);
        }
    }
}
