# PharmaLink.Api

API para gestión de medicamentos, insumos y dispensaciones. Usa .NET 8, EF Core InMemory por defecto, Swagger para documentación y un esquema de validación de recetas vía API externa (hospital).


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
  "BaseUrl": "http://localhost:7000", // Cambia si la API hospital corre en otro puerto
  "ApiKey": "hospital-key"
}
```
Ajusta el puerto para que no choque con esta API.



---
© 2025 PharmaLink
