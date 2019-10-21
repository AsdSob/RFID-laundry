using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel.Dictionaries.Base
{
    public abstract class DictionaryBaseViewModel : ViewModelBase, IInitializationAsync
    {
        private bool _isEnumDictionary;

        public bool IsEnumDictionary
        {
            get => _isEnumDictionary;
            set => Set(ref _isEnumDictionary, value);
        }

        public abstract string Name { get; }

        public RelayCommand AddCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public abstract Task InitializeAsync();

        protected DictionaryBaseViewModel()
        {
            AddCommand = new RelayCommand(Add, CanAdded);
            SaveCommand = new RelayCommand(Save, CanSave);
            RemoveCommand = new RelayCommand(Remove, CanRemove);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        protected abstract void Add();
        protected abstract void Save();
        protected abstract void Remove();
        protected abstract void Cancel();

        protected virtual bool CanAdded() => true;
        protected virtual bool CanRemove() => true;
        protected virtual bool CanSave() => true;
        protected virtual bool CanCancel() => true;
    
    }
}