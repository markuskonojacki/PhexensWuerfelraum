using Microsoft.Extensions.Configuration;
using System;

namespace PhexensWuerfelraum.Server.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleViewModel consoleViewModel = new ConsoleViewModel();

            var config = new ConfigurationBuilder()
                .AddIniFile("settings.ini", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();

            consoleViewModel.Port = config["port"];

            consoleViewModel.PropertyChanged += (s, e) =>
            {
                ConsoleViewModel.WriteOutput($"{e.PropertyName} => { s.GetType().GetProperty(e.PropertyName).GetValue(s).ToString() }");
            };

            consoleViewModel.RunCommand.Execute(null);
            System.Console.ReadKey();
        }
    }
}