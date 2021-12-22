## Configure

Add `wifi.config.yaml` with wifi credentials (ignored in .gitignore)

```yaml
Credentials:
    Ssid: [SSID]
    Password: [PASSWORD]
```

## Build
dotnet.exe build -t:Build -p:Configuration=Debug

## Deploy
meadow app deploy -f .\bin\Debug\netstandard2.1\App.dll