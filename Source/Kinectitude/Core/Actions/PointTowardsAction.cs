using Kinectitude.Core.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;
using Kinectitude.Core.Components;

namespace Kinectitude.Core.Actions
{
    [Plugin("point this entity towards ({X}, {Y})", "Point to location")]
    public class PointTowardsAction : Action
    {
        [PluginProperty("X", "X location to point towards")]
        public float X { get; set; }

        [PluginProperty("Y", "Y location to point towards")]
        public float Y { get; set; }

        public override void Run()
        {
            var transform = this.GetComponent<TransformComponent>();
            if (null != transform)
            {
                double angle = System.Math.Atan2(Y - transform.Y, X - transform.X);
                double degrees = angle * 180.0f / System.Math.PI;
                transform.Rotation = (float)degrees;
            }
        }
    }
}
