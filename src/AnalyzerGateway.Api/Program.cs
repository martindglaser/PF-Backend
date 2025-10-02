using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// EF Core
var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));

// HttpClient + Polly
builder.Services.AddHttpClient<AnalysisClient>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    http.BaseAddress = new Uri(cfg["AnalysisApi:BaseUrl"]!);
    http.Timeout = TimeSpan.FromSeconds(int.TryParse(cfg["AnalysisApi:TimeoutSeconds"], out var t) ? t : 30);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => (int)r.StatusCode == 429)
    .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * i)));

builder.Services.AddScoped<AnalysisService>();

builder.Services.AddControllers();

// Estas dos líneas requieren los paquetes correctos:
builder.Services.AddEndpointsApiExplorer();   // (viene con Microsoft.AspNetCore.OpenApi en .NET 8)
builder.Services.AddSwaggerGen();            // (viene de Swashbuckle.AspNetCore)

// Agregar servicios CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirMiOrigen",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // origen habilitado
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();


// Ruta física: 
//assets  (carpeta hermana a backend y frontend)
var assetsPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../../", "assets"));
if (!Directory.Exists(assetsPath))
{
    Console.WriteLine($"[WARN] No existe la carpeta de assets: {assetsPath}");
}

// Servir estáticos en /assets
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(assetsPath),
    RequestPath = "/assets",
    OnPrepareResponse = ctx =>
    {
        // CORS sólo necesario si vas a usar fetch/canvas; para <img> no hace falta
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:5173");
    }
});


app.UseCors("PermitirMiOrigen");

app.UseSwagger();     // Swashbuckle
app.UseSwaggerUI();   // Swashbuckle

app.MapControllers();

app.Run();
