using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace MandagsSpil.Client.Identity;

public static class IdentityInjections
{
    public static WebAssemblyHostBuilder AddIdentityServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddTransient<CookieHandler>();

        // set up authorization
        builder.Services.AddAuthorizationCore();

        // register the custom state provider
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

        // register the account management interface
        builder.Services.AddScoped(
            sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

        builder.Services.AddHttpClient(
            "Auth",
            opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:7220"))
            .AddHttpMessageHandler<CookieHandler>();

        return builder;
    }
}
