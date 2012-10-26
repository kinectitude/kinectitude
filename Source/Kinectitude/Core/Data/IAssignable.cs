using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal interface IAssignable
    {
        /// <summary>
        /// Used to set parameter without needing to know the type of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="Param"></param>
        void SetParam(object obj, string Param);
    }
}
