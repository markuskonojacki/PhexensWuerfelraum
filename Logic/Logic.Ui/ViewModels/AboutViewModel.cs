using System;
using System.Reflection;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class AboutViewModel
    {
        public string Version { get => Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString(); }
    }
}