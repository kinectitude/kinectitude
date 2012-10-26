using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Core.AbstractComponents;
using Kinectitude.Core.Attributes;
using SlimDX.DirectInput;

namespace Kinectitude.DirectInput
{
    [Plugin("Follow Component for the keyboard", "")]
    [Provides(typeof(BasicFollowComponent))]
    public class KeyboardFollowComponent : BasicFollowComponent
    {

        private enum MoveDirection : int { Up = 0, Down = 1, Left = 2, Right = 3};
        private bool [] keysDown = {false, false, false, false};
        //used to see if defaults are needed
        private bool[] needsDefault = {true, true, true, true};
        private KeyboardManager manager;
         
        private KeyChangeCallback moveUp;
        [Preset("default", Key.UpArrow)]
        [Plugin("Up key to move with", "Defaults to UpArrow")]
        public Key Up
        {
            get { return moveUp.Button; }
            set { setKey("Up", moveUp, value, (int) MoveDirection.Up); }
        }

        private KeyChangeCallback moveDown;
        [Preset("default", Key.DownArrow)]
        [Plugin("Down key to move with", "Defaults to DownArrow")]
        public Key Down
        {
            get { return moveDown.Button; }
            set { setKey("Down", moveDown, value, (int) MoveDirection.Down); }
        }

        private KeyChangeCallback moveRight;
        [Preset("default", Key.RightArrow)]
        [Plugin("Right key to move with", "Defaults to RightArrow")]
        public Key Right
        {
            get { return moveRight.Button; }
            set { setKey("Right", moveRight, value, (int) MoveDirection.Right); }
        }

        private KeyChangeCallback moveLeft;
        [Preset("default", Key.LeftArrow)]
        [Plugin("Left key to move with", "Defaults to LeftArrow")]
        public Key Left
        {
            get { return moveLeft.Button; }
            set { setKey("Left", moveLeft, value, (int)MoveDirection.Left); }
        }

        private float speed = 1;
        [Preset("default", 1)]
        [Plugin("Speed to move at", "Defaults to 1")]
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

        private void setKey(string change, KeyChangeCallback keyChangeCallback, Key value, int who)
        {
            if (value != keyChangeCallback.Button)
            {
                if(manager != null) manager.RemoveIKeyChange(keyChangeCallback);
                keyChangeCallback.Button = value;
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
            if(needsDefault[(int)MoveDirection.Up]) moveUp.Button = Key.UpArrow;
            manager.RegisterIKeyChange(moveUp);

            moveDown = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Down));
            if(needsDefault[(int)MoveDirection.Down]) moveDown.Button = Key.DownArrow;
            manager.RegisterIKeyChange(moveDown);

            moveLeft = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Left));
            if (needsDefault[(int)MoveDirection.Left]) moveLeft.Button = Key.LeftArrow;
            manager.RegisterIKeyChange(moveLeft);

            moveRight = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Right));
            if (needsDefault[(int)MoveDirection.Right]) moveRight.Button = Key.RightArrow;
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
