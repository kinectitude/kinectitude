using EditorModels.Models;

namespace EditorModels.ViewModels
{
    internal sealed class DefineViewModel : BaseViewModel
    {
        private Using use;
        private readonly Define define;

        public string Name
        {
            get { return define.Name; }
            set
            {
                if (define.Name != value)
                {
                    define.Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Class
        {
            get { return define.Class; }
            set
            {
                if (define.Class != value)
                {
                    define.Class = value;
                    NotifyPropertyChanged("Class");
                }
            }
        }

        public DefineViewModel(string name, string className)
        {
            define = new Define();
            define.Name = name;
            define.Class = className;
        }

        public void SetUsing(Using use)
        {
            if (null != this.use)
            {
                this.use.RemoveDefine(define);
            }

            this.use = use;

            if (null != this.use)
            {
                this.use.AddDefine(define);
            }
        }
    }
}
