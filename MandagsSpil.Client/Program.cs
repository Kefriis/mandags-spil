using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MandagsSpil.Client;
using MudBlazor.Services;
using MandagsSpil.Client.Layout.AppbarComponents;
using MudBlazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using MandagsSpil.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AppbarState>();

builder.Services.AddMudServices(options =>
{
    options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
});

builder.Services.AddPWAUpdater();

builder.Services.AddSingleton<Cod2State>();
builder.Services.AddSingleton<StorageService>();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
