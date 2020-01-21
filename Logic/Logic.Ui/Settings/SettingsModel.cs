using Jot.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class SettingsModel : BaseModel, ITrackingAware<SettingsModel>
    {
        #region properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "Server Address darf nicht leer sein.")]
        public string ServerAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Server Port darf nicht leer sein")]
        public string ServerPort { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Heldendatei-Pfad darf nicht leer sein")]
        public string HeldenDateiPath { get; set; }

        public string WikiUrl { get; set; }

        public bool EnableSSL { get; set; } = true;
        public string PublicKey { get; set; }

        public string WhiteboardUrl { get; set; }

        public string StaticUserName { get; set; }

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

        public bool GameMasterMode { get; set; }

        public bool AdditionalTrials { get; set; }

        public void ConfigureTracking(TrackingConfiguration<SettingsModel> configuration)
        {
            configuration
                .Id(s => "Settings")
                .Properties(s => new
                {
                    s.ServerAddress,
                    s.ServerPort,
                    s.HeldenDateiPath,
                    s.WikiUrl,
                    s.WhiteboardUrl,
                    s.StaticUserName,
                    s.SoundEffectsEnabled,
                    s.GameMasterMode,
                    s.AdditionalTrials,
                    s.EnableSSL,
                    s.PublicKey
                })
                .PersistOn(nameof(PropertyChanged));
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