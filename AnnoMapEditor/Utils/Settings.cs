﻿using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace AnnoMapEditor.Utils
{
    public class Settings : INotifyPropertyChanged
    {
        public static Settings Instance { get; } = new();

        public IDataArchive DataArchive
        {
            get => _dataArchive;
            private set
            {
                if (_dataArchive is System.IDisposable disposable)
                    disposable.Dispose();
                SetProperty(ref _dataArchive, value);
            }
        }
        private IDataArchive _dataArchive = Utils.DataArchive.Open(null);

        public string? DataPath 
        {
            get => _dataArchive.Path;
            set
            {
                if (value != DataPath)
                {
                    DataArchive = Utils.DataArchive.Open(value);
                    UserSettings.Default.DataPath = DataArchive.Path;
                    UserSettings.Default.Save();
                    OnPropertyChanged(nameof(DataPath));
                    OnPropertyChanged(nameof(IsValidDataPath));
                }
            }
        }

        public bool IsValidDataPath => _dataArchive?.IsValid ?? false;

        public Settings()
        {
            DataPath = UserSettings.Default.DataPath;
            if (DataArchive?.IsValid != true)
            {
                // auto detect on start-up if not valid
                DataPath = GetInstallDirFromRegistry();
            }
        }

        private static string? GetInstallDirFromRegistry()
        {
            string installDirKey = @"SOFTWARE\WOW6432Node\Ubisoft\Anno 1800";
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(installDirKey);
            return key?.GetValue("InstallDir") as string;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
