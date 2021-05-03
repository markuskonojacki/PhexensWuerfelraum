using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Jot;
using Jot.Storage;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class SettingsViewModel : BaseViewModel
    {
        #region properties

        public Tracker Tracker;
        public SettingsModel Setting { get; set; } = new SettingsModel();

        public string DataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum", "Data", Assembly.GetEntryAssembly().GetName().Version.Major.ToString());
            }
        }

        private bool CanFind() => true;

        public bool AllowEdit { get; set; } = true;

        #endregion properties

        #region commands

        public RelayCommand FindCommand { get; set; }
        public RelayCommand RenewUserIdentifierCommand { get; set; }

        #endregion commands

        #region contructors

        public SettingsViewModel()
        {
            Directory.CreateDirectory(DataPath);

            Tracker = new Tracker(new JsonFileStore(DataPath));

            if (IsInDesignModeStatic)
            {
                Setting = new SettingsModel()
                {
                    ServerAddress = "127.0.0.1",
                    ServerPort = "12113",
                    SoundEffectsEnabled = true,
                    AdditionalTrials = false,
                    GameMasterMode = true,
                    HeldenDateiPath = "C:\\temp\\.helden.zip.hld"
                };
            }
            else
            {
                Tracker.Configure<SettingsModel>();
                Tracker.Track(Setting);
            }

            FindCommand = new RelayCommand(() => Find(), CanFind);
            RenewUserIdentifierCommand = new RelayCommand(() => RenewUserIdentifier(), CanFind);

            if (Setting.UserIdentifier == Guid.Empty)
                Setting.UserIdentifier = Guid.NewGuid();

            Setting.PropertyChanged += Setting_PropertyChanged;
        }

        private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StaticUserName":
                    SimpleIoc.Default.GetInstance<CharacterViewModel>().CharacterList.First(c => c.Id == "StaticCharacter").Name = Setting.StaticUserName;
                    break;

                case "HeldenDateiPath":
                    SimpleIoc.Default.GetInstance<CharacterViewModel>().LoadCharacterList();
                    break;
            }
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

        public void RenewUserIdentifier()
        {
            Setting.UserIdentifier = Guid.NewGuid();
        }

        #endregion methods
    }
}