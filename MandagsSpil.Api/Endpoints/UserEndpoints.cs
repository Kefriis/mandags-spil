using System;
using System.Security.Claims;
using MandagsSpil.Api.Extensions;
using MandagsSpil.Api.Mappers;
using MandagsSpil.Api.Models;
using MandagsSpil.Api.Persistence;
using MandagsSpil.Shared.Contracts.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MandagsSpil.Api.Endpoints;

public static class UserEndpoints
{
    internal static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/user")
            .WithTags("User");

        group.MapGet("/info", async (ClaimsPrincipal user, [FromServices] PersistenceContext context) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = user.GetUserId();
                if (identity is null)
                    return Results.Unauthorized();

                var userInfo = await context.Users.FirstOrDefaultAsync(x => x.IdentityId == identity);

                if (userInfo == null)
                    return Results.NotFound();

                var mappedUser = userInfo.ToDto();
                mappedUser.IdentityId = userInfo.IdentityId;

                return Results.Ok(userInfo);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        group.MapPost("/create", async (ClaimsPrincipal user, [FromServices] PersistenceContext context, [FromBody] UserDto newUserDto) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = user.GetUserId();

                if (identity is null)
                    return Results.Unauthorized();

                // Check if a user with this identity already exists
                var existingUser = await context.Users.FirstOrDefaultAsync(u => u.IdentityId == identity);
                if (existingUser != null)
                {
                    return Results.Conflict("User record already exists.");
                }

                var newUser = new User
                {
                    Cod2Username = newUserDto.Cod2Username,
                    IdentityId = identity.Value
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                var mappedUser = newUser.ToDto();

                return Results.Created($"/user/info", mappedUser);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        // Updates the Cod2Username for the authenticated user.
        group.MapPut("/update", async (ClaimsPrincipal user, [FromServices] PersistenceContext context, [FromBody] UserDto updatedUserDto) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = user.GetUserId();
                if (identity is null)
                    return Results.Unauthorized();

                var userToUpdate = await context.Users.FirstOrDefaultAsync(u => u.IdentityId == identity);

                if (userToUpdate == null)
                {
                    return Results.NotFound("User record not found.");
                }

                userToUpdate.Cod2Username = updatedUserDto.Cod2Username;

                await context.SaveChangesAsync();

                var mappedUser = userToUpdate.ToDto();

                return Results.Ok(mappedUser);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();

        // Deletes the user record for the authenticated user.
        group.MapDelete("/delete", async (ClaimsPrincipal user, [FromServices] PersistenceContext context) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = user.GetUserId();
                if (identity is null)
                    return Results.Unauthorized();

                var userToDelete = await context.Users.FirstOrDefaultAsync(u => u.IdentityId == identity);

                if (userToDelete == null)
                {
                    return Results.NotFound("User record not found.");
                }

                context.Users.Remove(userToDelete);
                await context.SaveChangesAsync();

                return Results.NoContent();
            }

            return Results.Unauthorized();
        }).RequireAuthorization();
    }
}
