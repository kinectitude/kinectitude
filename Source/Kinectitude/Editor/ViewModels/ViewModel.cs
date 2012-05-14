/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models.Base;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.ViewModels
{
    public static class ViewModel
    {
        private static readonly Dictionary<Game, GameViewModel> gameViewModels;
        private static readonly Dictionary<Scene, SceneViewModel> sceneViewModels;
        private static readonly Dictionary<Entity, EntityViewModel> entityViewModels;
        private static readonly Dictionary<Entity, Dictionary<string, AttributeViewModel>> attributeViewModels;

        static ViewModel()
        {
            gameViewModels = new Dictionary<Game, GameViewModel>();
            sceneViewModels = new Dictionary<Scene, SceneViewModel>();
            entityViewModels = new Dictionary<Entity, EntityViewModel>();
            attributeViewModels = new Dictionary<Entity, Dictionary<string, AttributeViewModel>>();
        }

        public static GameViewModel GetGameViewModel(Game game, IPluginNamespace pluginNamespace)
        {
            GameViewModel gameViewModel = null;
            gameViewModels.TryGetValue(game, out gameViewModel);
            if (null == gameViewModel)
            {
                gameViewModel = new GameViewModel(game, pluginNamespace);
                gameViewModels[game] = gameViewModel;
            }
            return gameViewModel;
        }

        public static SceneViewModel GetSceneViewModel(Scene scene)
        {
            SceneViewModel sceneViewModel = null;
            sceneViewModels.TryGetValue(scene, out sceneViewModel);
            if (null == sceneViewModel)
            {
                sceneViewModel = new SceneViewModel(scene);
                sceneViewModels[scene] = sceneViewModel;
            }
            return sceneViewModel;
        }
    }
}
*/