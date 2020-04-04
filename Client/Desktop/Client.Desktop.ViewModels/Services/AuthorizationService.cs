﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Services
{
    public class AuthorizationService : ViewModelBase, IAuthorizationService
    {
        private CustomPrincipal _princimal;

        public CustomPrincipal CurrentPrincipal
        {
            get => _princimal;
            set => Set(() => CurrentPrincipal, ref _princimal, value);
        }

        public EventHandler CurrentPrincipalChanged { get; set; }
        public Task LogoutAsync()
        {
            var anonymousIdentity = new AnonymousIdentity();

            if (Thread.CurrentPrincipal is CustomPrincipal customPrincipal)
            {
                customPrincipal.Identity = anonymousIdentity;
            }

            CurrentPrincipal.Identity = anonymousIdentity;

            return Task.CompletedTask;
        }

        public AuthorizationService()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CurrentPrincipal))
                return;

            CurrentPrincipalChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}