//-----------------------------------------------------------------------
// <copyright file="TypeMatcherDCReader.cs" company="Kinectitude">
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
using Kinectitude.Core.Base;

namespace Kinectitude.Core.Data
{
    internal sealed class TypeMatcherDCReader : RepeatReader
    {
        internal readonly TypeMatcherWatcher Watcher;
        internal readonly string Param;
        Entity lastEntity = null;

        protected override ValueReader Reader
        {
            get { return Watcher.GetTypeMatcher()[Param]; }
        }

        internal static TypeMatcherDCReader GetTypeMatcherDCValueReader(object obj, string objParam, string param, IDataContainer owner)
        {
            TypeMatcherWatcher watcher = TypeMatcherWatcher.GetTypeMatcherWatcher(obj, objParam, owner);
            Func<TypeMatcherDCReader> create = new Func<TypeMatcherDCReader>(() => new TypeMatcherDCReader(watcher, param));
            return DoubleDictionary<TypeMatcherWatcher, string, TypeMatcherDCReader>.GetItem(watcher, param, create);
        }

        private TypeMatcherDCReader(TypeMatcherWatcher watcher, string param)
        {
            Param = param;
            Watcher = watcher;
            Watcher.NotifyOfChange(this);
        }

        private void typeMatcherChange()
        {
            if (lastEntity != null) lastEntity.NotifyOfChange(Param, this);
            Entity entity = Watcher.GetTypeMatcher().Entity;
            lastEntity = entity;
            entity.StopNotifications(Param, this);
            ((IChanges)this).Change();
        }
        internal override ValueWriter ConvertToWriter() { return new TypeMatcherDCWriter(this); }
    }
}
