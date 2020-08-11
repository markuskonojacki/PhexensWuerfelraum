using System;
using System.Linq;
using WixSharp;
using sys = System.Reflection;

public class Script
{
    private static void Main()
    {
        Compiler.AutoGeneration.IgnoreWildCardEmptyDirectories = true;

        var project = new ManagedProject("Phexens Würfelraum", new Dir(new Id("INSTALLDIR"), @"%LocalAppDataFolder%\PhexensWuerfelraum\Client", new Files(@"C:\VSDistribution\PhexensWuerfelraum.Ui.Desktop.Publish\*.*")));

        project.ResolveWildCards()
            .FindFile((f) => f.Name.EndsWith("PhexensWuerfelraum.exe"))
            .First()
            .Shortcuts = new[]
            {
                new FileShortcut("Phexens Würfelraum", @"%ProgramMenu%"),
                new FileShortcut("Phexens Würfelraum", "%Desktop%")
            };

        project.Version = new Version("3.0.0.0");
        project.GUID = new Guid("2f7e47da-3312-466f-8807-a4a107cd7427");

        project.EmbeddedUI = new EmbeddedAssembly(sys.Assembly.GetExecutingAssembly().Location);

        project.OutDir = @"C:\VSDistribution\PhexensWuerfelraum.Ui.Installer";
        project.OutFileName = "setup";

        project.Language = "de-DE";

        project.BuildMsi();
    }
}