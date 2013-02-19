using Kinectitude.Editor.Base;
using Kinectitude.Editor.Views.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.IO;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Editor.Models
{
    internal sealed class Project : BaseModel
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

        public Project()
        {
            OpenItems = new ObservableCollection<BaseModel>();
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

            BrowseCommand = new DelegateCommand(null, (parameter) =>
            {
                DialogService.ShowFolderDialog((result, path) =>
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

    }
}
