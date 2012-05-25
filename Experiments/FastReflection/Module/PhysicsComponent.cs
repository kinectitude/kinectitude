using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;

namespace Module
{
    public class PhysicsComponent : Component
    {
        public double Dx { get; set; }
        public double Dy { get; set; }
        public string Name { get; set; }

        public PhysicsComponent() { }

        public override void OnUpdate(double frameDelta)
        {
            throw new NotImplementedException();
        }
    }
}
