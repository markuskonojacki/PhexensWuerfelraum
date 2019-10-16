using System;
using System.Reflection;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class AboutViewModel
    {
        private Version AssemblyVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        public string Version { get => $"{AssemblyVersion().Major}.{AssemblyVersion().Minor}.{AssemblyVersion().Build}"; }
    }
}