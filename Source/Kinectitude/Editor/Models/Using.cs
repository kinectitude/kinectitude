﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Kinectitude.Editor.Models
{
    public class Using
    {
        private string path;
        //private readonly List<Alias> _aliases;
        //private readonly ReadOnlyCollection<Alias> aliases;
        private readonly SortedDictionary<string, Alias> aliases;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public IEnumerable<Alias> Aliases
        {
            get { return aliases.Values; }
        }

        public Using()
        {
            //_aliases = new List<Alias>();
            //aliases = new ReadOnlyCollection<Alias>(_aliases);
            aliases = new SortedDictionary<string, Alias>();
        }

        public void AddAlias(Alias alias)
        {
            aliases.Add(alias.Name, alias);
        }

        public void RemoveAlias(Alias alias)
        {
            aliases.Remove(alias.Name);
        }

        public Alias GetAlias(string name)
        {
            Alias ret = null;
            aliases.TryGetValue(name, out ret);
            return ret;
        }
    }
}
