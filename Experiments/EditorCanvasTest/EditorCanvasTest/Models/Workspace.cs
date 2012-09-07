using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace EditorCanvasTest.Models
{
    public class Workspace
    {
        private static readonly Lazy<Workspace> instance = new Lazy<Workspace>();

        public static Workspace Instance
        {
            get { return instance.Value; }
        }

        public ObservableCollection<Entity> Entities
        {
            get;
            private set;
        }

        public Workspace()
        {
            Entities = new ObservableCollection<Entity>();

            Entities.Add(new Entity("Coin", 50, 50, 60, 60, new RenderComponent(RenderComponent.ShapeType.Ellipse, "Yellow")));
            Entities.Add(new Entity("Block", 100, 100, 200, 200, new RenderComponent(RenderComponent.ShapeType.Rectangle, "Blue")));
        }
    }
}
