using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameStorage
    {
        Game LoadGame();
        void SaveGame(Game game);
    }
}
