using Kinectitude.Editor.Base;
using Kinectitude.Editor.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Models
{
    internal abstract class VisitableModel : BaseModel
    {
        public abstract void Accept(IGameVisitor visitor);
    }
}
