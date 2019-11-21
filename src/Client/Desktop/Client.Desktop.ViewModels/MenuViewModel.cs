using System;
using System.Windows.Input;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;

namespace Client.Desktop.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private Type _selectedItem;

        public Type SelectedItem
        {
            get => _selectedItem;
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        public ICommand NewCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ExitCommand { get; }

        public MenuViewModel()
        {
            NewCommand = new RelayCommand(() => Select(typeof(DataViewModel)));
            AddCommand = new RelayCommand(() => Select(typeof(AddViewModel)));
            ExitCommand = new RelayCommand(() => Select(typeof(ExitViewModel)));
        }

        private void Select(Type type)
        {
            // TODO: use enum?
            SelectedItem = type;
        }
    }
}