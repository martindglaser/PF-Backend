# 🚀 AnalyzerGateway.Api
## Backend Oficial – Plataforma BDT Global
API Gateway construido en ASP.NET Core, encargado de gestionar el análisis, comunicación con servicios externos y orquestación interna del ecosistema.

### 📦 Stack Tecnológico
Este backend está construido con:

- .NET 8 / ASP.NET Core
- Entity Framework Core
- Swagger / OpenAPI
- AutoMapper
- Newtonsoft.Json
- EFCore.Design + EF Tools
- SQL Lite
- CORS + Middlewares + HttpClientFactory

### 🗂️ Estructura del Proyecto
AnalyzerGateway.Api/
- Controllers/        → Endpoints HTTP
- Application/        → Casos de uso, DTOs, lógica de negocio
- Infrastructure/     → Persistencia, DBContext, Repositorios
- Domain/             → Entidades de dominio
- Migrations/         → Migraciones EF Core
- appsettings.json    → Configuración de la aplicación
- AnalyzerGateway.Api.csproj

## ⚙️ Instalación (Local)

1.  **Clonar el repositorio:**
    Abre tu terminal y clona este repositorio.

    ```bash
    git clone https://github.com/martindglaser/PF-Backend.git
    ```

2.  **Acceder al directorio:**
    ```bash
    cd src/AnalyzerGateway.Api
    ```

3. **Restaurar herramientas locales (EF Tools)**

    ```bash
    dotnet tool restore
    ```

3.  **Restaurar paquetes**
    ```bash
    dotnet restore
    ```
    
4.  **Crear carpeta de datos (si es necesario)**
    ```bash
    mkdir -p ../data
    ```

5.  **Aplicar migraciones**
    ```bash
    dotnet ef database update
    ```

6.  **Ejecutar el proyecto**
    ```bash
    dotnet run
    ```
    
## 🌐 Endpoints Principales
Una vez corriendo, accedés acá:

| Tipo           | URL                                                            |
| -------------- | -------------------------------------------------------------- |
| **API Base**   | [http://localhost:5000](http://localhost:5000)                 |
| **Swagger UI** | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
