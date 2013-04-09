//-----------------------------------------------------------------------
// <copyright file="Project.cs" company="Kinectitude">
//   Copyright (c) 2013, Kinectitude.
//   This software is released under the Microsoft Reciprocal License (Ms-RL).
//   The license and further copyright text can be found in the file
//   LICENSE at the root directory of this distribution.
// </copyright>
//-----------------------------------------------------------------------

using Kinectitude.Editor.Base;
using Kinectitude.Editor.Views.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.IO;
using Kinectitude.Editor.Storage;
using Kinectitude.Editor.Models.Interfaces;
using Kinectitude.Editor.Models.Notifications;
using System.Diagnostics;
using System;

namespace Kinectitude.Editor.Models
{
    internal sealed class Project : GameModel<IScope>
    {
        private string location;
        private string file;
        private Game game;
        private string gameRoot;
        private string gameFile;
        private BaseModel activeItem;
        private BaseModel inspectorItem;

        public string Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        [DependsOn("Location")]
        public bool LocationIsNewOrEmpty
        {
            get
            {
                if (null != Location)
                {
                    return !Directory.Exists(Location) || !Directory.EnumerateFileSystemEntries(Location).Any();
                }

                return false;
            }
        }

        public string Title
        {
            get { return file; }
            set
            {
                if (file != value)
                {
                    file = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        [DependsOn("Location"), DependsOn("Title")]
        public string FileName
        {
            get 
            {
                if (null != Location && null != Title)
                {
                    var ret = Path.Combine(Location, Title + ".xml");
                    return ret;
                }

                return null;
            }
        }

        public string GameRoot
        {
            get { return gameRoot; }
            set
            {
                if (gameRoot != value)
                {
                    gameRoot = value;
                    NotifyPropertyChanged("Root");
                }
            }
        }

        public Game Game
        {
            get { return game; }
            set
            {
                if (game != value)
                {
                    game = value;
                    game.Scope = this;
                    NotifyPropertyChanged("Game");
                }
            }
        }

        public string GameFile
        {
            get { return gameFile; }
            set
            {
                if (gameFile != value)
                {
                    gameFile = value;
                    NotifyPropertyChanged("GameFile");
                }
            }
        }

        public BaseModel ActiveItem
        {
            get { return activeItem; }
            set
            {
                if (activeItem != value)
                {
                    activeItem = value;
                    NotifyPropertyChanged("ActiveItem");
                }
            }
        }

        public BaseModel InspectorItem
        {
            get { return inspectorItem; }
            set
            {
                if (null == value)
                {
                    value = ActiveItem;
                }

                if (inspectorItem != value)
                {
                    inspectorItem = value;
                    NotifyPropertyChanged("InspectorItem");
                }
            }
        }

        public ObservableCollection<BaseModel> OpenItems { get; private set; }
        public ObservableCollection<Asset> Assets { get; private set; }
        
        public ICommand AddAssetCommand { get; private set; }
        public ICommand RemoveAssetCommand { get; private set; }
        public ICommand OpenItemCommand { get; private set; }
        public ICommand InspectItemCommand { get; private set; }
        public ICommand CloseItemCommand { get; private set; }
        public ICommand BrowseCommand { get; private set; }
        public ICommand CommitCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }

        public Project()
        {
            OpenItems = new ObservableCollection<BaseModel>();
            Assets = new ObservableCollection<Asset>();

            AddAssetCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowLoadDialog((result, file) =>
                {
                    Asset asset = new Asset("An Asset");
                    AddAsset(asset);
                });
            });

            RemoveAssetCommand = new DelegateCommand(null, (parameter) =>
            {
                Asset asset = parameter as Asset;
                RemoveAsset(asset);
            });

            BrowseCommand = new DelegateCommand(null, (parameter) =>
            {
                Workspace.Instance.DialogService.ShowFolderDialog((result, path) =>
                {
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Location = path;
                    }
                });
            });

            CommitCommand = new DelegateCommand((parameter) => LocationIsNewOrEmpty, (parameter) =>
            {
                if (LocationIsNewOrEmpty)
                {
                    ProjectStorage.CreateProject(this);
                    Workspace.Instance.Project = this;
                }
            });

            OpenItemCommand = new DelegateCommand(null, (parameter) => OpenItem(parameter as BaseModel));

            InspectItemCommand = new DelegateCommand(null, (parameter) => InspectorItem = parameter as BaseModel);

            CloseItemCommand = new DelegateCommand(null, (parameter) => CloseItem(parameter as BaseModel));

            PlayCommand = new DelegateCommand(null, p => Play());

            AddHandler<AssetUsed>(OnAssetUsed);
        }

        public override void Accept(IGameVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void OnAssetUsed(AssetUsed e)
        {
            var file = Path.GetFileName(e.PathName);
            if (!HasAssetWithFileName(file))
            {
                var assetsDirectory = new DirectoryInfo(Path.Combine(Location, GameRoot, "Assets"));
                if (!assetsDirectory.Exists)
                {
                    assetsDirectory.Create();
                }

                string destFile = Path.Combine(Location, GameRoot, "Assets", file);
                File.Copy(e.PathName, destFile, true);

                AddAsset(new Asset(file));
            }
        }

        public void AddAsset(Asset asset)
        {
            Assets.Add(asset);

            // Disabling undo since it would mean deleting/undeleting files

            //Workspace.Instance.CommandHistory.Log(
            //    "add asset '" + asset.File + "'",
            //    () => AddAsset(asset),
            //    () => RemoveAsset(asset)
            //);
        }

        public void RemoveAsset(Asset asset)
        {
            if (Assets.Contains(asset))
            {
                Assets.Remove(asset);

                var file = new FileInfo(Path.Combine(Location, GameRoot, "Assets", asset.File));
                if (file.Exists)
                {
                    file.Delete();
                }

                // Disabling undo since it would mean deleting/undeleting files

                //Workspace.Instance.CommandHistory.Log(
                //    "remove asset '" + asset.File + "'",
                //    () => RemoveAsset(asset),
                //    () => AddAsset(asset)
                //);
            }
        }

        public bool HasAssetWithFileName(string fileName)
        {
            return Assets.Any(x => x.File == fileName);
        }

        public void OpenItem(BaseModel item)
        {
            if (!OpenItems.Contains(item))
            {
                OpenItems.Add(item);
            }

            InspectorItem = item;
            ActiveItem = item;
        }

        public void CloseItem(BaseModel item)
        {
            OpenItems.Remove(item);

            if (ActiveItem == item)
            {
                ActiveItem = OpenItems.FirstOrDefault();
            }
        }

        private void Play()
        {
            var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kinectitude");
            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }

            var play = new DirectoryInfo(Path.Combine(appData, string.Format("Play_{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now)));
            play.Create();
            BuildPlayDirectory(play);

            var process = new Process();
            process.StartInfo.WorkingDirectory = play.FullName;
            process.StartInfo.FileName = "Kinectitude.Player.exe";
            process.EnableRaisingEvents = true;
            process.Exited += (o, e) => DeleteTempDirectory(play);
            process.Start();
        }

        private void BuildPlayDirectory(DirectoryInfo play)
        {
            BuildPlayer(play);
            BuildProject(play);
        }

        private void BuildPlayer(DirectoryInfo play)
        {
            var pluginsDir = new DirectoryInfo("Plugins");
            
            var playPlugins = Path.Combine(play.FullName, "Plugins");
            Directory.CreateDirectory(playPlugins);

            foreach (var file in pluginsDir.GetFiles())
            {
                file.CopyTo(Path.Combine(playPlugins, file.Name));
            }

            var required = new[]
            {
                "Kinectitude.Player.exe",
                "Kinectitude.Player.exe.config",
                "SlimDX.dll",
                "Kinectitude.Core.dll",
                "Kinectitude.Core.dll.config",
                "Irony.Interpreter.dll",
                "Irony.dll"
            };

            foreach (var file in required)
            {
                File.Copy(file, Path.Combine(play.FullName, file));
            }
        }

        private void BuildProject(DirectoryInfo play)
        {
            var dataDir = new DirectoryInfo(Path.Combine(Location, GameRoot));
            var dataAssets = new DirectoryInfo(Path.Combine(dataDir.FullName, "Assets"));

            if (dataAssets.Exists)
            {
                var playAssets = Path.Combine(play.FullName, "Assets");
                Directory.CreateDirectory(playAssets);

                foreach (var file in dataAssets.GetFiles())
                {
                    file.CopyTo(Path.Combine(playAssets, file.Name));
                }
            }

            var storage = ProjectStorage.CreateGameStorage(new FileInfo(Path.Combine(play.FullName, "game.kgl")));
            storage.SaveGame(Game);
        }

        private void DeleteTempDirectory(DirectoryInfo info)
        {
            var files = info.GetFiles();
            var dirs = info.GetDirectories();

            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            foreach (var dir in dirs)
            {
                DeleteTempDirectory(dir);
            }

            info.Delete(false);
        }
    }
}
