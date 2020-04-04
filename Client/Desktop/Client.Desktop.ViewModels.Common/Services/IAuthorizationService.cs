﻿using System;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Identity;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IAuthorizationService
    {
        CustomPrincipal CurrentPrincipal { get; set; }
        EventHandler CurrentPrincipalChanged { get; set; }
        Task LogoutAsync();
    }
}