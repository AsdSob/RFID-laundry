﻿using System;
using System.Collections.ObjectModel;
using Client.Desktop.ViewModels.Common.EntityViewModels;
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
        private ObservableCollection<MasterLinenEntityViewModel> _masterLinens;
        private MasterLinenEntityViewModel _selectedMasterLinen;

        public Action<bool> CloseAction { get; set; }

        public MasterLinenEntityViewModel SelectedMasterLinen
        {
            get => _selectedMasterLinen;
            set => Set(ref _selectedMasterLinen, value);
        }

        public ObservableCollection<MasterLinenEntityViewModel> MasterLinens
        {
            get => _masterLinens;
            set => Set(ref _masterLinens, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CloseCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand InitializeCommand { get; }


        public MasterLinenWindowModel(ILaundryService laundryService, IDialogService dialogService)
        {
            _laundryService = laundryService ?? throw new ArgumentNullException(nameof(laundryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
            DeleteCommand = new RelayCommand(Delete);
            //InitializeCommand = new RelayCommand(Initialize);
        }

        public void SetItem(MasterLinenEntityViewModel linen)
        {
            Initialize();
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
                MasterLinens = await _laundryService.MasterLinens();

            }
            catch (Exception e)
            {
                _dialogService.HideBusy();
            }
            finally
            {
                _dialogService.HideBusy();
            }
        }

        private void Save()
        {
            if (!SelectedMasterLinen.HasChanges() || !SelectedMasterLinen.IsValid) return;
            
            SelectedMasterLinen.AcceptChanges();
            _laundryService.AddOrUpdateAsync(SelectedMasterLinen.OriginalObject);
            Close();
        }

        private void Delete()
        {
            if (!_dialogService.ShowQuestionDialog($"Do you want to DELETE {SelectedMasterLinen.Name} ?"))  return;

            if (!SelectedMasterLinen.IsNew)
            {
                _laundryService.DeleteAsync(SelectedMasterLinen.OriginalObject);
            }

            Close();
        }

        private void Close()
        {
            if (SelectedMasterLinen.HasChanges())
            {
                if (_dialogService.ShowQuestionDialog($"Do you want to close window ? \n \"Changes is NOT saved\""))
                {
                    CloseAction?.Invoke(false);
                    return;
                }
            }

            CloseAction?.Invoke(true);
        }
    }
}
