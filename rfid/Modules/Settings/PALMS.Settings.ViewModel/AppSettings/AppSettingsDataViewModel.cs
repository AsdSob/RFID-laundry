using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using PALMS.ViewModels.Common;

namespace PALMS.Settings.ViewModel.AppSettings
{
    public class AppSettingsDataViewModel : ViewModelBase, IDataErrorInfo
    {
        private string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            set => Set(ref _connectionString, value);
        }

        public string this[string columnName] => Validate(columnName);

        public string Error { get; set; }

        private string Validate(string columnName)
        {
            string error;

            if (columnName == nameof(ConnectionString))
            {
                if (!ConnectionString.ValidateRequired(out error))
                {
                    return error;
                }
            }

            return null;
        }

        internal bool Validate()
        {
            return string.IsNullOrEmpty(Validate(nameof(ConnectionString)));
        }
    }
}