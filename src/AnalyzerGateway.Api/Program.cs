using AnalyzerGateway.Api.Data;
using AnalyzerGateway.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));

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


builder.Services.AddEndpointsApiExplorer();   
builder.Services.AddSwaggerGen();           


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirMiOrigen",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("[INFO] Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] Failed to apply migrations: {ex.Message}");
        throw;
    }
}

var assetsPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "../../../", "assets/screenshots"));
if (!Directory.Exists(assetsPath))
{
    Console.WriteLine($"[WARN] No existe la carpeta de assets: {assetsPath}");
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(assetsPath),
    RequestPath = "/assets/screenshots",
    OnPrepareResponse = ctx =>
    {
 
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});


app.UseCors("PermitirMiOrigen");

app.UseSwagger();    
app.UseSwaggerUI();   

app.MapControllers();

app.Run();
