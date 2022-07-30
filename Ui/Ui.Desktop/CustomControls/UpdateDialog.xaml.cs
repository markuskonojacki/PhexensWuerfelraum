using System.Diagnostics.Contracts;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PhexensWuerfelraum.Ui.Desktop.CustomControls
{
    /// <summary>
    /// Interaction logic for AccuracySelectionDialog.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class UpdateDialog : BaseMetroDialog
    {
        private readonly MetroWindow window;
        private readonly MetroDialogSettings dialogSettings;

        public UpdateDialog(MetroWindow window, MetroDialogSettings dialogSettings)
        {
            Contract.Requires(null != window);

            this.window = window;
            this.dialogSettings = dialogSettings;

            InitializeComponent();
        }
    }
}
