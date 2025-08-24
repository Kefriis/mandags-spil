using System;
using MandagsSpil.Shared.Contracts;

namespace MandagsSpil.Shared.Contracts;

public record ClassInfo(string Name, List<string> Weapons);

public record PlayerInfo(string UserName, NationEnum Nation, string? SelectedClass, Guid Id, string? ConnectionId = null);

public record LobbyStateDto(NationEnum Nation, List<PlayerInfo> Players, List<ClassInfo> Classes);