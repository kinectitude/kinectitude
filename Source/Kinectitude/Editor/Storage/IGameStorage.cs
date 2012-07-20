using Kinectitude.Editor.ViewModels;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameStorage
    {
        GameViewModel LoadGame();
        void SaveGame(GameViewModel game);
    }
}
