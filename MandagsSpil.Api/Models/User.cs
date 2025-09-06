using System;
using Microsoft.AspNetCore.Identity;

namespace MandagsSpil.Api.Models;

public class User
{
    public long UserId { get; set; }
    public string? Cod2Username { get; set; }
    public Guid IdentityId { get; set; }

    public ICollection<PlayerResult> PlayerResults { get; set; } = new List<PlayerResult>();
}
