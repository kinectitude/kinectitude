//-----------------------------------------------------------------------
// <copyright file="IPhysics.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;

namespace Kinectitude.Core.ComponentInterfaces
{
    public enum BodyType { Dynamic, Kinematic, Static }

    public interface IPhysics : IUpdateable
    {
        float XVelocity { get; set; }

        float YVelocity { get; set; }

        float AngularVelocity { get; set; }

        float Restitution { get; set; }

        float Mass { get; set; }

        float Friction { get; set; }

        float LinearDamping { get; set; }

        BodyType BodyType { get; set; }
    }
}
