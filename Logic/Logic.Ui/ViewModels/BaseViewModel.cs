using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class BaseViewModel : ObservableRecipient
    {
        public BaseViewModel()
        {
            //if (!IsInDesignModeStatic && !IsInDesignMode)
            //{
            //    DispatcherHelper.Initialize();
            //}
        }
    }
}