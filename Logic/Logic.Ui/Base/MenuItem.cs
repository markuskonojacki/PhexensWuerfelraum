using System;
using System.Windows.Input;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class MenuItem : BindableBase
    {
        private DelegateCommand _command;
        private object _icon;
        private bool _isEnabled = true;
        private Uri _navigationDestination;
        private string _text;

        public ICommand Command
        {
            get { return this._command; }
            set { this.SetProperty(ref this._command, (DelegateCommand)value); }
        }

        public object Icon
        {
            get { return this._icon; }
            set { this.SetProperty(ref this._icon, value); }
        }

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { this.SetProperty(ref this._isEnabled, value); }
        }

        public bool IsNavigation => this._navigationDestination != null;

        public Uri NavigationDestination
        {
            get { return this._navigationDestination; }
            set { this.SetProperty(ref this._navigationDestination, value); }
        }

        public string Text
        {
            get { return this._text; }
            set { this.SetProperty(ref this._text, value); }
        }
    }
}