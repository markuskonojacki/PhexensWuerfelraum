namespace PhexensWuerfelraum.Logic.Ui
{
    public class OpenInfoMessage
    {
        #region properties

        /// <summary>
        /// Text that will be displayed by a dialog
        /// </summary>
        public string InfoTitle { get; private set; }
        public string InfoText { get; private set; }

        #endregion properties

        #region constructors and destructors

        public OpenInfoMessage(string infoTitle, string infoText)
        {
            InfoTitle = infoTitle;
            InfoText = infoText;
        }

        #endregion constructors and destructors
    }
}