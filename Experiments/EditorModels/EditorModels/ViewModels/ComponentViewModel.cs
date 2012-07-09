using System.Collections.Generic;
using System.Linq;
using EditorModels.Models;
using EditorModels.ViewModels.Interfaces;

namespace EditorModels.ViewModels
{
    internal sealed class ComponentViewModel : BaseViewModel
    {
        private readonly PluginViewModel plugin;
        private readonly Component component;
        private readonly List<PropertyViewModel> properties;
        private Entity entity;
        private IComponentScope scope;

#if TEST

        public Component Component
        {
            get { return component; }
        }

#endif

        public event ScopeChangedEventHandler ScopeChanged;

        public PluginViewModel Plugin
        {
            get { return plugin; }
        }

        public string DisplayName
        {
            get { return plugin.DisplayName; }
        }

        public string Type
        {
            get { return null != scope ? scope.GetDefinedName(plugin) : plugin.ClassName; }
        }

        public string Provides
        {
            get { return plugin.Provides; }
        }

        public IEnumerable<string> Requires
        {
            get { return plugin.Requires; }
        }

        public bool HasLocalProperties
        {
            get { return Properties.Any(x => x.IsLocal); }
        }

        public bool IsInherited
        {
            get { return !IsRoot; }
        }

        public bool IsRoot
        {
            get { return null != scope ? !scope.HasInheritedComponent(plugin) : true; }
        }

        public IEnumerable<PropertyViewModel> Properties
        {
            get { return properties; }
        }

        public ComponentViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;
            component = new Component() { Type = this.Type };
            
            properties = new List<PropertyViewModel>();
            foreach (string property in plugin.Properties)
            {
                AddProperty(new PropertyViewModel(property));
            }
        }

        private void AddProperty(PropertyViewModel property)
        {
            property.SetComponent(component);
            properties.Add(property);
        }

        public void SetScope(Entity entity, IComponentScope scope)
        {
            if (null != this.scope)
            {
                this.scope.ScopeChanged -= OnScopeChanged;
                this.scope.DefineAdded -= OnDefineAdded;
                this.scope.DefineChanged -= OnDefinedNameChanged;
            }

            if (null != this.entity)
            {
                if (IsRoot)
                {
                    this.entity.RemoveComponent(component);
                }
            }

            this.scope = scope;
            this.entity = entity;

            if (null != this.scope)
            {
                this.scope.ScopeChanged += OnScopeChanged;
                this.scope.DefineAdded += OnDefineAdded;
                this.scope.DefineChanged += OnDefinedNameChanged;
                component.Type = Type;
            }

            if (null != this.entity)
            {
                if (IsRoot)
                {
                    this.entity.AddComponent(component);
                }
            }

            NotifyScopeChanged();
        }

        public bool DependsOn(ComponentViewModel requiredComponent)
        {
            return Requires.Contains(requiredComponent.Provides);
        }

        public PropertyViewModel GetProperty(string name)
        {
            return Properties.FirstOrDefault(x => x.Name == name);
        }

        private void OnDefineAdded(DefineViewModel define)
        {
            if (define.Class == plugin.ClassName)
            {
                UpdateType();
            }
        }

        private void OnScopeChanged()
        {
            UpdateType();
        }

        private void UpdateType()
        {
            component.Type = Type;
            NotifyPropertyChanged("Type");
        }

        private void NotifyScopeChanged()
        {
            if (null != ScopeChanged)
            {
                ScopeChanged();
            }
        }

        private void OnDefinedNameChanged(PluginViewModel plugin, string newName)
        {
            if (this.plugin == plugin)
            {
                UpdateType();
            }
        }
    }
}
