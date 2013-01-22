using Kinectitude.Editor.Models;
using Kinectitude.Editor.Storage.Kgl;
using System.IO;
using System.Xml.Linq;

namespace Kinectitude.Editor.Storage
{
    internal static class ProjectStorage
    {
        private static class Constants
        {
            public static readonly XName Project = "Project";
            public static readonly XName GameRoot = "GameRoot";
            public static readonly XName GameFile = "GameFile";
            public static readonly XName Asset = "Asset";
            public static readonly XName File = "File";
        }

        public static IGameStorage CreateGameStorage(FileInfo file)
        {
            IGameStorage storage = null;

            if (file.Extension == ".kgl")
            {
                storage = new KglGameStorage(file);
                //Workspace.ValueMaker = new KGLValueMaker();
            }

            return storage;
        }

        public static void SaveProject(Project project)
        {
            FileInfo projectFile = new FileInfo(project.File);

            DirectoryInfo gameRoot = new DirectoryInfo(Path.Combine(projectFile.DirectoryName, project.GameRoot));
            if (!gameRoot.Exists)
            {
                gameRoot.Create();
            }

            if (string.IsNullOrEmpty(project.GameFile))
            {
                project.GameFile = "game.kgl";  // TODO: This is a temporary fix until we require GameFile to always exist
            }

            IGameStorage storage = CreateGameStorage(new FileInfo(Path.Combine(gameRoot.ToString(), project.GameFile)));
            storage.SaveGame(project.Game);

            XElement projectElement = new XElement(
                Constants.Project,
                new XAttribute(Constants.GameRoot, project.GameRoot),
                new XAttribute(Constants.GameFile, project.GameFile)
                );

            foreach (Asset asset in project.Assets)
            {
                projectElement.Add(new XElement(Constants.Asset, new XAttribute(Constants.File, asset.File)));
            }

            projectElement.Save(projectFile.ToString());
        }

        public static Project LoadProject(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            Project project = null;

            if (file.Exists)
            {
                // TODO: Display error if !Exists

                using (Stream stream = file.OpenRead())
                {
                    XElement projectElement = XElement.Load(stream);

                    project = new Project()
                    {
                        File = file.ToString(),
                        GameRoot = (string)projectElement.Attribute(Constants.GameRoot),
                        GameFile = (string)projectElement.Attribute(Constants.GameFile)
                    };

                    foreach (XElement assetElement in projectElement.Elements(Constants.Asset))
                    {
                        Asset asset = LoadAsset(assetElement);
                        project.AddAsset(asset);
                    }

                    FileInfo gameFile = new FileInfo(Path.Combine(Path.GetDirectoryName(project.File), project.GameRoot, project.GameFile));

                    if (gameFile.Exists)
                    {
                        // TODO: Display error if !Exists

                        IGameStorage storage = CreateGameStorage(gameFile);
                        if (null != storage)
                        {
                            project.Game = storage.LoadGame();
                        }
                    }
                }
            }

            return project;
        }

        private static Asset LoadAsset(XElement element)
        {
            return new Asset((string)element.Attribute(Constants.File));
        }
    }
}
