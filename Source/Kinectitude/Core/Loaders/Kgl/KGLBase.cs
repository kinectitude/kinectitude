//-----------------------------------------------------------------------
// <copyright file="KGLBase.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Irony.Parsing;
using Kinectitude.Core.Base;
using Kinectitude.Core.Data;
using Kinectitude.Core.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Loaders.Kgl
{
    internal abstract class KGLBase
    {
        protected readonly KinectitudeGrammar grammar = new KinectitudeGrammar();

        public object MakeAssignable(ParseTreeNode node, IScene scene, IDataContainer entity, Event evt)
        {
            if (node == null) return ConstantReader.TrueValue;
            if (node.Term == grammar.TypeMatcher) return makeTypeMatcher(node, scene);
            return makeValueReader(node, scene, entity, evt);
        }

        private TypeMatcher makeTypeMatcherHelper(ParseTreeNode node, IScene scene)
        {
            string strVal = node.ChildNodes[0].Token.ValueString;
            if (grammar.Constants.Constants.ContainsKey(strVal)) Game.CurrentGame.Die("Invalid use of the term " + strVal);

            if (node.Term == grammar.IsType)
                return new PrototypeTypeMatcher(scene.GetOfPrototype(strVal, false));

            return new PrototypeTypeMatcher(scene.GetOfPrototype(strVal, true));
        }

        private TypeMatcher makeTypeMatcher(ParseTreeNode node, IScene scene)
        {
            if (node.ChildNodes.Count == 1)
            {
                return makeTypeMatcherHelper(node.ChildNodes[0], scene);
            }
            else
            {
                List<TypeMatcher> readables = new List<TypeMatcher>();
                for (; node.ChildNodes.Count != 1; node = node.ChildNodes[2])
                    readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                readables.Add(makeTypeMatcherHelper(node.ChildNodes[0], scene));
                return new ListedTypeMatcher(readables);
            }
        }

        public ValueReader MakeAssignmentValue(ValueReader ls, ParseTreeNode binOpNode, ValueReader rs)
        {
            if (binOpNode == null) return rs;
            KinectitudeGrammar.OpCode opCode = grammar.OpLookup[binOpNode.Term];
            return binOpCreate(opCode, ls, rs);
        }

        private ValueReader makeParameterReader(IScene scene, IDataContainer entity, string name, string component, string param)
        {
            IDataContainer who = getDataContainer(scene, entity, name);
            IChangeable obj = who.GetChangeable(component);
            return ParameterValueReader.GetParameterValueReader(obj, param, who);
        }

        private ValueReader makeDcReader(IScene scene, IDataContainer entity, string name, string param)
        {
            IDataContainer dataContainer = getDataContainer(scene, entity, name);
            return DataContainerReader.GetDataContainerReader(dataContainer, param);
        }

        private ValueReader makeValueReader(ParseTreeNode node, IScene scene, IDataContainer entity, Event evt)
        {
            BnfTerm type0;
            //From is used for assignments where the node is not an expr.  It can only be an exactvalue, but not constant
            ParseTreeNode from;
            if (node.Term == grammar.Expr)
            {
                from = node.ChildNodes[0];
                type0 = from.Term;
            }
            else
            {
                from = node;
                type0 = node.Term;
            }

            if (type0 == grammar.UniOp)
            {
                ValueReader value = makeValueReader(node.ChildNodes[1], scene, entity, evt);
                return uniOpCreate(node.ChildNodes[0].ChildNodes[0].Term, value);
            }
            else if (type0 == grammar.Number)
            {
                return new ConstantReader(node.ChildNodes[0].Token.Value);
            }
            else if (type0 == grammar.ThreeVal)
            {
                ParseTreeNodeList list = from.ChildNodes;
                return makeParameterReader(scene, entity, list[0].Token.ValueString,
                    list[1].Token.ValueString, list[2].Token.ValueString);
            }
            else if (type0 == grammar.TwoVal)
            {
                ParseTreeNodeList list = from.ChildNodes;
                if (null == getDataContainer(scene, entity, list[0].Token.ValueString))
                    return makeParameterReader(scene, entity, "this", list[0].Token.ValueString, list[1].Token.ValueString);
                return makeDcReader(scene, entity, list[0].Token.ValueString, list[1].Token.ValueString);
            }
            else if (type0 == grammar.Identifier)
            {
                return makeDcReader(scene, entity, "this", from.Token.ValueString);
            }
            else if (type0 == grammar.Constants)
            {
                return from.Token.Value as ConstantReader;
            }
            else if (type0 == grammar.ParentVal)
            {
                switch (from.ChildNodes.Count)
                {
                    case 2:
                        return TypeMatcherDCReader.GetTypeMatcherDCValueReader(evt, from.ChildNodes[0].Token.ValueString,
                            from.ChildNodes[1].Token.ValueString, entity);
                    case 3:
                        return TypeMatcherProperyReader.GetTypeMatcherProperyReader(evt, from.ChildNodes[0].Token.ValueString,
                            from.ChildNodes[1].Token.ValueString, from.ChildNodes[2].Token.ValueString, entity);
                }
            }
            else if (type0 == grammar.Function)
            {
                string name = grammar.GetName(from);
                IEnumerable<ParseTreeNode> arguments = grammar.GetOfType(from, grammar.Argument);
                List<ValueReader> argReaders = new List<ValueReader>();
                foreach (ParseTreeNode argument in arguments) argReaders.Add(makeValueReader(argument.ChildNodes[0], scene, entity, evt));
                return FunctionReader.getFunctionReader(name, argReaders);
            }
            else if (node.ChildNodes.Count == 3 && node.ChildNodes[1].Term == grammar.BinOp)
            {
                ValueReader left = makeValueReader(node.ChildNodes[0], scene, entity, evt);
                ValueReader right = makeValueReader(node.ChildNodes[2], scene, entity, evt);
                KinectitudeGrammar.OpCode opCode = grammar.OpLookup[node.ChildNodes[1].ChildNodes[0].Term];
                return binOpCreate(opCode, left, right);
            }
            else if (node.ChildNodes.Count == 1 && node.ChildNodes[0].Term == grammar.Expr)
            {
                return makeValueReader(node.ChildNodes[0], scene, entity, evt);
            }
            return new ConstantReader(from.Token.Value);
        }


        private IDataContainer getDataContainer(IScene scene, IDataContainer entity, string name)
        {
            //TODO ask group about making the name of scene and game the keyword instead of game and scene.  Also allow this in game/scene?
            /*TODO if I have an entity created later, and it's in an action/evt/component should I be able to reference it?
             * Makes it difficult if I can't, but idk how to program this better if I can*/
            switch (name)
            {
                case "this":
                    return entity;
                case "scene":
                    return scene;
                case "game":
                    return scene.Game;
                default:
                   return scene.GetEntity(name);
            }
        }

        private ValueReader uniOpCreate(BnfTerm op, ValueReader value)
        {
            if (grammar.Not == op) return new NotOpReader(value);
            if (grammar.Minus == op) return new NegOpReader(value);
            Game.CurrentGame.Die("Error with implementation of operator " + op.Name);
            return null;
        }

        private ValueReader binOpCreate(KinectitudeGrammar.OpCode op, ValueReader left, ValueReader right)
        {
            switch (op)
            {
                case KinectitudeGrammar.OpCode.And: return new AndOpReader(left, right);
                case KinectitudeGrammar.OpCode.Div: return new DivOpReader(left, right);
                case KinectitudeGrammar.OpCode.Eql: return new EqlOpReader(left, right);
                case KinectitudeGrammar.OpCode.Ge: return new GeOpReader(left, right);
                case KinectitudeGrammar.OpCode.Gt: return new GtOpReader(left, right);
                case KinectitudeGrammar.OpCode.Le: return new GeOpReader(right, left);
                case KinectitudeGrammar.OpCode.LeftShift: return new LeftShiftOpReader(left, right);
                case KinectitudeGrammar.OpCode.Lt: return new GtOpReader(right, left);
                case KinectitudeGrammar.OpCode.Minus: return new MinusOpReader(left, right);
                case KinectitudeGrammar.OpCode.Mult: return new MultOpReader(left, right);
                case KinectitudeGrammar.OpCode.Neq: return new NeqOpReader(left, right);
                case KinectitudeGrammar.OpCode.Or: return new OrOpReader(left, right);
                case KinectitudeGrammar.OpCode.Plus: return new PlusOpReader(left, right);
                case KinectitudeGrammar.OpCode.Pow: return new PowOpReader(left, right);
                case KinectitudeGrammar.OpCode.Rem: return new RemOpReader(left, right);
                case KinectitudeGrammar.OpCode.RightShift: return new RightShiftOpReader(left, right);
                case KinectitudeGrammar.OpCode.Becomes: return right;
                default:
                    Game.CurrentGame.Die("Error with implementation of operator " + op);
                    return null;

            }
        }

        public PropertyHolder GetProperties(ParseTreeNode node)
        {
            PropertyHolder propertyHolder = new PropertyHolder();

            if (grammar.Prototype == node.Term || grammar.Entity == node.Term)
                node = node.ChildNodes.First(child => child.Term == grammar.EntityDefinition);

            foreach (ParseTreeNode property in grammar.GetOfType(node, grammar.Properties))
                propertyHolder.AddValue(property.ChildNodes[0].Token.ValueString, property.ChildNodes[1]);

            return propertyHolder;
        }
    }
}
