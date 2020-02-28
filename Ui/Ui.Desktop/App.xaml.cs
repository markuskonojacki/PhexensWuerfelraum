using PhexensWuerfelraum.Ui.Desktop.Views;

//using Squirrel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace PhexensWuerfelraum.Ui.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string UpdateUrl = @"https://wuerfelraum.3d20.de/Download/";

        protected override void OnStartup(StartupEventArgs e)
        {
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            base.OnStartup(e);

#if !DEBUG
            if (new Logic.Ui.SettingsModel().AutoUpdate)
            {
                InitSquirrel();
                Update();
            }
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "if !DEBUG")]
        private void InitSquirrel()
        {
            //using (var mgr = new UpdateManager(UpdateUrl))
            //{
            //    SquirrelAwareApp.HandleEvents(
            //        onInitialInstall: v => mgr.CreateShortcutForThisExe(),
            //        onAppUpdate: v => mgr.CreateShortcutForThisExe(),
            //        onAppUninstall: v => mgr.RemoveShortcutForThisExe());
            //}
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "if !DEBUG")]
        private void Update()
        {
            Task<string> task = Task.Run(() => CheckForUpdates());

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string localVersion = fvi.FileVersion;

            var versionRemote = new Version(task.Result);
            var versionLocal = new Version(localVersion);
            var t = versionRemote.CompareTo(versionLocal);
            if (t > 0)
            {
                MySplashScreen splashScreen = new MySplashScreen();
                splashScreen.Show();

                //Remote version is greater
                Task<int> task2 = Task.Run(() => ProceedUpdate(versionRemote.ToString()));
                int up = task2.Result;

                splashScreen.Close();
            }
        }

        //Check if there's an update available, then return the version
        private async Task<string> CheckForUpdates()
        {
            //Check for updates
            //using (var mgr = new UpdateManager(UpdateUrl))
            //{
            //    var updateInfo = await mgr.CheckForUpdate(progress: x => Trace.WriteLine(x / 3));
            //    string futureVersion = updateInfo.FutureReleaseEntry.Version.ToString();
            //    return futureVersion;
            //}
            return "";
        }

        //Proceed with the update, then restart the app
        private async Task<int> ProceedUpdate(string version)
        {
            //using (var mgr = new UpdateManager(UpdateUrl))
            //{
            //    await mgr.UpdateApp();
            //    UpdateManager.RestartApp();
            //}
            return 0;
        }
    }

    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            //Debugger.Break();
        }
    }
}