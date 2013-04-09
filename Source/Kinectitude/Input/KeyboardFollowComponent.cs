//-----------------------------------------------------------------------
// <copyright file="KeyboardFollowComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

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
        private enum MoveDirection : int { Up = 0, Down = 1, Left = 2, Right = 3 };
        private bool [] keysDown = { false, false, false, false };
        private KeyboardManager manager;
         
        private KeyChangeCallback moveUp;
        [PluginProperty("Up Key", "Up key to move with. Defaults to UpArrow", Keys.Up)]
        public Keys Up
        {
            get { return moveUp.Key; }
            set { setKey("Up", moveUp, value, (int) MoveDirection.Up); }
        }

        private KeyChangeCallback moveDown;
        [PluginProperty("Down Key", "Down key to move with. Defaults to DownArrow", Keys.Down)]
        public Keys Down
        {
            get { return moveDown.Key; }
            set { setKey("Down", moveDown, value, (int) MoveDirection.Down); }
        }

        private KeyChangeCallback moveRight;
        [PluginProperty("Right Key", "Right key to move with. Defaults to RightArrow", Keys.Right)]
        public Keys Right
        {
            get { return moveRight.Key; }
            set { setKey("Right", moveRight, value, (int) MoveDirection.Right); }
        }

        private KeyChangeCallback moveLeft;
        [PluginProperty("Left Key", "Left key to move with. Defaults to LeftArrow", Keys.Left)]
        public Keys Left
        {
            get { return moveLeft.Key; }
            set { setKey("Left", moveLeft, value, (int)MoveDirection.Left); }
        }

        private float speed = 1;
        [PluginProperty("Speed", "Speed to move at. Defaults to 1", 1.0f)]
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

        public KeyboardFollowComponent()
        {
            moveUp = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Up));
            moveDown = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Down));
            moveLeft = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Left));
            moveRight = new KeyChangeCallback(() => setKeyDown((int)MoveDirection.Right));

            //Up = Keys.Up;
            //Down = Keys.Down;
            //Left = Keys.Left;
            //Right = Keys.Right;

            //Speed = 1.0f;
        }

        private void setKey(string change, KeyChangeCallback keyChangeCallback, Keys value, int who)
        {
            if (value != keyChangeCallback.Key)
            {
                if (manager != null) manager.RemoveIKeyChange(keyChangeCallback);
                keyChangeCallback.Key = value;
                if (manager != null) manager.RegisterIKeyChange(keyChangeCallback);
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
            
            moveUp.Key = Up;
            moveDown.Key = Down;
            moveLeft.Key = Left;
            moveRight.Key = Right;

            manager.RegisterIKeyChange(moveUp);
            manager.RegisterIKeyChange(moveDown);
            manager.RegisterIKeyChange(moveLeft);
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
