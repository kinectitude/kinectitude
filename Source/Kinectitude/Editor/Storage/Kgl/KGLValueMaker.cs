using Irony.Parsing;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Loaders.Kgl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KGLValueMaker : KGLBase, IValueMaker
    {
        public KGLValueMaker()
        {
            grammar.Root = grammar.Expr;
        }

        public ValueReader CreateValueReader(string value, IScene scene, IDataContainer entity, Event evt = null)
        {
            Parser parser = new Parser(grammar);
            ParseTree tree = parser.Parse(value);
            if (tree.HasErrors()) { /*TODO*/}
            return MakeAssignable(tree.Root, scene, entity, evt) as ValueReader;
        }
    }
}
