# RENAPER API

API REST para consulta de datos de personas del padrón nacional. Sistema de gestión de API keys con límites de solicitudes semanales y funcionalidad de ampliación de cuota mediante pagos.

## 📋 Descripción

RENAPER es una API que simula ser una institución gubernamental que ofrece datos de personas a empresas privadas. El sistema implementa un modelo de negocio basado en consultas con límites semanales y permite la ampliación de cuotas mediante pagos.

### Características principales

- ✅ Consulta de datos de personas por CUIL
- ✅ Sistema de API keys con límite semanal de solicitudes
- ✅ Ampliación de cuota mediante pago ($1 por cada 10 consultas adicionales)
- ✅ Validación automática de API keys en cada request
- ✅ Reset automático de contadores semanales
- ✅ Arquitectura en 3 capas (API, Aplicación, Dominio)
- ✅ Soporte para PostgreSQL
- ✅ Documentación Swagger/OpenAPI

## 🏗️ Arquitectura

El proyecto está estructurado en 3 capas siguiendo principios de arquitectura limpia:

```
src/
├── RENAPER.Api/          # Capa de presentación (Controllers, Configuración)
├── RENAPER.Aplicacion/   # Capa de aplicación (Services, Lógica de negocio)
└── RENAPER.Dominio/      # Capa de dominio (Entidades, DbContext, Migraciones)
```

### Capas

- **API**: Controllers, configuración de servicios, middleware, Swagger
- **Aplicación**: Servicios de negocio (`IPersonaService`, `IApiKeyService`)
- **Dominio**: Entidades del dominio (`Persona`, `ApiKey`), `RenaperDbContext`, migraciones

## 🛠️ Tecnologías

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **PostgreSQL** (Npgsql)
- **Swagger/OpenAPI**
- **Docker** (opcional)

## 📦 Requisitos

- .NET 8.0 SDK
- PostgreSQL 12+ (o usar LocalDB SQL Server cambiando la configuración)
- Visual Studio 2022 / VS Code / Rider (opcional)

## 🚀 Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone <repository-url>
cd RENAPER
```

### 2. Configurar la base de datos

Edita el archivo `src/RENAPER.Api/appsettings.json` y configura la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=renaper-db"
  }
}
```

**Nota**: El proyecto está configurado para PostgreSQL. Para usar SQL Server, cambia:
- En `Program.cs`: `UseNpgsql` → `UseSqlServer`
- En `RENAPER.Api.csproj`: `Npgsql.EntityFrameworkCore.PostgreSQL` → `Microsoft.EntityFrameworkCore.SqlServer`
- Actualiza la connection string

### 3. Crear la base de datos

La aplicación crea automáticamente la base de datos al iniciar usando `EnsureCreated()`. Si prefieres usar migraciones:

```bash
cd src/RENAPER.Dominio
dotnet ef migrations add InitialCreate --startup-project ../RENAPER.Api
dotnet ef database update --startup-project ../RENAPER.Api
```

### 4. Ejecutar la aplicación

```bash
cd src/RENAPER.Api
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📚 Endpoints de la API

### 1. Crear API Key

Crea una nueva API key asociada a un email con un límite inicial de solicitudes semanales.

**Endpoint**: `POST /api/keys`

**Request Body**:
```json
{
  "mail": "usuario@ejemplo.com",
  "solicitudesSemanales": 100
}
```

**Response** (200 OK):
```json
{
  "apiKey": "a1b2c3d4e5f6...",
  "mail": "usuario@ejemplo.com",
  "solicitudesSemanales": 100
}
```

**Validaciones**:
- Mail requerido y formato válido
- Solicitudes semanales mayor a 0

---

### 2. Ampliar Solicitudes

Suma 10 solicitudes semanales adicionales a la API key del usuario (equivalente a $1 de pago).

**Endpoint**: `POST /api/keys/ampliar`

**Request Body**:
```json
{
  "mail": "usuario@ejemplo.com"
}
```

**Response** (200 OK):
```json
{
  "mensaje": "Se agregaron 10 solicitudes semanales exitosamente"
}
```

**Response** (404 Not Found):
```json
{
  "error": "No se encontró una API key activa para el mail: usuario@ejemplo.com"
}
```

**Validaciones**:
- Mail requerido y formato válido
- Debe existir una API key activa para el mail

---

### 3. Consultar Persona por CUIL

Obtiene los datos completos de una persona mediante su CUIL. Requiere API key válida y descuenta una solicitud del límite semanal.

**Endpoint**: `GET /api/personas/por-cuil/{cuil}`

**Headers**:
```
X-API-Key: {tu-api-key}
```

**Response** (200 OK):
```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "Pérez",
  "direccion": "Av. Corrientes 1234",
  "dni": "12345678",
  "cuil": "20123456789",
  "telefono": "1123456789",
  "mail": "juan.perez@email.com"
}
```

**Response** (401 Unauthorized):
```json
{
  "error": "API Key requerida. Envíe la cabecera X-API-Key"
}
```

o

```json
{
  "error": "API Key inválida o sin solicitudes disponibles"
}
```

**Response** (404 Not Found):
```json
{
  "error": "No se encontró una persona con CUIL: 20123456789"
}
```

---

## 🔑 Sistema de API Keys

### Funcionamiento

1. **Creación**: Cada API key se asocia a un email único y tiene un límite semanal de solicitudes.
2. **Validación**: En cada request, se verifica:
   - Que la API key exista y esté activa
   - Que tenga solicitudes disponibles en la semana actual
3. **Descuento**: Tras una consulta exitosa, se descuenta 1 solicitud del contador.
4. **Reset semanal**: Cada lunes (inicio de semana), el contador de solicitudes usadas se resetea automáticamente.
5. **Ampliación**: Se pueden agregar 10 solicitudes adicionales por $1 mediante el endpoint de ampliación.

### Modelo de datos

**ApiKey**:
- `Id`: Identificador único
- `Key`: Token de API key (GUID)
- `Mail`: Email del dueño (único)
- `SolicitudesSemanales`: Límite semanal de solicitudes
- `SolicitudesUsadas`: Contador de solicitudes usadas en la semana actual
- `FechaInicioSemana`: Fecha de inicio de la semana actual
- `Activa`: Estado de la API key

**Persona**:
- `Id`: Identificador único
- `Nombre`: Nombre de la persona
- `Apellido`: Apellido de la persona
- `Direccion`: Dirección completa
- `DNI`: Documento Nacional de Identidad (único)
- `CUIL`: Código Único de Identificación Laboral (único)
- `Telefono`: Teléfono de contacto
- `Mail`: Email de contacto

## 🗄️ Base de Datos

### Configuración

El proyecto utiliza **PostgreSQL** por defecto. La base de datos se crea automáticamente al iniciar la aplicación.

### Migraciones

Si necesitas crear migraciones manualmente:

```bash
cd src/RENAPER.Dominio
dotnet ef migrations add NombreMigracion --startup-project ../RENAPER.Api
dotnet ef database update --startup-project ../RENAPER.Api
```

### Cambiar a SQL Server

1. En `Program.cs`, cambia:
   ```csharp
   options.UseNpgsql(connectionString);
   ```
   por:
   ```csharp
   options.UseSqlServer(connectionString);
   ```

2. En `RENAPER.Api.csproj`, reemplaza:
   ```xml
   <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
   ```
   por:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
   ```

3. Actualiza la connection string en `appsettings.json`:
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RenaperDb;Trusted_Connection=true;MultipleActiveResultSets=true"
   ```

## 🐳 Docker

El proyecto incluye un `Dockerfile` para containerización.

### Construir la imagen

```bash
cd src/RENAPER.Api
docker build -t renaper-api .
```

### Ejecutar el contenedor

```bash
docker run -p 8080:8080 -p 8081:8081 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Port=5432;User Id=postgres;Password=admin;Database=renaper-db" \
  renaper-api
```

## 📖 Ejemplos de Uso

### 1. Crear una API Key

```bash
curl -X POST https://localhost:5001/api/keys \
  -H "Content-Type: application/json" \
  -d '{
    "mail": "empresa@ejemplo.com",
    "solicitudesSemanales": 50
  }'
```

### 2. Consultar una persona

```bash
curl -X GET https://localhost:5001/api/personas/por-cuil/20123456789 \
  -H "X-API-Key: a1b2c3d4e5f6..."
```

### 3. Ampliar solicitudes

```bash
curl -X POST https://localhost:5001/api/keys/ampliar \
  -H "Content-Type: application/json" \
  -d '{
    "mail": "empresa@ejemplo.com"
  }'
```

## 🧪 Testing

Para probar la API:

1. Inicia la aplicación
2. Accede a Swagger UI: `https://localhost:5001/swagger`
3. Crea una API key usando el endpoint `/api/keys`
4. Copia la API key generada
5. Usa el botón "Authorize" en Swagger o incluye el header `X-API-Key` en tus requests
6. Consulta personas usando el endpoint `/api/personas/por-cuil/{cuil}`

## 📝 Notas

- Las personas deben estar pre-cargadas en la base de datos. No hay endpoint para crear personas.
- El sistema asume que el pago ya fue procesado antes de llamar al endpoint de ampliación.
- El reset semanal ocurre automáticamente cuando se detecta que cambió la semana.
- Cada mail puede tener solo una API key activa.

## 🔒 Seguridad

**Nota**: Este es un MVP básico. Para producción, considera:

- Encriptar/hashear las API keys en la base de datos
- Implementar autenticación y autorización más robusta
- Rate limiting adicional
- Logging y auditoría
- Validación de datos más estricta
- HTTPS obligatorio

## 📄 Licencia

Este proyecto es un MVP de demostración.

## 👤 Autor

RENAPER - Sistema de consulta de datos de personas

---

**Versión**: 1.0.0  
**Última actualización**: 2025

