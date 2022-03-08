function Invoke-Publish($out){
    $out = $out ?? (Read-Host "Output directory")
    dotnet publish -c Release --output $out /p:DebugType=None
}

Invoke-Publish @args
