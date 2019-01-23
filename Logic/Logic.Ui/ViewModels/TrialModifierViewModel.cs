using GalaSoft.MvvmLight.CommandWpf;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class TrialModifierViewModel : BaseViewModel
    {
        #region properties

        private bool _canExecute;
        public bool IsChildWindowOpenOrNotProperty { get; set; } = true;
        public int TrialModifer { get; set; } = 0;

        #endregion properties

        #region commands

        public RelayCommand CancelCommand;
        public RelayCommand OkCommand;

        #endregion commands

        public TrialModifierViewModel()
        {
            _canExecute = true;

            OkCommand = new RelayCommand(() => CloseOk(), _canExecute);
            CancelCommand = new RelayCommand(() => CloseCancel(), _canExecute);
        }

        private void CloseCancel()
        {
        }

        private void CloseOk()
        {
        }
    }
}