using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(181);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
        new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
        {
            MustRevalidate = true,
            NoCache = true,
            NoStore = true,
        };

    string mainUrl = "https://observatory2.puresourcecode.com/";
#if DEBUG
    mainUrl = "https://localhost:7063";
#endif

    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Content-Security-Policy",
        $"default-src 'self' {mainUrl} 'unsafe-inline' 'unsafe-eval'; " +
        $"script-src 'unsafe-inline' 'unsafe-eval' {mainUrl}; " +
        "connect-src 'self'; " +
        $"img-src 'self' {mainUrl}; " +
        $"style-src 'self' {mainUrl}; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none';");
    context.Response.Headers.Add("Referrer-Policy", "same-origin");
    context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=()");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode_block");
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("SameSite", "Strict");

    return next.Invoke();
});

app.UseHttpsRedirection();
app.UseHsts();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
