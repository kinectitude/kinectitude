﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Data
{
    internal class DoubleDictionary<FirstHash, SecondHash, Item> where Item : class
    {
        private static readonly Dictionary<FirstHash, Dictionary<SecondHash, Item>> itemHash = 
            new Dictionary<FirstHash, Dictionary<SecondHash, Item>>();

        internal static Item GetItem(FirstHash key1, SecondHash key2, Func<Item> create)
        {
            Dictionary<SecondHash, Item> secondDict;
            if (!itemHash.TryGetValue(key1, out secondDict))
            {
                secondDict = new Dictionary<SecondHash, Item>();
                itemHash[key1] = secondDict;
            }
            Item item = null;
            secondDict.TryGetValue(key2, out item);
            if(item == null)
            {
                item = create();
                PutItem(key1, key2, item);
            }
            return item;
        }

        internal static void PutItem(FirstHash key1, SecondHash key2, Item item)
        {
            Dictionary<SecondHash, Item> secondDict;
            if (!itemHash.TryGetValue(key1, out secondDict))
            {
                secondDict = new Dictionary<SecondHash, Item>();
                itemHash[key1] = secondDict;
            }
            secondDict[key2] = item;
        }

        internal static void DeleteDict(FirstHash key1)
        {
            if(itemHash.ContainsKey(key1)) itemHash.Remove(key1);
        }
    }
}
