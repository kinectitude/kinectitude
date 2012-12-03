using Kinectitude.Editor.Base;
using Kinectitude.Editor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Kinectitude.Editor.Models
{
    internal sealed class Project : BaseModel
    {
        private string file;
        private Game game;
        private string gameRoot;
        private string gameFile;

        public string File
        {
            get { return file; }
            set
            {
                if (file != value)
                {
                    file = value;
                    NotifyPropertyChanged("File");
                }
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

        public ObservableCollection<Asset> Assets { get; private set; }
        
        public ICommand AddAssetCommand { get; private set; }
        public ICommand RemoveAssetCommand { get; private set; }

        public Project()
        {
            Assets = new ObservableCollection<Asset>();

            AddAssetCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowLoadDialog((result, file) =>
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

        }

        public void AddAsset(Asset asset)
        {
            Assets.Add(asset);

            Workspace.Instance.CommandHistory.Log(
                "add asset '" + asset.File + "'",
                () => AddAsset(asset),
                () => RemoveAsset(asset)
            );
        }

        public void RemoveAsset(Asset asset)
        {
            Assets.Remove(asset);

            Workspace.Instance.CommandHistory.Log(
                "remove asset '" + asset.File + "'",
                () => RemoveAsset(asset),
                () => AddAsset(asset)
            );
        }
    }
}
