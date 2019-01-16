# Phexens Würfelraum
Phexens Würfelraum ist ein Programm um mit anderen Menschen gemeinsam [Das Schwarze Auge](http://www.ulisses-spiele.de/sortiment/rollenspiele/das-schwarze-auge/) online zu spielen. Es unterstützt den Import von durch die [Helden-Software](https://www.helden-software.de/) erstellten Charakteren.

## Server

### Linux
Install Mono on server
```bash
apt install mono-complete
```

Run in powershell console in VS to compile executable for Ubuntu
```powershell
dotnet publish Server\Server.Console -c release -r ubuntu.16.04-x64 -o C:\VSDistribution\PhexensWuerfelraum.Server.Console
```

Run the server
```bash
chmod +x PhexensWuerfelraum.Server.Console
./PhexensWuerfelraum.Server.Console --port 1212
```

### Windows