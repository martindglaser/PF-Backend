
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore ./src/AnalyzerGateway.Api/AnalyzerGateway.Api.csproj
RUN dotnet publish ./src/AnalyzerGateway.Api/AnalyzerGateway.Api.csproj -c Release -o /app


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

CMD ["dotnet", "AnalyzerGateway.Api.dll"]
