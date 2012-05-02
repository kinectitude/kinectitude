using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kinectitude.Editor.Models;

namespace Kinectitude.Editor.Storage
{
    public interface IGameStorage
    {
        Game LoadGame();
        void SaveGame(Game game);
    }
}
