//-----------------------------------------------------------------------
// <copyright file="KGLValueMaker.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Irony.Parsing;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Loaders.Kgl;
using Kinectitude.Editor.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglValueMaker : KGLBase, IValueMaker
    {
        private readonly Parser parser;

        public KglValueMaker()
        {
            grammar.Root = grammar.value;
            parser = new Parser(grammar);
        }

        public bool HasErrors(string value)
        {
            var tree = parser.Parse(value);
            return tree.HasErrors();
        }

        public ValueReader CreateValueReader(string value, IScene scene, IDataContainer entity, Event evt = null)
        {
            ParseTree tree = parser.Parse(value);   // Should always be valid since UI calls HasErrors first
            var assignable = MakeAssignable(tree.Root, scene, entity, evt);

            var reader = assignable as ValueReader;
            if (null == reader)
            {
                var typeMatcher = assignable as TypeMatcher;
                if (null != typeMatcher)
                {
                    reader = ConstantReader.NullValue;
                }
            }

            return reader;
        }
    }
}
