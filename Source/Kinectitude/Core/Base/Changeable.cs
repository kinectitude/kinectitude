using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    public class Changeable : IChangeable
    {
        internal DataContainer DataContainer { get; set; }

        bool IChangeable.ShouldCheck { get; set; }

        public static float ScaleX(float X) { return X * Game.CurrentGame.ScaleX; }
        public static float ScaleY(float Y) { return Y * Game.CurrentGame.ScaleY; }

        /// <summary>
        /// Returns the x,y of the current size of the window
        /// </summary>
        /// <returns></returns>
        public static Tuple<int, int> GetWindowSize() { return Game.CurrentGame.GetWinowSize(); }

        public void OffsetByWindow(ref int x, ref int y){
            Tuple<int, int> offset = Game.CurrentGame.GetWidowOffset();
            x -= offset.Item1;
            y -= offset.Item2;
        }

        protected void Change(string str)
        {
            if (((IChangeable) this).ShouldCheck) DataContainer.ChangedProperty(new Tuple<IChangeable, string>((IChangeable)this, str));
        }
    }
}
