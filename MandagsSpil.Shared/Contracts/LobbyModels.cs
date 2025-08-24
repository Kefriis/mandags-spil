using System;
using MandagsSpil.Shared.Contracts;

namespace MandagsSpil.Shared.Contracts;

public record ClassInfo(ClassNameEnum Name, List<string> Weapons, int MaxPlayers = 1);

public record PlayerInfo(string UserName, NationEnum Nation, ClassNameEnum SelectedClass, Guid Id, string? ConnectionId = null);

public record LobbyStateDto(NationEnum Nation, List<PlayerInfo> Players, List<ClassInfo> Classes);

public enum ClassNameEnum
{
    Unknown,
    Rifleman,
    Support,
    Sniper,
    Engineer
}