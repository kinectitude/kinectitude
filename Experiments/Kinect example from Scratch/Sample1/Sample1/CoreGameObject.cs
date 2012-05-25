using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample1
{
    class CoreGameObject
    {
        private LinkedList<CoreComponent> objs = new LinkedList <CoreComponent>();
        public double x;
        public double y;
        public void addComponent(CoreComponent component)
        {
            objs.AddLast(component);
        }
        public void removeComponent(CoreComponent component)
        {
            objs.Remove(component);
        }
    }
}
