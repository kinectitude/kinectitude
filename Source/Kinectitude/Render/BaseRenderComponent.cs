//-----------------------------------------------------------------------
// <copyright file="BaseRenderComponent.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Core.Base;
using Kinectitude.Core.Components;
using SlimDX;
using RenderTarget = SlimDX.Direct2D.RenderTarget;
using Kinectitude.Core.Attributes;

namespace Kinectitude.Render
{
    [Provides(typeof(IRender)), Requires(typeof(TransformComponent))]
    public abstract class BaseRenderComponent : Component, IRender
    {
        protected TransformComponent transformComponent;
        protected RenderManager renderManager;

        private float opacity;
        [PluginProperty("Opacity", "", 1.0f)]
        public float Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity != value)
                {
                    opacity = value;
                    Change("Opacity");
                }
            }
        }

        private bool fixedPosition;
        [PluginProperty("Fixed Position", "", false)]
        public bool FixedPosition
        {
            get { return fixedPosition; }
            set
            {
                if (fixedPosition != value)
                {
                    fixedPosition = value;
                    Change("FixedPosition");
                }
            }
        }

        protected BaseRenderComponent()
        {
            Opacity = 1.0f;
        }

        public override void Ready()
        {
            renderManager = GetManager<RenderManager>();
            renderManager.Add(this);
            transformComponent = GetComponent<TransformComponent>();
            OnReady();
        }

        public void Render(RenderTarget renderTarget)
        {
            Matrix3x2 oldTransform = renderTarget.Transform;
            renderTarget.Transform = Matrix3x2.Multiply(oldTransform, Matrix3x2.Rotation(transformComponent.Rotation, new System.Drawing.PointF(transformComponent.X, transformComponent.Y)));
            OnRender(renderTarget);
            renderTarget.Transform = oldTransform;
        }

        public sealed override void Destroy()
        {
            renderManager.Remove(this);
            OnDestroy();
        }

        protected virtual void OnReady() { }

        protected virtual void OnDestroy() { }

        protected abstract void OnRender(RenderTarget renderTarget);
    }
}
