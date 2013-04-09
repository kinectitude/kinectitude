//-----------------------------------------------------------------------
// <copyright file="Asset.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Base;

namespace Kinectitude.Editor.Models
{
    class Asset : BaseModel
    {
        public string File { get; private set; }

        public Asset(string file)
        {
            File = file;
        }
    }
}
