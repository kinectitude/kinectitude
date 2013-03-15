using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.ComponentInterfaces
{
    public delegate void ChangeEventHandler(string property);

    public interface ITransform
    {
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }
        float Rotation { get; set; }

        event ChangeEventHandler Changed;
    }
}
