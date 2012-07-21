using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Loaders
{
    internal abstract class LanguageKeywords
    {
        internal abstract string Name { get; }
        internal abstract string Prototype { get; }
        internal abstract string Type { get; }
        internal abstract string If { get; }
        internal abstract string FirstScene { get; }
        internal abstract string File { get; }
        internal abstract string Class { get; }
        internal abstract string Entity { get; }
        internal abstract string Action { get; }
        internal abstract string Condition { get; }
        internal abstract string Manager { get; }
        internal abstract string Event { get; }
        internal abstract string Trigger { get; }
        internal abstract string Component { get; }
        internal abstract string Using { get; }
        internal abstract string Define { get; }
        internal abstract string Scene { get; }
        internal abstract string Plugins { get; }

        private static readonly Dictionary<string, LanguageKeywords> langDict;

        static LanguageKeywords()
        {
            EnglishLanguageKeywords english = new EnglishLanguageKeywords();
            langDict = new Dictionary<string, LanguageKeywords>()
            {
                {"en",english},
                {"eng", english},
                {"english", english}
            };
        }

        internal static LanguageKeywords GetLanguage(string lang)
        {
            lang = lang.Split('-')[0];
            LanguageKeywords keywords;
            if (langDict.TryGetValue(lang, out keywords)) return keywords;
            throw new ArgumentException("Invalid language");
        }

    }
}
