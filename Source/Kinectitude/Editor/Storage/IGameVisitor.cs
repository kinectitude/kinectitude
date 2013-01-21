using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Editor.Models.Values;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameVisitor
    {
        void Visit(Action action);
        void Visit(Assignment assignment);
        void Visit(Attribute attribute);
        void Visit(Component component);
        void Visit(Condition condition);
        void Visit(Define define);
        void Visit(Entity entity);
        void Visit(Event evt);
        void Visit(ForLoop loop);
        void Visit(Game game);
        void Visit(ReadOnlyAction action);
        void Visit(ReadOnlyAssignment assignment);
        void Visit(ReadOnlyCondition condition);
        void Visit(ReadOnlyEvent evt);
        void Visit(ReadOnlyForLoop loop);
        void Visit(ReadOnlyProperty property);
        void Visit(ReadOnlyWhileLoop loop);
        void Visit(Manager manager);
        void Visit(Property property);
        void Visit(Scene scene);
        void Visit(Service service);
        void Visit(Using use);
        void Visit(Value val);
        void Visit(WhileLoop loop);
    }
}
