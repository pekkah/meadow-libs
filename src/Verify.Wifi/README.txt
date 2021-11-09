
## Build
dotnet.exe build -t:Build -p:Configuration=Debug

## Deploy
meadow app deploy -f .\bin\Debug\netstandard2.1\App.dll