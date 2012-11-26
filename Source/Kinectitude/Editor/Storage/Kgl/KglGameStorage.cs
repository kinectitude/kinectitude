using Kinectitude.Editor.Models;
using Kinectitude.Editor.Storage.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
            // Check if the project file exists

            string projectFolder = Path.GetDirectoryName(FileName);
            string assetFolder = Path.GetFileNameWithoutExtension(FileName);
            string gameFile = Path.Combine(projectFolder, assetFolder, "game.kgl");

            if (!File.Exists(FileName))
            {
                XElement project = new XElement
                (
                    XmlConstants.Project,
                    new XElement(XmlConstants.Root, assetFolder)
                );
                project.Save(FileName);
            }

            FileInfo file = new FileInfo(gameFile);
            file.Directory.Create();

            // Check if the project folder exists

            string kgl = Visitor.Apply(game);
            File.WriteAllBytes(FileName, System.Text.Encoding.UTF8.GetBytes(kgl));
        }
    }
}
