using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Exceptions
{
    public class MissingRequirementsException : Exception
    {
        public string errorMessage;

        private void creationHelper(string name, Entity entity, List<Type> missingTypes)
        {
            string identity = null != entity.Name ? entity.Name : "Entity " + entity.Id.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(identity).Append(" is missing the following component(s)");
            foreach (Type missingType in missingTypes)
            {
                sb.Append(missingType.FullName).Append("\n");
            }
            sb.Append("required by ").Append(name);
            errorMessage = sb.ToString();
        }

        public MissingRequirementsException(Component component, List<Type> missingTypes)
        {
            creationHelper(component.GetType().FullName, component.Entity, missingTypes);
        }

        public MissingRequirementsException(Event Event, List<Type> missingTypes)
        {
            creationHelper(Event.GetType().FullName, Event.Entity, missingTypes);
        }
    }
}
