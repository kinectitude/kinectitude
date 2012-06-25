using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EditorModels.ViewModels;
using EditorModels.Models;

namespace EditorModels.Storage
{
    internal interface IGameStorage
    {
        GameViewModel LoadGame();
        void SaveGame(Game game);
    }
}
