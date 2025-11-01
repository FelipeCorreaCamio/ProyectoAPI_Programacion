# ProyectoAPI_Programacion

# 📂 Estructura del Proyecto PharmaLink

- **🧑‍💻 Controllers/**  
  Acá van los cinco controladores que pide el proyecto: `Medicamentos`, `Dispensaciones`, `Insumos`, `Recetas` y `Reposiciones`. Cada uno hace lo que le toca en la API.

- **📦 Models/**  
  Tiene las tres entidades que pedía el entregable: `Medicamento`, `Dispensacion` e `Insumo`.

- **🗄️ Data/**  
  Está `PharmaLinkContext.cs`, que es el contexto de la base de datos y está hecho según lo que pide el modelo.

- **🔑 Middleware/**  
  Contiene `ApiKeyMiddleware.cs`, que se encarga de la autenticación con API Key en los headers.

- **🛠️ Utils/**  
  Tiene `ErrorResponse.cs` para manejar los errores de manera uniforme con el formato `{ code, message, details }`.

- **📄 Docs/**  
  Está `Endpoints.cs` donde se documentan los endpoints de la API. Lo dejé como `.cs` para poder poner comentarios o ejemplos de código.

- **⚙️ Program.cs y Startup.cs**  
  archivos que sirven para levantar la API y configurar los servicios, rutas y middlewares.

- **📝 appsettings.json**  
  Para poner las claves, la conexión a la base de datos y otras cosas de configuración.

- **📘 README.md**  
  Lo pusimos por si alguien lo quiere usar para ver de qué va el proyecto.
