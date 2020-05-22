﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;
using Client.Desktop.ViewModels.Common.Windows;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Windows
{
    public class MasterLinenWindowModel : ViewModelBase, IWindowDialogViewModel
    {
        private readonly ILaundryService _laundryService;
        private readonly IDialogService _dialogService;
        private readonly IMainDispatcher _dispatcher;
        private ObservableCollection<MasterLinenEntity> _masterLinens;
        private MasterLinenEntityViewModel _selectedMasterLinen;

        public Action<bool> CloseAction { get; set; }

        public MasterLinenEntityViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(ref _selectedMasterLinen, value);
        }

        public ObservableCollection<MasterLinenEntity> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public MasterLinenWindowModel(ILaundryService laundryService, IDialogService dialogService, IMainDispatcher dispatcher)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            InitializeCommand = new RelayCommand(Initialize);

        }

        public void SetSelectedLinen(MasterLinenEntityViewModel linen)
        {
            SelectedMasterLinen = null;

            if (linen != null)
            {
                SelectedMasterLinen = linen;
                return;
            }

            SelectedMasterLinen = new MasterLinenEntityViewModel(new MasterLinenEntity()
            {
                PackingValue = 1
            });
        }

        private async void Initialize()
        {
            _dialogService.ShowBusy();

            try
            {
                var linens = await _laundryService.GetAllAsync<MasterLinenEntity>();
                MasterLinens = linens.ToObservableCollection();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedMasterLinen))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void Save()
        {
            if (!SelectedMasterLinen.HasChanges() || !SelectedMasterLinen.IsValid)
            {
                return;
            }
            
            SelectedMasterLinen.AcceptChanges();

            _laundryService.AddOrUpdateAsync(SelectedMasterLinen.OriginalObject);

            if (_dialogService.ShowQuestionDialog("Saved! \n Do you want to close window ? "))
            {
                CloseAction?.Invoke(true);
            }
        }


        private void Close()
        {
            if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"All changes will be canceled\""))
            {
                CloseAction?.Invoke(false);
            }
        }
    }
}
