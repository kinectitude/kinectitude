using System;
using System.Collections.Generic;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class MissingRequirementsException : Exception
    {

        private static string creationHelper(string identity, List<Type> missingTypes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(identity).Append(" is missing the following what(s)");
            foreach (Type missingType in missingTypes)
            {
                sb.Append(missingType.FullName).Append("\n");
            }
            return sb.ToString();
        }

        private MissingRequirementsException(string message) : base(message) { }

        internal static MissingRequirementsException MissingRequirement(string identity, List<Type> missingTypes)
        {
            return new MissingRequirementsException(creationHelper(identity, missingTypes));
        }
    }
}
