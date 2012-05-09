using Kinectitude.Core;
using Kinectitude.Core.Base;

namespace Kinectitude.Physics
{
    public interface IPhysics : IUpdateable
    {
        double X { get; set; }
        double Y { get; set; }
        double Dx { get; set; }
        double Dy { get; set; }
        double Width { get; }
        double Height { get; }

        bool Above(IPhysics other);
        bool HitTest(IPhysics other, double predTimestep);
    }
}
