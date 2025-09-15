# Guía de Instalación del Proyecto

Seguir estos pasos para preparar y ejecutar el proyecto en tu entorno local:

---

## 1️⃣ Clonar el repositorio  
git clone https://github.com/tuusuario/tu-repo.git

cd tu-repo/src/AnalyzerGateway.Api
## 3️⃣ Restaurar paquetes
dotnet restore
## 4️⃣ Crear carpeta de datos (si es necesario)
mkdir -p ../data
## 5️⃣ Aplicar migraciones
dotnet ef database update
## 6️⃣ Ejecutar el proyecto
dotnet run
