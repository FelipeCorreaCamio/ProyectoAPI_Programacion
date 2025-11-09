Param(
    [switch]$NoBrowser,
    [int]$Port = 5000
)

Write-Host "Deteniendo procesos previos de PharmaLink.Api..." -ForegroundColor Cyan
Get-Process -Name PharmaLink.Api -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Limpiando bin/ y obj/ si estaban bloqueados..." -ForegroundColor Cyan
Remove-Item -Recurse -Force "$PSScriptRoot\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$PSScriptRoot\obj" -ErrorAction SilentlyContinue

Write-Host "Restaurando paquetes..." -ForegroundColor Cyan
 dotnet restore | Out-Null

Write-Host "Compilando..." -ForegroundColor Cyan
 dotnet build | Out-Null

Write-Host "Lanzando API en puerto $Port (ASPNETCORE_ENVIRONMENT=Development)" -ForegroundColor Green
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:$Port"

Start-Process -FilePath "dotnet" -ArgumentList "run" -WorkingDirectory $PSScriptRoot

Start-Sleep -Seconds 3

try {
    $status = (Invoke-WebRequest -UseBasicParsing http://localhost:$Port/swagger/index.html).StatusCode
    Write-Host "Swagger responde con status $status" -ForegroundColor Green
    if(-not $NoBrowser){
        Start-Process "http://localhost:$Port/swagger" | Out-Null
    }
} catch {
    Write-Warning "No se pudo obtener Swagger todavía. Revisa el log del proceso." 
}

Write-Host "Listo. Usa Ctrl+C en la ventana del proceso para detener." -ForegroundColor Yellow
