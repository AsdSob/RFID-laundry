using System;
using System.Windows.Input;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Content;
using Client.Desktop.ViewModels.Content.Master;

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
        public ICommand ExitCommand { get; }

        public ICommand ClientCommand { get; }
        public ICommand StaffCommand { get; }

        public MenuViewModel()
        {
            NewCommand = new RelayCommand(() => Select(typeof(DataViewModel)));
            ExitCommand = new RelayCommand(() => Select(typeof(ExitViewModel)));
            ClientCommand = new RelayCommand(() => Select(typeof(ClientViewModel)));
            StaffCommand = new RelayCommand(() => Select(typeof(StaffViewModel)));

        }

        private void Select(Type type)
        {
            // TODO: use enum?
            SelectedItem = type;
        }
    }
}