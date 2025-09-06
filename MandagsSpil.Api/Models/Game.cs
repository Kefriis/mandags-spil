using System;

namespace MandagsSpil.Api.Models;

public class Game
{
    public long GameId { get; set; }
    public string? MapName { get; set; }
    public DateTime GameDate { get; set; }
    public string? WinningTeam { get; set; }
    public bool IsTie { get; set; }

    public ICollection<PlayerResult> PlayerResults { get; set; } = new List<PlayerResult>();
}