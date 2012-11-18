using Kinectitude.Editor.Base;
using Kinectitude.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = Kinectitude.Editor.Models.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameVisitor
    {
        void Visit(Action action);
        void Visit(Attribute attribute);
        void Visit(Component component);
        void Visit(Condition condition);
        void Visit(Define define);
        void Visit(Entity entity);
        void Visit(Event evt);
        void Visit(Game game);
        void Visit(InheritedAction action);
        void Visit(InheritedCondition condition);
        void Visit(InheritedEvent evt);
        void Visit(InheritedProperty property);
        void Visit(Manager manager);
        void Visit(Property property);
        void Visit(Scene scene);
        void Visit(Using use);
    }
}
