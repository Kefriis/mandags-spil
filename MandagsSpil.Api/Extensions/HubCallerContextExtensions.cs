using System;
using Microsoft.AspNetCore.SignalR;

namespace MandagsSpil.Api.Extensions;

public static class HubCallerContextExtensions
{
    public static Guid GetUserId(this HubCallerContext context)
    {
        var userId = context.UserIdentifier;

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var guid))
        {
            throw new HubException("User is not authenticated or has an invalid ID");
        }

        return guid;
    }

}
