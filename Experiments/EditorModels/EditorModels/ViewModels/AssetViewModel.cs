using EditorModels.Models;

namespace EditorModels.ViewModels
{
    class AssetViewModel : BaseViewModel
    {
        private readonly Asset asset;
        private Game game;

        public string Name
        {
            get { return asset.Name; }
            set
            {
                if (asset.Name != value)
                {
                    asset.Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string FileName
        {
            get { return asset.FileName; }
            set
            {
                if (asset.FileName != value)
                {
                    asset.FileName = value;
                    NotifyPropertyChanged("FileName");
                }
            }
        }

        public AssetViewModel(string name)
        {
            asset = new Asset();

            Name = name;
        }

        public void SetGame(Game game)
        {
            if (null != this.game)
            {
                this.game.RemoveAsset(asset);
            }

            this.game = game;

            if (null != this.game)
            {
                this.game.AddAsset(asset);
            }
        }
    }
}
