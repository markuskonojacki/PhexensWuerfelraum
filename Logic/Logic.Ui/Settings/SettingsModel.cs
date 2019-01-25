using Jot;
using Jot.DefaultInitializer;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class SettingsModel : BaseModel, ITrackingAware
    {
        #region properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "Server Address darf nicht leer sein.")]
        [Trackable]
        public string ServerAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Server Port darf nicht leer sein")]
        [Trackable]
        public string ServerPort { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Heldendatei-Pfad darf nicht leer sein")]
        [Trackable]
        public string HeldenDateiPath { get; set; }

        [Trackable]
        public string WikiUrl { get; set; }

        [Trackable]
        public string WhiteboardUrl { get; set; }

        [Trackable]
        public string StaticUserName { get; set; }

        [Trackable]
        public bool SoundEffectsEnabled { get; set; } = true;

        #region AutoUpdate

        public bool AutoUpdate { get => GetAutoUpdate(); set => SetAutoUpdate(value); }

        private bool GetAutoUpdate()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(filePath);

            return !File.Exists(Path.Combine(filePath, ".disableautoupdate"));
        }

        private void SetAutoUpdate(bool value)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhexensWuerfelraum");
            Directory.CreateDirectory(filePath);

            if (value == true)
            {
                File.Delete(Path.Combine(filePath, ".disableautoupdate"));
            }
            else
            {
                File.Create(Path.Combine(filePath, ".disableautoupdate"));
            }
        }

        #endregion AutoUpdate

        [Trackable]
        public bool GameMasterMode { get; set; }

        [Trackable]
        public bool AdditionalTrials { get; set; }

        public void InitConfiguration(TrackingConfiguration configuration)
        {
            configuration
            .IdentifyAs("Settings")
            .RegisterPersistTrigger(nameof(PropertyChanged));
        }

        #endregion properties

        #region methods

        protected override void InitCommands()
        {
            base.InitCommands();
        }

        protected override void OnErrorsCollected()
        {
            base.OnErrorsCollected();
        }

        #endregion methods
    }
}