using System.Collections.Generic;
using MessagePassing.Public;

namespace MessagePassing.Core
{
    internal sealed class Game : Actor
    {
        private Scene currentScene;
        private readonly List<Scene> scenes;

        public Game()
        {
            scenes = new List<Scene>();
        }

        public void AddScene(Scene scene)
        {
            currentScene = scene;   // hack
            scenes.Add(scene);
        }

        public override void Publish(string id, params object[] data)
        {
            // Publish only to the currently active scene
            currentScene.Publish(id, data);
        }

        public override void Subscribe(string id, MessageCallback callback, params object[] parameters)
        {
            // Subscribe to the same message id in every scene.
            foreach (Scene scene in scenes)
            {
                scene.Subscribe(id, callback, parameters);
            }
        }
    }
}
