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
            grammar.Root = grammar.Expr;
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
            return MakeAssignable(tree.Root, scene, entity, evt) as ValueReader;
        }
    }
}
