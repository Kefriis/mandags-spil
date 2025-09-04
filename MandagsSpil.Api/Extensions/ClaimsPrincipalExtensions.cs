using System;
using System.Security.Claims;

namespace MandagsSpil.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    { 
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if(userId != null)
        {
            return Guid.Parse(userId);
        }

        return null;
    }
}
