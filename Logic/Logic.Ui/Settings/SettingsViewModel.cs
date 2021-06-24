using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using IWshRuntimeLibrary;
using Jot;
using Jot.Storage;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class SettingsViewModel : BaseViewModel
    {
        #region properties

        public Tracker Tracker;
        public SettingsModel Setting { get; set; } = new();

        public string DataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum", "Data", Assembly.GetEntryAssembly().GetName().Version.Major.ToString());
            }
        }

        private bool CanFind() => true;

        public bool AllowEdit { get; set; } = true;

        public bool IsInstalled => currentExePath == targetExePath && System.IO.File.Exists(targetExePath);

        private readonly string startMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
        private readonly string targetDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum", "Client");
        private readonly string targetExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum", "Client", "PhexensWuerfelraum.exe");
        private readonly string currentExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PhexensWuerfelraum.exe");

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

        public void Install()
        {
            if (IsInstalled == false)
            {
                if (!Directory.Exists(targetDirPath))
                    Directory.CreateDirectory(targetDirPath);

                if (currentExePath != targetExePath)
                    System.IO.File.Move(currentExePath, targetExePath, true);

                // create desktop lnk
                var desktopShortcut = new WshShell().CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Phexens Würfelraum.lnk")) as IWshShortcut;
                desktopShortcut.TargetPath = Path.Combine(targetDirPath, "PhexensWuerfelraum.exe");
                desktopShortcut.WorkingDirectory = targetDirPath;
                desktopShortcut.Save();

                // create start menu lnk
                var startmenuShortcut = new WshShell().CreateShortcut(Path.Combine(startMenuPath, "Phexens Würfelraum.lnk")) as IWshShortcut;
                startmenuShortcut.TargetPath = Path.Combine(targetDirPath, "PhexensWuerfelraum.exe");
                startmenuShortcut.WorkingDirectory = targetDirPath;
                startmenuShortcut.Save();

                Process.Start(targetExePath);
                Environment.Exit(0);
            }
        }

        public void Uninstall()
        {
            System.IO.File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Phexens Würfelraum.lnk"));
            System.IO.File.Delete(Path.Combine(startMenuPath, "Phexens Würfelraum.lnk"));

            Process.Start(new ProcessStartInfo()
            {
                Arguments = $"/C choice /C J /N /D J /T 5 & rmdir /S /Q \"{ Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum") }\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });

            Environment.Exit(0);
        }

        #endregion methods
    }
}