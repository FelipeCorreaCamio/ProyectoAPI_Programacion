# PharmaLink.Api

API para gestión de medicamentos, insumos y dispensaciones. Usa .NET 8, EF Core InMemory por defecto, Swagger para documentación y un esquema de validación de recetas vía API externa (hospital).

## Requisitos
- .NET 8 SDK instalado
- PowerShell 5+ (incluido en Windows)

## Ejecución rápida

```powershell
# En la carpeta del proyecto
cd "c:\Users\pipep\Documents\CosasUAP\2doAño-SegundoCuatrimestre\programacion2\ProyectoAPI_Programacion\PharmaLink.Api"
./run.ps1 -Port 5000
```

Esto:
1. Detiene procesos previos que bloqueen el exe
2. Limpia bin/ y obj/ si están bloqueados
3. Restaura y compila
4. Arranca la API en modo Development
5. Abre Swagger (si no usas -NoBrowser)

Si prefieres manual:
```powershell
cd "c:\Users\pipep\Documents\CosasUAP\2doAño-SegundoCuatrimestre\programacion2\ProyectoAPI_Programacion\PharmaLink.Api"
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

## Endpoints principales
- GET `/api/medicamentos` lista medicamentos
- POST `/api/medicamentos` crea medicamento
- PUT `/api/medicamentos/{id}` actualiza
- POST `/api/dispensaciones` valida receta externa y descuenta stock

Swagger: `http://localhost:5000/swagger`

## Configuración externa (Hospital API)
En `appsettings.json`:
```json
"HospitalApi": {
  "BaseUrl": "http://localhost:7000", // Cambia si tu API hospital corre en otro puerto
  "ApiKey": "hospital-key"
}
```
Ajusta el puerto para que no choque con esta API.

## Semilla de datos
Al iniciar, `DataInitializer` carga medicamentos e insumos de prueba si la base está vacía (solo InMemory). Si migras a SQL Server, quita o adapta el seeding.

## Cambiar a SQL Server
Agrega en `appsettings.Development.json` o `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PharmaLink;Trusted_Connection=True;TrustServerCertificate=True"  
}
```
Y crea migraciones:
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```
(Asegúrate de tener instalado el paquete `Microsoft.EntityFrameworkCore.Tools` si hace falta.)

## Problemas comunes
| Problema | Causa | Solución |
|----------|-------|----------|
| MSB3021/3027 copia exe | Proceso previo bloqueando `PharmaLink.Api.exe` | `taskkill /F /IM PharmaLink.Api.exe` o usar `run.ps1` |
| AddressInUse 3000 | Puerto ocupado | Cambiar puerto (`run.ps1 -Port 5000`) |
| Lista vacía medicamentos | Seeding no corrió | Ver log "Saved 10 entities" y reiniciar limpiando bin/obj |

## Pruebas rápidas vía PowerShell
```powershell
Invoke-WebRequest -UseBasicParsing http://localhost:5000/api/medicamentos | Select-Object StatusCode
Invoke-RestMethod -Method GET http://localhost:5000/api/medicamentos | Format-Table Nombre,Stock
```

## Próximos pasos sugeridos
- Añadir validaciones de modelo (DataAnnotations) a DTOs
- Añadir autenticación real para endpoints sensibles
- Tests automatizados con xUnit
- Persistencia real usando SQL Server

---
© 2025 PharmaLink
