namespace PhexensWuerfelraum.Logic.Ui
{
    /// <summary>
    /// If sent through the Messenger this message tells that a view model wants to
    /// open the child window.
    /// </summary>
    public class OpenHeroPickDialogMessage
    {
        #region properties

        /// <summary>
        /// Just some text that comes from the sender.
        /// </summary>
        public string SomeText { get; private set; }

        #endregion properties

        #region constructors and destructors

        public OpenHeroPickDialogMessage()
        {
        }

        public OpenHeroPickDialogMessage(string someText)
        {
            SomeText = someText;
        }

        #endregion constructors and destructors
    }
}