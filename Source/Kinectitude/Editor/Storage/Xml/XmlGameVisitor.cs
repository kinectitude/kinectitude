using Kinectitude.Editor.Models;
using Kinectitude.Editor.Models.Properties;
using Kinectitude.Editor.Models.Statements.Actions;
using Kinectitude.Editor.Models.Statements.Assignments;
using Kinectitude.Editor.Models.Statements.Base;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using System.Linq;
using System.Xml.Linq;
using Action = Kinectitude.Editor.Models.Statements.Actions.Action;
using Attribute = Kinectitude.Editor.Models.Attribute;

namespace Kinectitude.Editor.Storage.Xml
{
    internal class XmlGameVisitor : IGameVisitor
    {
        private XObject result;

        public XmlGameVisitor() { }

        public XObject Apply(GameModel model)
        {
            model.Accept(this);
            return result;
        }

        public void Visit(Action action)
        {
            XElement element = new XElement(XmlConstants.Action, new XAttribute(XmlConstants.Type, action.Type));

            foreach (AbstractProperty property in action.Properties)
            {
                if (!property.IsInherited)
                {
                    element.Add(Apply(property));
                }
            }

            result = element;
        }

        public void Visit(Assignment assignment)
        {

        }

        public void Visit(Attribute attribute)
        {
            result = new XAttribute(attribute.Name, attribute.Value);
        }

        public void Visit(Component component)
        {
            XElement element = new XElement(XmlConstants.Component, new XAttribute(XmlConstants.Type, component.Type));

            foreach (AbstractProperty property in component.Properties)
            {
                if (property.IsLocal)
                {
                    element.Add(Apply(property));
                }
            }

            result = element;
        }

        public void Visit(Condition condition)
        {
            XElement element = new XElement(XmlConstants.Condition, new XAttribute(XmlConstants.If, condition.If));

            foreach (AbstractStatement statement in condition.Statements)
            {
                element.Add(Apply(statement));
            }

            result = element;
        }

        public void Visit(Define define)
        {
            result = new XElement(XmlConstants.Define, new XAttribute(XmlConstants.Name, define.Name), new XAttribute(XmlConstants.Class, define.Class));
        }

        public void Visit(Entity entity)
        {
            XName elementName = entity.IsPrototype ? XmlConstants.PrototypeElement : XmlConstants.Entity;

            XElement element = new XElement(elementName);

            if (null != entity.Name)
            {
                element.Add(new XAttribute(XmlConstants.Name, entity.Name));
            }

            if (entity.Prototypes.Count() > 0)
            {
                element.Add(new XAttribute(XmlConstants.Prototype, string.Join(" ", entity.Prototypes.Select(x => x.Name))));
            }

            foreach (Attribute attribute in entity.Attributes)
            {
                if (attribute.HasOwnValue)
                {
                    element.Add(Apply(attribute));
                }
            }

            foreach (Component component in entity.Components)
            {
                if (component.IsRoot || component.HasOwnValues)
                {
                    element.Add(Apply(component));
                }
            }

            foreach (AbstractEvent evt in entity.Events)
            {
                if (evt.IsLocal)
                {
                    element.Add(Apply(evt));
                }
            }

            result = element;
        }

        public void Visit(Event evt)
        {
            XElement element = new XElement(XmlConstants.Event, new XAttribute(XmlConstants.Type, evt.Type));

            foreach (AbstractProperty property in evt.Properties)
            {
                if (!property.IsInherited)
                {
                    element.Add(Apply(property));
                }
            }

            foreach (AbstractStatement statement in evt.Statements)
            {
                element.Add(Apply(statement));
            }

            result = element;
        }

        public void Visit(ForLoop loop)
        {

        }

        public void Visit(Game game)
        {
            XElement element = new XElement
            (
                XmlConstants.Game,
                new XAttribute(XmlConstants.Name, game.Name),
                new XAttribute(XmlConstants.Width, game.Width),
                new XAttribute(XmlConstants.Height, game.Height),
                new XAttribute(XmlConstants.IsFullScreen, game.IsFullScreen),
                new XAttribute(XmlConstants.FirstScene, game.FirstScene.Name)
            );

            foreach (Using use in game.Usings)
            {
                element.Add(Apply(use));
            }

            foreach (Entity entity in game.Prototypes)
            {
                element.Add(Apply(entity));
            }

            foreach (Attribute attribute in game.Attributes)
            {
                element.Add(Apply(attribute));
            }

            foreach (Scene scene in game.Scenes)
            {
                element.Add(Apply(scene));
            }

            result = element;
        }

        public void Visit(InheritedAction action)
        {

        }

        public void Visit(InheritedAssignment assignment)
        {

        }

        public void Visit(InheritedCondition condition)
        {

        }

        public void Visit(InheritedEvent evt)
        {

        }

        public void Visit(InheritedForLoop loop)
        {

        }

        public void Visit(InheritedProperty property)
        {

        }

        public void Visit(InheritedWhileLoop loop)
        {

        }

        public void Visit(Manager manager)
        {
            XElement element = new XElement(XmlConstants.Component, new XAttribute(XmlConstants.Type, manager.Type));

            foreach (AbstractProperty property in manager.Properties)
            {
                if (!property.IsInherited)
                {
                    element.Add(Apply(property));
                }
            }

            result = element;
        }

        public void Visit(Property property)
        {
            result = new XAttribute(property.Name, property.Value);
        }

        public void Visit(Scene scene)
        {
            XElement element = new XElement(XmlConstants.Scene, new XAttribute(XmlConstants.Name, scene.Name));

            foreach (Attribute attribute in scene.Attributes)
            {
                element.Add(Apply(attribute));
            }

            foreach (Manager manager in scene.Managers)
            {
                element.Add(Apply(manager));
            }

            foreach (Entity entity in scene.Entities)
            {
                element.Add(Apply(entity));
            }

            result = element;
        }

        public void Visit(Using use)
        {
            XElement element = new XElement(XmlConstants.Using, new XAttribute(XmlConstants.File, use.File));

            foreach (Define define in use.Defines)
            {
                element.Add(Apply(define));
            }

            result = element;
        }

        public void Visit(WhileLoop loop)
        {

        }
    }
}
