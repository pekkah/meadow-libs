[CmdletBinding()]
param (
    [Parameter()]
    [String]
    $Source,

    [Parameter()]
    [String]
    $DestinationServer,

    [Parameter()]
    [String]
    $Destination
)

$Source = Resolve-Path -Path $Source
$SourceWsl = wsl wslpath -a $Source.Replace("\", "\\")
$Destination = wsl wslpath $Destination.Replace("\", "\\")
$DestinationWsl = "$DestinationServer`:$Destination"
Write-Host "Source: $SourceWsl ($Source)"
Write-Host "Destination: $DestinationWsl ($Destination)"
Write-Host "Exec: ssh $DestinationServer mkdir -p $Destination"
invoke-expression "ssh $DestinationServer mkdir -p $Destination"
Write-Host "Exec: wsl rsync -rv --no-p --no-g $SourceWsl $DestinationWsl"
invoke-expression "wsl rsync -rv --no-p --no-g $SourceWsl $DestinationWsl"
Write-Host "Complete"