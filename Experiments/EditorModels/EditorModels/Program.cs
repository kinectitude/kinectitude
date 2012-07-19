﻿using EditorModels.Storage;
using EditorModels.ViewModels;

namespace EditorModels
{
    class Program
    {
        static void Main(string[] args)
        {
            IGameStorage storage = new XmlGameStorage("C:\\Users\\Brandon\\Development\\Kinectitude\\Source\\Kinectitude\\Editor\\Samples\\Pong\\game.xml");
            GameViewModel game = storage.LoadGame();

            storage = new XmlGameStorage("C:\\Users\\Brandon\\Desktop\\pongtest.xml");
            storage.SaveGame(game);
        }
    }
}
