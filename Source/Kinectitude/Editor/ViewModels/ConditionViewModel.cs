using System.Collections.Generic;
using System.Linq;

namespace Kinectitude.Editor.ViewModels
{
    internal class ConditionViewModel : AbstractConditionViewModel
    {
        private string rule;

        public override string If
        {
            get { return rule; }
            set
            {
                if (rule != value)
                {
                    rule = value;
                    NotifyPropertyChanged("If");
                }
            }
        }

        public override string Type
        {
            get { return null; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public override bool IsInherited
        {
            get { return false; }
        }

        public override IEnumerable<PluginViewModel> Plugins
        {
            get { return Actions.SelectMany(x => x.Plugins).Distinct(); }
        }

        public ConditionViewModel()
        {
            
        }

        public override bool InheritsFrom(AbstractActionViewModel action)
        {
            return false;
        }
    }
}
