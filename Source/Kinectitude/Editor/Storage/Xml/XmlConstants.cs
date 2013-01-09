using System.Xml.Linq;

namespace Kinectitude.Editor.Storage.Xml
{
    internal static class XmlConstants
    {
        private static readonly XNamespace Namespace = "http://www.kinectitude.com/2012/v1";

        public static readonly XName Game = Namespace + "Game";
        public static readonly XName Using = Namespace + "Using";
        public static readonly XName Define = Namespace + "Define";
        public static readonly XName Scene = Namespace + "Scene";
        public static readonly XName Manager = Namespace + "Manager";
        public static readonly XName PrototypeElement = Namespace + "Prototype";
        public static readonly XName Entity = Namespace + "Entity";
        public static readonly XName Component = Namespace + "Component";
        public static readonly XName Event = Namespace + "Event";
        public static readonly XName Condition = Namespace + "Condition";
        public static readonly XName Action = Namespace + "Action";

        public static readonly XName Xmlns = "xmlns";
        public static readonly XName Name = "Name";
        public static readonly XName Width = "Width";
        public static readonly XName Height = "Height";
        public static readonly XName IsFullScreen = "IsFullScreen";
        public static readonly XName FirstScene = "FirstScene";
        public static readonly XName Prototype = "Prototype";
        public static readonly XName Type = "Type";
        public static readonly XName File = "File";
        public static readonly XName Class = "Class";
        public static readonly XName If = "If";
    }
}
