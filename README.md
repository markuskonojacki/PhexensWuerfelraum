[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/Derevar/PhexensWuerfelraum/blob/master/LICENSE)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://github.com/Derevar/PhexensWuerfelraum/graphs/contributors)

| [DE](#phexens-würfelraum)
|:---|
| [EN](#phexs-dice-room)

![](Ui/Ui.Desktop/Resources/AppIcon.ico)

| DE |
|:---|

# Phexens Würfelraum
Zuvorderst: Phexens Würfelraum ist ein komplett __NICHT__ offizielles Projekt meinerseits. Es besteht keinerlei offizielle Verbindung zu Fanpro, Ulisses Spiele, der Significant Fantasy Medienrechte GbR oder ihren Produkten.

Nun zum Tool selbst: Phexens Würfelraum ist ein Programm um es Menschen zu ermöglichen, gemeinsam [Das Schwarze Auge](http://www.ulisses-spiele.de/sortiment/rollenspiele/das-schwarze-auge/) über das Internet zu spielen. Es unterstützt den Import von durch die [Helden-Software](https://www.helden-software.de/) erstellten Charakteren.

## Client

### Installation

Klickt auf [Releases](https://github.com/Derevar/PhexensWuerfelraum/releases/latest), ladet euch die `setup.msi` des aktuellen Releases herunter und installiert das Programm.

### Daten
Alle Daten werden unter `%LocalAppData%/PhexensWuerfelraum` gespeichert.

## Server

### Build
#### Linux

Zum Build wird das [.NET Core 3.1 x64 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks) benötigt.

Führt folgendes in der Paket Manager Console im Visual Studio aus, um eine ausführbare Datei für Linux zu erstellen
```powershell
dotnet publish Server\Server.Console -c release -r ubuntu.18.04-x64 -o C:\VSDistribution\PhexensWuerfelraum.Server.Console
```

##### Run
Das Ausführen des Server setzt das Mono Framework voraus. Die sicherste Methode um alle Abhängigkeiten zu installieren wäre:
```bash
apt install mono-complete
```

Mache die Applikation ausführbar.
```bash
chmod +x PhexensWuerfelraum.Server.Console
```

Starte die Applikation. Die Angabe des Portes kann wahlweise über die `settings.ini` oder einen Übergabeparameter erfolgen. Der Übergabeparameter hat Vorrang.
```bash
./PhexensWuerfelraum.Server.Console --port 12113
```

#### Windows

Führt folgendes in der Paket Manager Console im Visual Studio aus, um eine ausführbare Datei für Windows zu erstellen
```powershell
dotnet publish Server\Server.Console -c release -r win10-x86 -o C:\VSDistribution\PhexensWuerfelraum.Server.Console
```

##### Run

Starte die Applikation. Die Angabe des Portes kann wahlweise über die `settings.ini` oder einen Übergabeparameter erfolgen. Der Übergabeparameter hat Vorrang.
```bash
C:\VSDistribution\PhexensWuerfelraum.Server.Console\PhexensWuerfelraum.Server.Console.exe --port 12113
```

## Lizenz

Phexens Würfelraum ist unter der MIT Lizenz veröffentlicht. 
Für Details siehe: [LICENSE.txt](LICENSE)
Lizenzen von Dritt-Hersteller-Software, Grafiken und Soundeffekten finden sich hier: [THIRD-PARTY-LICENSES.txt](THIRD-PARTY-LICENSES.txt)

DAS SCHWARZE AUGE, AVENTURIEN, DERE, MYRANOR, THARUN, UTHURIA und RIESLAND sind eingetragene Marken der Significant Fantasy Medienrechte GbR.

| EN |
|:---|

# Phex's Dice Room
Phex's Dice Room is an application to support online play of the pen and paper game [The Dark Eye](https://www.ulisses-us.com/games/the-dark-eye/).
I currently do not have plans to support other languages apart from german.

## License
Phex's Dice Room is licensed under the MIT license.
For details see: [LICENSE.txt](LICENSE)
For licenses of third partys see: [THIRD-PARTY-LICENSES.txt](THIRD-PARTY-LICENSES.txt)
