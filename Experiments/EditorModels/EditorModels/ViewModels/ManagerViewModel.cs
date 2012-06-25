using EditorModels.Models;

namespace EditorModels.ViewModels
{
    internal sealed class ManagerViewModel : BaseViewModel
    {
        private readonly PluginViewModel plugin;
        private readonly Manager manager;
        private Scene scene;

        public string Type
        {
            get { return plugin.ClassName; }
        }

        public PluginViewModel Plugin
        {
            get { return plugin; }
        }

        public ManagerViewModel(PluginViewModel plugin)
        {
            this.plugin = plugin;
            manager = new Manager();
        }

        public void SetScene(Scene scene)
        {
            if (null != this.scene)
            {
                this.scene.RemoveManager(manager);
            }

            this.scene = scene;

            if (null != this.scene)
            {
                this.scene.AddManager(manager);
            }
        }
    }
}
