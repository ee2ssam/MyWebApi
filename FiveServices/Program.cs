using FiveServices.Data;
using FiveServices.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Render 등 클라우드 플랫폼에서 PORT 환경 변수를 제공할 때 해당 포트로 바인딩
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// 리버스 프록시(Render 등) 뒤에서 올바른 HTTPS 프로토콜 및 클라이언트 IP를 인식하도록 설정
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MyGameDB")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'MyGameDB' not found.");

builder.Services.AddDbContext<MyGameDbContext>(options =>
    options.UseNpgsql(connectionString));

// HttpClient 및 PingService 등록 (싱글톤 및 백그라운드 서비스)
builder.Services.AddHttpClient();
builder.Services.AddSingleton<PingService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PingService>());

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 리버스 프록시 헤더 적용 (반드시 파이프라인 최상단에 위치)
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// DB Connection Health Check endpoint
app.MapGet("/api/db-check", async (MyGameDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = canConnect ? "Success" : "Failed",
            message = canConnect ? "Successfully connected to Supabase (MyGameDB)!" : "Could not connect to database."
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, title: "Database Connection Error");
    }
});

// Render Health Check endpoint (경량 헬스체크용)
app.MapGet("/healthz", () => Results.Ok("OK"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
