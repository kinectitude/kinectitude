//-----------------------------------------------------------------------
// <copyright file="ISound.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.ComponentInterfaces
{
    public interface ISound : IUpdateable
    {
        String Filename { get; set; }

        bool Looping { get; set; }

        float Volume { get; set; }
    }
}
