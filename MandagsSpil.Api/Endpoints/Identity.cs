using System;
using System.Security.Claims;
using System.Text;
using MandagsSpil.Api.Extensions;
using MandagsSpil.Api.Interfaces;
using MandagsSpil.Api.Models;
using MandagsSpil.Shared.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace MandagsSpil.Api.Endpoints;

internal static class IdentityEndpoints
{

    internal static void MapCustomIdentityEndpoints(this WebApplication app)
    {
        app.MapPost("/logout", async (SignInManager<AppUser> signInManager, [FromBody] object empty) =>
        {
            if (empty is not null)
            {
                await signInManager.SignOutAsync();

                return Results.Ok();
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        app.MapGet("/roles", (ClaimsPrincipal user) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)user.Identity;
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c =>
                        new
                        {
                            c.Issuer,
                            c.OriginalIssuer,
                            c.Type,
                            c.Value,
                            c.ValueType
                        });

                return TypedResults.Json(roles);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        var group = app.MapGroup("/identity")
            .WithTags("Identity");

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            // The user ID is in the NameIdentifier claim.
            var userId = user.GetUserId();

            if (userId is null)
            {
                // This should not happen on an endpoint with .RequireAuthorization(), but it's good practice to check.
                return Results.Unauthorized();
            }

            // You can also get other claims like the username.
            return Results.Ok(new { id = userId, name = user.Identity?.Name });
        }).RequireAuthorization();

        group.MapPost("/forgotPassword", async (UserManager<AppUser> userManager, IEmailSender emailSender, [FromBody] ForgotPasswordModel model) =>
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is null)
                {
                    return Results.Ok();
                }

                app.Logger.LogInformation("User found");

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                app.Logger.LogInformation("Reset token generated");

                var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                await emailSender.SendResetPasswordEmailAsync(model.Email, validToken, model.CallBackUrl);

                return Results.Ok();
            }
            catch (System.Exception ex)
            {
                app.Logger.LogError(ex, "an error occured");
                throw;
            }

        });

        group.MapPost("/resetPassword", async (UserManager<AppUser> userManager, [FromBody] ResetPasswordModel model) =>
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return Results.NotFound();
            }

            var validToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var result = await userManager.ResetPasswordAsync(user, validToken, model.NewPassword);

            if (result.Succeeded)
                return Results.Ok(result);
                
            return Results.BadRequest(result);
        });
    }
}
