using System;
using System.Reflection;

namespace PhexensWuerfelraum.Logic.Ui
{
    public class AboutViewModel
    {
        private static Version AssemblyVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }

        public static string Version { get => $"{AssemblyVersion().Major}.{AssemblyVersion().Minor}.{AssemblyVersion().Build}"; }
    }
}