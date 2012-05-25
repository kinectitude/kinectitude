using System;
using System.Collections.Generic;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Exceptions
{
    internal sealed class MissingRequirementsException : Exception
    {

        private static string creationHelper(string name, IDataContainer entity, List<Type> missingTypes)
        {
            string identity = null != entity.Name ? entity.Name : "Entity " + entity.Id.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(identity).Append(" is missing the following component(s)");
            foreach (Type missingType in missingTypes)
            {
                sb.Append(missingType.FullName).Append("\n");
            }
            sb.Append("required by ").Append(name);
            return sb.ToString();
        }

        private MissingRequirementsException(string message) : base(message) { }

        internal static MissingRequirementsException MissingRequirement(Component component, List<Type> missingTypes)
        {
            return new MissingRequirementsException
                (creationHelper(component.GetType().FullName, component.Entity, missingTypes));
        }

        internal static MissingRequirementsException MissingRequirement(Event Event, List<Type> missingTypes)
        {
            return new MissingRequirementsException
                (creationHelper(Event.GetType().FullName, Event.Entity, missingTypes));
        }
    }
}
