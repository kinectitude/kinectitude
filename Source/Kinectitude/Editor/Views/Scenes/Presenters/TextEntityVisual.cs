//-----------------------------------------------------------------------
// <copyright file="TextEntityVisual.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Models;
using Kinectitude.Render;
using System;
using System.Windows.Media;

namespace Kinectitude.Editor.Views.Scenes.Presenters
{
    internal class TextEntityVisual : EntityVisual
    {
        public string Value
        {
            get { return GetStringValue<TextRenderComponent>("Value"); }
        }

        public string FontFamily
        {
            get { return GetStringValue<TextRenderComponent>("FontFamily"); }
        }

        public System.Windows.FontWeight FontWeight
        {
            get { return ConvertFontWeight(GetEnumValue<TextRenderComponent, SlimDX.DirectWrite.FontWeight>("FontWeight")); }
        }

        public System.Windows.FontStyle FontStyle
        {
            get { return ConvertFontStyle(GetEnumValue<TextRenderComponent, SlimDX.DirectWrite.FontStyle>("FontStyle")); }
        }

        public System.Windows.FontStretch FontStretch
        {
            get { return ConvertFontStretch(GetEnumValue<TextRenderComponent, SlimDX.DirectWrite.FontStretch>("FontStretch")); }
        }

        public double FontSize
        {
            get { return GetDoubleValue<TextRenderComponent>("FontSize"); }
        }

        public Brush FontColor
        {
            get
            {
                try
                {
                    var b = (Brush)BrushConverter.ConvertFromString(GetStringValue<TextRenderComponent>("FontColor"));
                    return b;
                }
                catch (Exception)
                {
                    return Brushes.Black;
                }
            }
        }

        public System.Windows.TextAlignment TextAlignment
        {
            get { return ConvertTextAlignment(GetEnumValue<TextRenderComponent, SlimDX.DirectWrite.TextAlignment>("TextAlignment")); }
        }

        public TextEntityVisual(EntityPresenter presenter, Component render, Entity entity) : base(presenter, render, entity) { }

        private System.Windows.FontWeight ConvertFontWeight(SlimDX.DirectWrite.FontWeight fontWeight)
        {
            switch (fontWeight)
            {
                case SlimDX.DirectWrite.FontWeight.Black: return System.Windows.FontWeights.Black;
                case SlimDX.DirectWrite.FontWeight.Bold: return System.Windows.FontWeights.Bold;
                case SlimDX.DirectWrite.FontWeight.DemiBold: return System.Windows.FontWeights.DemiBold;
                case SlimDX.DirectWrite.FontWeight.ExtraBlack: return System.Windows.FontWeights.ExtraBlack;
                case SlimDX.DirectWrite.FontWeight.ExtraBold: return System.Windows.FontWeights.ExtraBold;
                case SlimDX.DirectWrite.FontWeight.ExtraLight: return System.Windows.FontWeights.ExtraLight;
                case SlimDX.DirectWrite.FontWeight.Light: return System.Windows.FontWeights.Light;
                case SlimDX.DirectWrite.FontWeight.Medium: return System.Windows.FontWeights.Medium;
                case SlimDX.DirectWrite.FontWeight.Normal: return System.Windows.FontWeights.Normal;
                case SlimDX.DirectWrite.FontWeight.Thin: return System.Windows.FontWeights.Thin;
            }

            return default(System.Windows.FontWeight);
        }

        private System.Windows.FontStyle ConvertFontStyle(SlimDX.DirectWrite.FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case SlimDX.DirectWrite.FontStyle.Italic: return System.Windows.FontStyles.Italic;
                case SlimDX.DirectWrite.FontStyle.Normal: return System.Windows.FontStyles.Normal;
                case SlimDX.DirectWrite.FontStyle.Oblique: return System.Windows.FontStyles.Oblique;
            }

            return default(System.Windows.FontStyle);
        }

        private System.Windows.FontStretch ConvertFontStretch(SlimDX.DirectWrite.FontStretch fontStretch)
        {
            switch (fontStretch)
            {
                case SlimDX.DirectWrite.FontStretch.Condensed: return System.Windows.FontStretches.Condensed;
                case SlimDX.DirectWrite.FontStretch.Expanded: return System.Windows.FontStretches.Expanded;
                case SlimDX.DirectWrite.FontStretch.ExtraCondensed: return System.Windows.FontStretches.ExtraCondensed;
                case SlimDX.DirectWrite.FontStretch.ExtraExpanded: return System.Windows.FontStretches.ExtraExpanded;
                case SlimDX.DirectWrite.FontStretch.Medium: return System.Windows.FontStretches.Medium;
                case SlimDX.DirectWrite.FontStretch.SemiCondensed: return System.Windows.FontStretches.SemiCondensed;
                case SlimDX.DirectWrite.FontStretch.SemiExpanded: return System.Windows.FontStretches.SemiExpanded;
                case SlimDX.DirectWrite.FontStretch.UltraCondensed: return System.Windows.FontStretches.UltraCondensed;
                case SlimDX.DirectWrite.FontStretch.UltraExpanded: return System.Windows.FontStretches.UltraExpanded;
            }

            return default(System.Windows.FontStretch);
        }

        private System.Windows.TextAlignment ConvertTextAlignment(SlimDX.DirectWrite.TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case SlimDX.DirectWrite.TextAlignment.Center: return System.Windows.TextAlignment.Center;
                case SlimDX.DirectWrite.TextAlignment.Leading: return System.Windows.TextAlignment.Left;
                case SlimDX.DirectWrite.TextAlignment.Trailing: return System.Windows.TextAlignment.Right;
            }
            
            return default(System.Windows.TextAlignment);
        }
    }
}
