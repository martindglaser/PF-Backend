# Pasos para la instalacion del proyecto
git clone https://github.com/tuusuario/tu-repo.git
cd tu-repo/src/AnalyzerGateway.Api

# (si elegiste local tools)
dotnet tool restore

# restaurar paquetes
dotnet restore

# crear carpeta data si usás una ruta relativa (si tu Program.cs no la crea)
mkdir -p ../data

# aplicar migraciones (usa la herramienta restore o global)
dotnet ef database update

# correr
dotnet run
