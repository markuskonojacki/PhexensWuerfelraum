using System;
using System.IO;
using GalaSoft.MvvmLight.Command;
using Jot;
using Jot.Storage;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class SettingsViewModel : BaseViewModel
    {
        #region properties

        public StateTracker Tracker;
        public SettingsModel Setting { get; set; } = new SettingsModel();

        private bool CanFind() => true;

        public bool AllowEdit { get; set; } = true;

        #endregion properties

        #region commands

        public RelayCommand FindCommand { get; set; }

        #endregion commands

        #region contructors

        public SettingsViewModel()
        {
            var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(settingsFilePath);

            Tracker = new StateTracker() { StoreFactory = new JsonFileStoreFactory(settingsFilePath) };

            if (IsInDesignModeStatic)
            {
                Setting = new SettingsModel()
                {
                    ServerAddress = "127.0.0.1",
                    ServerPort = "1212",
                    SoundEffectsEnabled = true,
                    AdditionalTrials = false,
                    GameMasterMode = true,
                    HeldenDateiPath = "C:\\temp\\.helden.zip.hld"
                };
            }
            else
            {
                Tracker.Configure(Setting).Apply();
            }

            FindCommand = new RelayCommand(() => Find(), CanFind);
        }

        #endregion contructors

        #region methods

        /// <summary>
        /// open windows file pick dialog to pick helden.zip.hld
        /// </summary>
        private void Find()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".hld",
                Filter = "Helden-Software Charakterdatenbank (helden.zip.hld)|*.hld",
                CheckFileExists = true,
                Multiselect = false,
                Title = "Helden-Software Charakterdatenbank auswählen",
            };

            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\helden";
            if (Directory.Exists(path))
            {
                dlg.InitialDirectory = path;
            }

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                Setting.HeldenDateiPath = dlg.FileName;
            }
        }

        #endregion methods
    }
}