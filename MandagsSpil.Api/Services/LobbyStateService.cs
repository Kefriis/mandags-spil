using System.Collections.Concurrent;
using MandagsSpil.Shared.Contracts;

namespace MandagsSpil.Api.Services;

public class LobbyStateService
{
    private readonly ConcurrentDictionary<NationEnum, List<PlayerInfo>> _playersByNation = new();

    private readonly Dictionary<NationEnum, List<ClassInfo>> _classesByNation = new()
    {
        [NationEnum.USA] = new()
        {
            new ClassInfo("Rifleman", new List<string> { "M1 Garand", "M1A1 Carbine" }),
            new ClassInfo("Support Gunner", new List<string> { "BAR (Browning Automatic Rifle)" }),
            new ClassInfo("Sniper", new List<string> { "Springfield" }),
            new ClassInfo("Engineer", new List<string> { "Thompson", "Grease Gun", "Shot gun" })
        },
        [NationEnum.UK] = new()
        {
            new ClassInfo("Rifleman", new List<string> { "Lee-Enfield"}),
            new ClassInfo("Support Gunner", new List<string> { "Bren" }),
            new ClassInfo("Sniper", new List<string> { "Scoped Lee-Enfield" }),
            new ClassInfo("Engineer", new List<string> { "Sten", "Shot gun" })
        },
        [NationEnum.Germany] = new()
        {
            new ClassInfo("Rifleman", new List<string> { "Kar98k", "Gewehr 43" }),
            new ClassInfo("Support Gunner", new List<string> { "MP44" }),
            new ClassInfo("Sniper", new List<string> { "Scoped Kar98k" }),
            new ClassInfo("Engineer", new List<string> { "MP40", "Shot gun" })
        },
        [NationEnum.USSR] = new()
        {
            new ClassInfo("Rifleman", new List<string> { "Mosin-Nagant", "SVT-40" }),
            new ClassInfo("Support Gunner", new List<string> { "PPSH" }),
            new ClassInfo("Sniper", new List<string> { "Scoped Mosin-Nagant" }),
            new ClassInfo("Engineer", new List<string> { "PPS-42", "Shot gun" })
        }
    };

    public IReadOnlyDictionary<NationEnum, List<ClassInfo>> ClassesByNation => _classesByNation;

    public List<PlayerInfo> GetPlayers(NationEnum nation) =>
        _playersByNation.TryGetValue(nation, out var players) ? players : new List<PlayerInfo>();

    public void JoinLobby(string userName, NationEnum nation, Guid playerId, string connectionId)
    {
        var players = _playersByNation.GetOrAdd(nation, _ => new List<PlayerInfo>());

        lock (players)
        {
            if (!players.Any(p => p.Id == playerId))
            {
                players.Add(new PlayerInfo(userName, nation, null, playerId, connectionId));
            }
        }
    }

    public void LeaveLobby(string userName, NationEnum nation, Guid playerId)
    {
        if (_playersByNation.TryGetValue(nation, out var players))
        {
            lock (players)
            {
                var player = players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                    players.Remove(player);
            }
        }
    }

    public NationEnum LeaveLobby(string connectionId)
    {
        foreach (var nation in _playersByNation.Keys)
        {
            if (_playersByNation.TryGetValue(nation, out var players))
            {
                lock (players)
                {
                    var player = players.FirstOrDefault(p => p.ConnectionId == connectionId);
                    if (player != null)
                    {
                        players.Remove(player);
                        return nation;
                    }
                }
            }
        }

        return NationEnum.Unknown;
    }

    public void SelectClass(string userName, NationEnum nation, string className, Guid playerId)
    {
        if (_playersByNation.TryGetValue(nation, out var players))
        {
            lock (players)
            {
                var playerIndex = players.FindIndex(p => p.Id == playerId);
                if (playerIndex >= 0)
                {
                    var player = players[playerIndex];
                    bool classExists = _classesByNation.TryGetValue(nation, out var classes)
                        && classes.Any(c => c.Name == className);

                    if (classExists)
                    {
                        players[playerIndex] = player with { SelectedClass = className };
                    }
                }
            }
        }
    }

    public LobbyStateDto GetLobbyState(NationEnum nation)
    {
        var players = GetPlayers(nation);
        var classes = _classesByNation.TryGetValue(nation, out var result) ? result : new List<ClassInfo>();
        return new LobbyStateDto(nation, players, classes);
    }
}