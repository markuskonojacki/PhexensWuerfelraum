using GalaSoft.MvvmLight.CommandWpf;
using Jot;
using Jot.DefaultInitializer;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

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

        [Trackable]
        public bool GameMasterMode { get; set; }

        [Trackable]
        public bool AdditionalTrials { get; set; }

        public RelayCommand OkCommand { get; private set; }

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
            OkCommand = new RelayCommand(
                () =>
                {
                    Trace.WriteLine("OK");
                },
                () => IsOk);
        }

        protected override void OnErrorsCollected()
        {
            base.OnErrorsCollected();
            OkCommand.RaiseCanExecuteChanged();
        }

        #endregion methods
    }
}