//-----------------------------------------------------------------------
// <copyright file="RenderManager.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Attributes;
using Kinectitude.Core.Base;
using SlimDX;
using SlimDX.Direct2D;
using System;

namespace Kinectitude.Render
{
    [Plugin("Render Manager", "")]
    public class RenderManager : Manager<IRender>
    {
        private static float Clip(float input, float min, float max)
        {
            if (input > max)
            {
                return max;
            }
            else if (input < min)
            {
                return min;
            }

            return input;
        }

        private RenderService renderService;
        private Matrix3x2 cameraTransform;
        private float cameraX;
        private float cameraY;
        private float width;
        private float height;

        public SlimDX.DirectWrite.Factory DirectWriteFactory
        {
            get { return renderService.DirectWriteFactory; }
        }

        [PluginProperty("Scene Width", "", 0)]
        public float Width
        {
            get { return width != 0 ? width : renderService.Width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    Change("Width");
                }
            }
        }

        [PluginProperty("Scene Height", "", 0)]
        public float Height
        {
            get { return height != 0 ? height : renderService.Height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    Change("Height");
                }
            }
        }

        [PluginProperty("Camera X", "", 0)]
        public float CameraX
        {
            get { return cameraX; }
            set
            {
                if (cameraX != value)
                {
                    cameraX = Clip(value, 0, Width - renderService.Width);
                    UpdateCameraTransform();
                    Change("CameraX");
                }
            }
        }

        [PluginProperty("Camera Y", "", 0)]
        public float CameraY
        {
            get { return cameraY; }
            set
            {
                if (cameraY != value)
                {
                    cameraY = Clip(value, 0, Height - renderService.Height);
                    UpdateCameraTransform();
                    Change("CameraY");
                }
            }
        }

        public RenderManager()
        {
            renderService = GetService<RenderService>();
            UpdateCameraTransform();
        }

        private void UpdateCameraTransform()
        {
            cameraTransform = Matrix3x2.Translation(-CameraX, -CameraY);
        }

        public void Render(RenderTarget renderTarget)
        {
            foreach (IRender render in Children)
            {
                Matrix3x2 oldTransform = renderTarget.Transform;
                if (!render.FixedPosition)
                {
                    renderTarget.Transform = Matrix3x2.Multiply(oldTransform, cameraTransform);
                }
                render.Render(renderTarget);
                renderTarget.Transform = oldTransform;
            }
        }

        protected override void OnStart()
        {
            renderService.RenderAction += Render;
        }

        protected override void OnStop()
        {
            renderService.RenderAction -= Render;
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color, float opacity)
        {
            return renderService.GetSolidColorBrush(color, opacity);
        }

        public SolidColorBrush GetSolidColorBrush(string color, float opacity)
        {
            return renderService.GetSolidColorBrush(RenderService.ColorFromString(color), opacity);
        }

        public Bitmap GetBitmap(string image)
        {
            return renderService.GetBitmap(image);
        }

        public override void OnUpdate(float frameDelta)
        {
            foreach (IRender render in Children)
            {
                render.OnUpdate(frameDelta);
            }
        }
    }
}
