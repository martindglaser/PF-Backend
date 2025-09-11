using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.Services;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();

app.UseSwagger();     // Swashbuckle
app.UseSwaggerUI();   // Swashbuckle

app.MapControllers();

app.Run();
