using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.AbstractComponents;
using Kinectitude.Core.Attributes;
using System.Windows.Forms;

namespace Kinectitude.Input
{
    [Plugin("Keyboard Motion Component", "")]
    [Provides(typeof(BasicFollowComponent))]
    public class KeyboardFollowComponent : BasicFollowComponent
    {

        private enum MoveDirection : int { Up = 0, Down = 1, Left = 2, Right = 3};
        private bool [] keysDown = {false, false, false, false};
        //used to see if defaults are needed
        private bool[] needsDefault = {true, true, true, true};
        private KeyboardManager manager;
         
        private KeyChangeCallback moveUp;
        [Preset("default", Keys.Up)]
        [PluginProperty("Up Key", "Up key to move with. Defaults to UpArrow")]
        public Keys Up
        {
            get { return moveUp.Key; }
            set { setKey("Up", moveUp, value, (int) MoveDirection.Up); }
        }

        private KeyChangeCallback moveDown;
        [Preset("default", Keys.Down)]
        [PluginProperty("Down Key", "Down key to move with. Defaults to DownArrow")]
        public Keys Down
        {
            get { return moveDown.Key; }
            set { setKey("Down", moveDown, value, (int) MoveDirection.Down); }
        }

        private KeyChangeCallback moveRight;
        [Preset("default", Keys.Right)]
        [PluginProperty("Right Key", "Right key to move with. Defaults to RightArrow")]
        public Keys Right
        {
            get { return moveRight.Key; }
            set { setKey("Right", moveRight, value, (int) MoveDirection.Right); }
        }

        private KeyChangeCallback moveLeft;
        [Preset("default", Keys.Left)]
        [PluginProperty("Left Key", "Left key to move with. Defaults to LeftArrow")]
        public Keys Left
        {
            get { return moveLeft.Key; }
            set { setKey("Left", moveLeft, value, (int)MoveDirection.Left); }
        }

        private float speed = 1;
        [Preset("default", 1)]
        [PluginProperty("Speed", "Speed to move at. Defaults to 1")]
        public float Speed
        {
            get { return speed; }
            set
            {
                if (speed != value)
                {
                    speed = value;
                    Change("Speed");
                }
            }
        }

        private void setKey(string change, KeyChangeCallback keyChangeCallback, Keys value, int who)
        {
            if (value != keyChangeCallback.Key)
            {
                if(manager != null) manager.RemoveIKeyChange(keyChangeCallback);
                keyChangeCallback.Key = value;
                if(manager != null) manager.RegisterIKeyChange(keyChangeCallback);
                needsDefault[who] = false;
                Change(change);
            }
        }

        private void setKeyDown(int key) { keysDown[key] = true; }

        public override void OnUpdate(float t)
        {
            float xMovement = keysDown[(int)MoveDirection.Right] ? 1 : 0;
            xMovement -= keysDown[(int)MoveDirection.Left] ? 1 : 0;
            xMovement *= speed;

            float yMovement = keysDown[(int)MoveDirection.Down] ? 1 : 0;
            yMovement -= keysDown[(int)MoveDirection.Up] ? 1 : 0;
            yMovement *= speed;

            UpdateDelta(xMovement, yMovement);

            //they will automatically be reset to true if they are down.
            for (int i = 0; i < keysDown.Length; i++) keysDown[i] = false;

            base.OnUpdate(t);
        }

        public override void Ready() 
        {
            base.Ready();
            manager = GetManager<KeyboardManager>();

            moveUp = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Up));
            if(needsDefault[(int)MoveDirection.Up]) moveUp.Key = Keys.Up;
            manager.RegisterIKeyChange(moveUp);

            moveDown = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Down));
            if(needsDefault[(int)MoveDirection.Down]) moveDown.Key = Keys.Down;
            manager.RegisterIKeyChange(moveDown);

            moveLeft = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Left));
            if (needsDefault[(int)MoveDirection.Left]) moveLeft.Key = Keys.Left;
            manager.RegisterIKeyChange(moveLeft);

            moveRight = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Right));
            if (needsDefault[(int)MoveDirection.Right]) moveRight.Key = Keys.Right;
            manager.RegisterIKeyChange(moveRight);

            manager.Add(this);
        }

        public override void Destroy()
        {
            manager.RemoveIKeyChange(moveUp);
            manager.RemoveIKeyChange(moveDown);
            manager.RemoveIKeyChange(moveLeft);
            manager.RemoveIKeyChange(moveRight);
        }
    }
}
