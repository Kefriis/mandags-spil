using MandagsSpil.Api;
using MandagsSpil.Api.Hubs;
using MandagsSpil.Api.Models;
using MandagsSpil.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MandagsSpil.Api.Endpoints;
using MandagsSpil.Api.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.SetMinimumLevel(LogLevel.Information);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorizationBuilder();
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetValue<string>("DefaultConnection"));
        //options.UseInMemoryDatabase("AppDb");
    });

builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.Configure<MailOptions>(
    builder.Configuration.GetSection(MailOptions.Mail));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

builder.Services.AddSingleton<LobbyStateService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});


var app = builder.Build();

app.MapGet("/cod2/classes", (LobbyStateService lobbyStateService) => lobbyStateService.ClassesByNation);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// create routes for the identity endpoints
app.MapIdentityApi<AppUser>();

app.MapCustomIdentityEndpoints();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();

app.MapHub<LobbyHub>("/lobbyhub");

app.Run();