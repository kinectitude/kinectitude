using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
//XNA is not included in this project, only farseer.  Farseer will see this and define the Xna Stuff it needs.
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Collision.Shapes;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            World w = new World(Vector2.Zero);
            w.Gravity = new Vector2(0f, 0f);
            Body body = BodyFactory.CreateCircle(w, 2f, 2f, 1f);
            body.Position = new Vector2(2f, 2f);
            body.BodyType = BodyType.Dynamic;
            body.Restitution = 1f;
            body.Mass = 1f;
            Transform t;
            body.GetTransform(out t);
            body.ApplyForce(new Vector2(140f, 0));
            Console.WriteLine(t.Position + " " + body.LinearVelocity + " " + body.Position);
            //100 ms passed;
            w.Step(0.1f);
            body.GetTransform(out t);
            Console.WriteLine(t.Position + " " + body.LinearVelocity);
            //body.ApplyLinearImpulse(new Vector2(0.1f, 0));
            w.Step(0.1f);
            body.GetTransform(out t);
            Console.WriteLine(t.Position + " " + body.LinearVelocity);
            Console.WriteLine("...");
            //body.ApplyLinearImpulse(new Vector2(0.8f, 0));
            w.Step(0.2f);
            body.GetTransform(out t);
            Console.WriteLine(t.Position + " " + body.LinearVelocity);
            Console.WriteLine("DONE!");
            Console.ReadKey();
        }
    }
}