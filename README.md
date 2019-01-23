| README.md |
|:---|

![](Ui/Ui.Desktop/Resources/AppIcon.ico)

# Phexens Würfelraum
Phexens Würfelraum ist ein Programm um mit anderen Menschen gemeinsam [Das Schwarze Auge](http://www.ulisses-spiele.de/sortiment/rollenspiele/das-schwarze-auge/) online zu spielen. Es unterstützt den Import von durch die [Helden-Software](https://www.helden-software.de/) erstellten Charakteren.

## Server

### Build
#### Linux

Run in packet manager console in VS to compile executable for Ubuntu
```powershell
dotnet publish Server\Server.Console -c release -r ubuntu.18.04-x64 -o C:\VSDistribution\PhexensWuerfelraum.Server.Console
```

### Run
Das Ausführen des Server setzt das Mono Framework voraus.
```bash
apt install mono-complete
```

Mache die Applikation ausführbar.
```bash
chmod +x PhexensWuerfelraum.Server.Console
```

Starte die Applikation. Die Angabe des Portes kann wahlweise über die `settings.ini` oder einen Übergabeparameter erfolgen.
```bash
./PhexensWuerfelraum.Server.Console --port 1212
```

### Windows

## Lizenz

Phexens Würfelraum ist unter der MIT Lizenz veröffentlicht. Für Details siehe: [LICENSE.txt](LICENSE)
