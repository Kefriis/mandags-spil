using System;

namespace MandagsSpil.Api.Models;

public class PlayerResult
{
    public long PlayerResultId { get; set; }
    public string? Cod2Username { get; set; }
    public int Score { get; set; }
    public int Deaths { get; set; }
    public int Ping { get; set; }

    public long GameId { get; set; }
    public Game? Game { get; set; }
    public Nation Nation { get; set; }

        // Foreign Key Property
    // This links a PlayerResult to a User
    public long UserId { get; set; }

    // Reference Navigational Property
    // This allows you to access the User object from a PlayerResult
    public User? User { get; set; }
}

public enum Nation
{
    USA,
    UK,
    Germany,
    USSR,
    Unknown
}
