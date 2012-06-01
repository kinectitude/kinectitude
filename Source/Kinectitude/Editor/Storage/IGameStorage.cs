using Kinectitude.Editor.Models.Base;

namespace Kinectitude.Editor.Storage
{
    internal interface IGameStorage
    {
        Game LoadGame();
        void SaveGame(Game game);
    }
}
