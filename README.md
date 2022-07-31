[![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/markuskonojacki/PhexensWuerfelraum/blob/master/LICENSE)
[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://github.com/markuskonojacki/PhexensWuerfelraum/graphs/contributors)
![GitHub all releases](https://img.shields.io/github/downloads/markuskonojacki/PhexensWuerfelraum/total)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/markuskonojacki/PhexensWuerfelraum)

| [DE](#phexens-würfelraum)
|:---|
| [EN](#phexs-dice-room)

![](Ui/Ui.Desktop/Resources/AppIcon.ico)

![](screenshot.png)

| DE |
|:---|

# Phexens Würfelraum
Zuvorderst: Phexens Würfelraum ist ein komplett __NICHT__ offizielles Projekt meinerseits. Es besteht keinerlei offizielle Verbindung zu Fanpro, Ulisses Spiele, der Significant Fantasy Medienrechte GbR oder ihren Produkten.

Nun zum Tool selbst: Phexens Würfelraum ist ein Programm um es Menschen zu ermöglichen, gemeinsam [Das Schwarze Auge](http://www.ulisses-spiele.de/sortiment/rollenspiele/das-schwarze-auge/) über das Internet zu spielen. Es unterstützt den Import von durch die [Helden-Software](https://www.helden-software.de/) erstellten Charakteren.

## Client

### Installation

Klickt auf [Releases](https://github.com/markuskonojacki/PhexensWuerfelraum/releases/latest), ladet euch die `PhexensWuerfelraum-v*.*.*.zip` des aktuellen Releases herunter, entpackt die Datei `PhexensWuerfelraum.exe` und startet sie. Phexens Würfelraum installiert sich von selbst und erstellt Verknüpfungen auf Desktop sowie im Startmenü.

### Daten
Alle Daten werden unter `%LocalAppData%/PhexensWuerfelraum` gespeichert.

## Server

### Docker

Beispiel `docker-compose.yml`:

```yml
---
version: '3.8'

services:
  phexenswuerfelraum:
    container_name: phexenswuerfelraum-server
    image: derevar/phexenswuerfelraum-server:latest
    ports:
      - 12113:12113
    environment:
      - PUID=1000
      - PGID=1000
      - TZ=Europe/Berlin
    volumes:
      - /appdata/phexenswuerfelraum/config:/app/phexenswuerfelraum/config
    restart: unless-stopped
```

### Ohne Docker

Ladet euch aus den [Releases](https://github.com/markuskonojacki/PhexensWuerfelraum/releases/latest) die für eurer Betriebssystem passende `Server-*.zip` herunter und entpackt diese. Starte die Applikation.

#### Linux
```bash
.\PhexensWuerfelraum.Server.Console
```

#### Windows
```powershell
.\PhexensWuerfelraum.Server.Console.exe
```

## Lizenz

Phexens Würfelraum ist unter der MIT Lizenz veröffentlicht. 
Für Details siehe: [LICENSE.txt](LICENSE.txt)
Lizenzen von Dritt-Hersteller-Software, Grafiken und Soundeffekten finden sich hier: [THIRD-PARTY-LICENSES.txt](THIRD-PARTY-LICENSES.txt)

DAS SCHWARZE AUGE, AVENTURIEN, DERE, MYRANOR, THARUN, UTHURIA und RIESLAND sind eingetragene Marken der Significant Fantasy Medienrechte GbR.

| EN |
|:---|

# Phex's Dice Room
Phex's Dice Room is an application to support online play of the pen and paper game [The Dark Eye](https://www.ulisses-us.com/games/the-dark-eye/).
I currently do not have plans to support other languages apart from german.

## License
Phex's Dice Room is licensed under the MIT license.
For details see: [LICENSE.txt](LICENSE.txt)
For licenses of third partys see: [THIRD-PARTY-LICENSES.txt](THIRD-PARTY-LICENSES.txt)
