# PharmaLink API - Documentación Completa

## 1. Introducción
PharmaLink es una API REST para gestionar medicamentos, insumos y el flujo de dispensación y reposición asociados a un hospital. Integra una API externa del hospital para validar recetas electrónicas y responder pedidos de reposición.

Objetivos:
- CRUD básico de medicamentos e insumos.
- Registrar dispensaciones (validando recetas externas y gestionando stock).
- Recibir y confirmar reposiciones de insumos (actualizando stock).
- Proveer documentación interactiva via Swagger.
- Autenticación mediante API Key en operaciones sensibles.

## 2. Arquitectura General
Estructura por carpetas:
- `Program.cs`: Punto de entrada. Configura puerto, invoca Startup y realiza seeding según flags / proveedor.
- `Startup.cs`: Registra servicios (Controllers, Swagger, DbContext, HttpClient) y el pipeline HTTP.
- `Data/PharmaLinkContext.cs`: DbContext de EF Core (Medicamentos, Insumos, Dispensaciones).
- `Data/DataInitializer.cs`: Carga datos demo si la base está vacía.
- `Models/`: Entidades persistentes y DTOs.
- `Middleware/ApiKeyMiddleware.cs`: Filtra acceso a rutas protegidas por `X-API-KEY`.
- `Docs/`: Archivos de documentación y colección Postman.
- `Migrations/`: Migraciones EF Core (estructura de la BD).
- `appsettings*.json`: Configuraciones (connection strings, flags, API externas).

## 3. Flujo de Inicio
1. Ejecutar `dotnet run` (idealmente con entorno Development para usar `appsettings.Development.json`).
2. `Program.cs` crea el builder y llama a `Startup.ConfigureServices`.
3. `Startup` registra dependencias, incluyendo DbContext:
   - Si `ConnectionStrings:DefaultConnection` apunta a SQLite (`pharmalink.db`) => persistente.
   - Si no hay connection string pero existe `pharmalink.db` => usa SQLite.
   - Si no existe => usa InMemory (datos no persisten tras reinicio).
4. Se construye la app y se configura el pipeline (Swagger, Routing, Authorization).
5. Seeding opcional: si provider es InMemory o `Demo:SeedOnStart = true`, se insertan datos de ejemplo.

## 4. Modelos Principales
### Medicamento
Campos: Id, Codigo, Nombre, Presentacion, Stock, PrecioUnitario.
### Insumo
Campos: Id, Codigo, Nombre, Presentacion, Stock, PrecioUnitario.
### Dispensacion
Campos: Id, CodigoReceta, MedicamentoId, Cantidad, FechaEntrega, Confirmado.

## 5. DTOs
- `DispensacionRequestDto`: CodigoReceta, MedicamentoId, Cantidad.
- `DispensacionResponseDto`: Id, CodigoReceta, MedicamentoId, Cantidad, FechaEntrega, Confirmado.
- `PedidoInsumosDto`: PedidoId, HospitalId, FechaPedido, Items[], Contacto.
- `PedidoItemDto`: Codigo, Nombre, Presentacion, CantidadSolicitada, Unidad, Prioridad.
- `ConfirmacionEnvioDto`: PedidoId, FechaEnvio, Items[], NumeroRemito.
- `ConfirmacionItemDto`: Codigo, CantidadEnviada.
- `ValidacionRecetaRequestDto / ResponseDto`: Estructuras para validar la receta (flexible para la API del hospital).

## 6. Endpoints (Actuales)
### Medicamentos
- `GET /api/medicamentos` -> Lista todos.
- `POST /api/medicamentos` -> Crea uno nuevo.
- `PUT /api/medicamentos/{id}` -> Actualiza existente.
(Pendiente opcional: `DELETE /api/medicamentos/{id}`)

### Insumos
- `GET /api/insumos`
- `POST /api/insumos`
- `PUT /api/insumos/{id}`
(Pendiente opcional: `DELETE /api/insumos/{id}`)

### Dispensaciones
- `POST /api/dispensaciones` -> Valida receta vía hospital, descuenta stock y registra entrega.
- `GET /api/dispensaciones/{id}` -> Obtiene una dispensación registrada.
(Pendiente: listado general, eliminación, actualización de dispensación).

### Recetas
- `POST /api/recetas/validate` -> Envía código de receta a API hospital para validar.
(Pendiente: `POST /api/recetas/notify` sugerido en colección Postman, para recibir avisos del hospital.)

### Reposiciones
- `POST /api/reposiciones/pedidos` -> Recibe pedido y devuelve estado (ACCEPTED/PARTIAL/REJECTED).
- `POST /api/reposiciones/confirmacion` -> Actualiza stock según items enviados.
(Pendiente: persistir histórico de confirmaciones/pedidos y vista combinada.)

## 7. Lógica de Dispensación
1. Petición con receta y MedicamentoId.
2. Middleware (si agregado) valida API Key.
3. Controller llama HttpClient `HospitalApi` para `/recetas/{codigo}/validate`.
4. Si receta válida: busca medicamento y chequea stock.
5. Transacción: crea Dispensacion y descuenta stock.
6. Devuelve DTO con confirmación.

## 8. Gestión de Reposiciones
- Pedido: Se evalúa disponibilidad de cada insumo, calcula aceptados/parciales/rechazados.
- Confirmación: Transacción que suma stock (crea insumos nuevos si no existían). Falta almacenamiento formal del evento.

## 9. Autenticación por API Key
- Middleware revisa encabezado `X-API-KEY` en rutas sensibles.
- Comparación con valor en `appsettings.json` (`ApiKey`).
- Respuestas de error estandarizadas parciales (ErrorResponse con Code y Message). Se puede extender a incluir `Details`.
- Nota: Debe agregarse `app.UseMiddleware<ApiKeyMiddleware>();` en Startup para activarlo (si todavía no está).

## 10. Persistencia y EF Core
Proveedores soportados:
- SQLite (archivo `pharmalink.db`).
- SQL Server (si se configura una cadena que no aparenta ser SQLite).
- InMemory (fallback para pruebas rápidas).
Migraciones:
- Carpeta `Migrations/` contiene la migración inicial para crear tablas.
Operaciones:
- Add -> EntityState.Added -> SaveChanges.
- Update -> Cambios en tracked entity -> SaveChanges.
- Delete (si se implementa) -> Remove -> SaveChanges.

## 11. Seeding
- DataInitializer inserta lista básica de Medicamentos e Insumos.
- Corre si la tabla está vacía y se activó flag o provider es InMemory.
- Evita duplicados revisando existencia previa.

## 12. Manejo de Errores
- Formato actual: `ErrorResponse { Code, Message }`.
- Recomendado ampliar a `{ Code, Message, Details }` para alinearse a enunciado.
- Controladores principales ya retornan códigos HTTP apropiados (400, 404, 500, etc.) en casos clave.

## 13. Integración con API del Hospital
- HttpClient registrado como `HospitalApi` con BaseAddress configurable en `appsettings.json`.
- Usa headers (`X-API-KEY`) si se provee ApiKey del hospital.
- Dependencias externas simuladas mediante endpoints de validación.

## 14. Seguridad (Estado Actual)
- Middleware implementado pero requiere añadirse al pipeline.
- Sin autenticación por token/JWT (no exigido).
- Medicamentos/Insumos abiertos (podrían protegerse opcionalmente).

## 15. Gaps Identificados
- Falta DELETE en Medicamentos/Insumos (no obligatorio, pero buena práctica).
- Falta almacenamiento histórico de reposiciones confirmadas (solo se ajusta stock, sin auditoría).
- Falta vista combinada del flujo de reposición (desafío adicional del enunciado).
- Middleware de API Key debe activarse explícitamente.
- ErrorResponse no incluye `details` y no se usa en todos los controladores.
- No hay endpoint para listar todas las dispensaciones.
- Falta `POST /api/recetas/notify` (aparece en PostmanCollection como esperado de integración hospital -> farmacia).

## 16. Mejoras Propuestas
1. Agregar `app.UseMiddleware<ApiKeyMiddleware>();` en `Startup.Configure`.
2. Crear entidad `ReposicionEntrega` y registrar cada confirmación.
3. Endpoint `GET /api/reposiciones/seguimiento` que combine pedidos, confirmaciones y stock actual.
4. Implementar DELETE en Medicamentos e Insumos.
5. Uniformar errores con `{ code, message, details }`.
6. Endpoint `GET /api/dispensaciones` para listar todas.
7. Implementar `POST /api/recetas/notify` para recibir eventos del hospital.

## 17. Ejemplo de Flujo (Dispensación)
```text
Cliente -> POST /api/dispensaciones
  Body: { "codigoReceta": "REC-123", "medicamentoId": 1, "cantidad": 5 }
API -> Valida receta externa
  Si válida -> busca medicamento y stock
    OK -> Crea Dispensacion, descuenta stock, responde 201 con DTO
    NO -> 400 con error (stock insuficiente / receta inválida)
```

## 18. Ejemplo de Flujo (Reposición)
```text
Hospital -> POST /api/reposiciones/pedidos
API -> Evalúa cada item -> Respuesta con estado
Hospital -> POST /api/reposiciones/confirmacion (envío real)
API -> Suma stock y responde detalles
(Pendiente: guardar historial y exponer seguimiento)
```

## 19. Swagger
- URL: `http://localhost:5000/swagger`
- Permite probar endpoints y ver modelos.
- Siempre habilitado para facilitar la demo.

## 20. Checklist Rápido Antes de Demo
- ¿Existe `pharmalink.db`? Sí => persistencia.
- ¿Seeding activo solo si hace falta? Sí (Demo:SeedOnStart).
- ¿Swagger operativo? Sí.
- ¿HospitalApi BaseUrl seteado? Ajustar según puerto real.
- ¿API Key middleware activo? Verificar si se añadió al pipeline.

## 21. Preguntas Frecuentes
- “¿Por qué no persisten los datos?” -> Probablemente se está usando InMemory (sin archivo ni connection string).
- “¿Cómo cambio a SQL Server?” -> Editar `ConnectionStrings:DefaultConnection` con cadena SQL Server y correr migraciones.
- “¿Cómo protejo todos los endpoints?” -> Extender middleware para más rutas o agregar autenticación global.
- “¿Cómo reinicio seeding?” -> Borrar `pharmalink.db` si se usa SQLite y reiniciar con SeedOnStart=true.

## 22. Roadmap Futuro
- Auditoría completa de reposiciones y dispensaciones.
- Paginación y filtros en listados.
- Tests de integración (WebApplicationFactory).
- Autenticación avanzada (JWT + roles/claims).
- Cache de recetas válidas para reducir llamadas externas.
- Observabilidad (logging estructurado, métricas).

---
© 2025 PharmaLink

---

## Anexo A. Ciclo de vida de una request (paso a paso)
1) Cliente hace una petición HTTP a `http://localhost:5000/...`.
2) La request entra al pipeline (middlewares) en el orden definido en `Startup.Configure`.
3) Enrutado: ASP.NET Core busca un método de controlador que coincida con la ruta y el verbo.
4) Inyección de dependencias: crea la instancia del controlador y le pasa lo que pidió por constructor (DbContext, HttpClient, etc.).
5) Lógica de negocio: el método ejecuta validaciones, llama EF Core, otras APIs, etc.
6) Respuesta: retorna un resultado con status code (200/201/400/404/500) y JSON.

## Anexo B. Inyección de dependencias (DI)
- `services.AddControllers()`: registra controladores.
- `services.AddDbContext<PharmaLinkContext>(...)`: contexto EF Core (ciclo de vida scoped: 1 por request).
- `services.AddHttpClient("HospitalApi", ...)`: cliente HTTP nombrado para la API del hospital.
- En controladores, pedís dependencias por constructor: ASP.NET Core te las pasa automáticamente.

## Anexo C. EF Core: consultas, cambios y transacciones
Consultas típicas:
- `Find(id)`: trae por clave primaria (si está trackeado, usa caché).
- `FirstOrDefault(...)`, `Where(...)`: consulta traducida a SQL.
- `ToList()`: materializa resultados.

Cambios:
- Agregar: `_context.Set<T>().Add(entidad); await _context.SaveChangesAsync();`
- Modificar: cargar entidad, cambiar propiedades y `SaveChangesAsync()`.
- Eliminar: `_context.Remove(entidad); await _context.SaveChangesAsync();`

Transacciones:
```
using var tx = await _context.Database.BeginTransactionAsync();
// ... cambios ...
await _context.SaveChangesAsync();
await tx.CommitAsync();
```
En error: `await tx.RollbackAsync();` y devolver 500 con ErrorResponse.

## Anexo D. Ejemplos de respuestas JSON
- 200 OK (GET medicamentos): `[{ "id":1, "codigo":"MED-001", "nombre":"Ibuprofeno", ... }]`
- 201 Created (POST): `{ "id": 10, "codigo":"MED-XYZ", ... }` y header Location.
- 400 BadRequest (validación): `{ "code":"invalid_request", "message":"Campo X requerido", "details":["..."] }`
- 404 NotFound: `{ "code":"not_found", "message":"Recurso no existe" }`
- 500 Error: `{ "code":"db_error", "message":"Error inesperado" }`

## Anexo E. Seguridad por API Key (orden recomendado en pipeline)
1) `app.UseSwagger(); app.UseSwaggerUI();`
2) `app.UseRouting();`
3) `app.UseMiddleware<ApiKeyMiddleware>();` (protege recetas, dispensaciones, reposiciones)
4) `app.UseAuthorization();`
5) `app.UseEndpoints(endpoints => endpoints.MapControllers());`

Header a enviar: `X-API-KEY: <tu_api_key>`

## Anexo F. Integración con API del hospital
- `AddHttpClient("HospitalApi", ...)` con BaseAddress de `appsettings.json`.
- En el controlador: `var client = _http.CreateClient("HospitalApi");`
- Opcional: `client.DefaultRequestHeaders.Add("X-API-KEY", apiKeyDelHospital);`
- Validación receta: `GET /recetas/{codigo}/validate` (la ruta real puede variar según el equipo hospital).

## Anexo G. Troubleshooting rápido
- Swagger no abre: verificar puerto 5000 y que `UseSwagger/UseSwaggerUI` estén en Startup.
- No persisten datos: probablemente está usando InMemory. Asegurar `pharmalink.db` o `ConnectionStrings` en Development.
- Warning HTTPS: quitar `UseHttpsRedirection()` si no escuchás en https.
- Dispensación falla: revisar `HospitalApi:BaseUrl` y la `X-API-KEY` del hospital.
- Reposiciones no cambian stock: chequear body de confirmación y validaciones de cantidad.

## Anexo H. Glosario rápido
- API Key: clave en header `X-API-KEY` para autorizar rutas.
- DTO: objeto de transferencia entre cliente y servidor (evita exponer entidades directamente).
- DbContext: unidad de trabajo de EF Core; traduce LINQ a SQL y trackea cambios.
- Middleware: componente del pipeline que intercepta requests/responses.
- Migración: cambio de esquema de la BD representado en C# (crea/actualiza tablas).

## Anexo I. Roadmap detallado (ideas de mejora)
- Persistir historial de reposiciones (tabla ReposicionEntrega) y listar auditoría.
- Vista combinada de reposiciones: pedidos + confirmaciones + stock actual.
- DELETE para medicamentos/insumos (si piden CRUD completo).
- Estandarizar errores `{ code, message, details }` en todos los controladores.
- Endpoint `GET /api/dispensaciones` para listar.
- Endpoint `POST /api/recetas/notify` para flujo hospital -> farmacia.
- Logs de arranque: imprimir proveedor EF (SQLite/InMemory/SQL Server) para diagnóstico.
