using ShopMgmt.Web.Components;
using ShopMgmt.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7120/";

void ConfigureClient(IHttpClientBuilder b) =>
    b.ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                builder.Environment.IsDevelopment()
                    ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    : null
        });

ConfigureClient(builder.Services.AddHttpClient<MaterialApiClient>());
ConfigureClient(builder.Services.AddHttpClient<CategoryApiClient>());
ConfigureClient(builder.Services.AddHttpClient<StockBatchApiClient>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
