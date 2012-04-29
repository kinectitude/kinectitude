using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.Storage
{
    public interface IGameStorage
    {
        Game LoadGame();
        void SaveGame(Game game);
    }
}
