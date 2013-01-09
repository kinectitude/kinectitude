using Kinectitude.Editor.Models;
using System;
using System.IO;

namespace Kinectitude.Editor.Storage.Kgl
{
    internal sealed class KglGameStorage : IGameStorage
    {
        private readonly string FileName;
        private readonly KglGameVisitor Visitor = new KglGameVisitor();

        internal KglGameStorage(string fileName)
        {
            FileName = fileName;
        }

        public Game LoadGame()
        {
            throw new NotImplementedException();
        }

        public void SaveGame(Game game)
        {
            string kgl = Visitor.Apply(game);
            File.WriteAllBytes(FileName, System.Text.Encoding.UTF8.GetBytes(kgl));
        }
    }
}
