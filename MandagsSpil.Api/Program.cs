using MandagsSpil.Api;
using MandagsSpil.Api.Hubs;
using MandagsSpil.Api.Models;
using MandagsSpil.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using MandagsSpil.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthorizationBuilder();
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseInMemoryDatabase("AppDb");
        //For debugging only: options.EnableDetailedErrors(true);
        //For debugging only: options.EnableSensitiveDataLogging(true);
    });

builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<LobbyStateService>();

builder.Services.AddSignalR();

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