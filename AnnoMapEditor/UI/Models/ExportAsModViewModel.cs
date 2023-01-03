﻿using AnnoMapEditor.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnnoMapEditor.UI.Models
{
    public class ExportAsModViewModel : ObservableBase
    {
        enum ModStatus
        {
            NotFound,
            Active,
            Inactive
        }

        public MapTemplates.Session? Session
        { 
            get => _session;
            set
            {
                _session = value;
                if(_session is not null)
                {
                    AllowedMapTypes = Mods.MapType.MapTypesForRegion[_session.Region];
                    SelectedMapType = AllowedMapTypes.First();
                }
                else
                {
                    AllowedMapTypes = Enumerable.Empty<Mods.MapType>();
                    SelectedMapType = null;
                }
                InfoMapTypeSelection = _session is not null && _session.Region == MapTemplates.Region.Moderate;
                CheckExistingMod();
            }
        }
        MapTemplates.Session? _session;

        public string ModName
        {
            get => _modName;
            set
            {
                SetProperty(ref _modName, value);
                CheckExistingMod();
            }
        }
        private string _modName = string.Empty;

        public string ResultingModName { get; private set; } = string.Empty;
        public string ResultingFullModName { get; private set; } = string.Empty;

        public bool CanExport => true;
        public string ModExistsWarning { get; private set; } = string.Empty;

        public bool InfoMapTypeSelection
        {
            get => _infoMapTypeSelection;
            set => SetProperty(ref _infoMapTypeSelection, value);
        }
        private bool _infoMapTypeSelection = true;

        public string ModID
        {
            get => _modID;
            set => SetProperty(ref _modID, value);
        }
        private string _modID = string.Empty;

        public Mods.MapType? SelectedMapType
        {
            get => _mapType;
            set
            {
                SetProperty(ref _mapType, value);
                CheckExistingMod();
            }
        }
        private Mods.MapType? _mapType = null;

        public IEnumerable<Mods.MapType> AllowedMapTypes
        {
            get => _allowedMapTypes;
            set => SetProperty(ref _allowedMapTypes, value);
        }
        private IEnumerable<Mods.MapType> _allowedMapTypes = Enumerable.Empty<Mods.MapType>();

        public ExportAsModViewModel()
        {
            Settings.Instance.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.DataPath))
                CheckExistingMod();
        }

        private void CheckExistingMod()
        {
            ResultingModName = (_modName.Trim() == string.Empty ? $"Custom {_mapType}" : _modName);
            string modName = "[Map] " + ResultingModName;
            ModStatus status = ModExists(modName);
            if (status == ModStatus.Inactive)
                modName = "-" + modName;
            ResultingFullModName = modName;
            ModExistsWarning = status != ModStatus.NotFound ? $"Replace existing \"{ResultingFullModName}\"" : "";
            OnPropertyChanged(nameof(ResultingModName));
            OnPropertyChanged(nameof(ModExistsWarning));
        }

        private static ModStatus ModExists(string modName)
        {
            if (Settings.Instance.DataPath is null)
                return ModStatus.NotFound;

            string activeModPath = Path.Combine(Settings.Instance.DataPath, "mods", modName);
            if (Directory.Exists(activeModPath))
                return ModStatus.Active;

            string inactiveModPath = Path.Combine(Settings.Instance.DataPath, "mods", "-" + modName);
            return Directory.Exists(inactiveModPath) ? ModStatus.Inactive : ModStatus.NotFound;
        }

        public async Task<bool> Save()
        {
            string? modsFolderPath = Settings.Instance.DataPath;
            if (modsFolderPath is not null)
                modsFolderPath = Path.Combine(modsFolderPath, "mods");

            if (!Directory.Exists(modsFolderPath) || Session is null)
            {
                Log.Warn("mods/ path or session not set. This shouldn't have happened.");
                return false;
            }

            Mods.Mod mod = new(Session)
            {
                MapType = SelectedMapType
            };

            CheckExistingMod();
            return await mod.Save(Path.Combine(modsFolderPath, ResultingFullModName), ResultingModName, ModID);
        }
    }
}
