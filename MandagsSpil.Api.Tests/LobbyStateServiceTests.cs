using MandagsSpil.Api.Services;
using MandagsSpil.Shared.Contracts;
using NUnit.Framework;

namespace MandagsSpil.Api.Tests;

public class LobbyStateServiceTests
{

    [Test]
    public void AssingPlayerToRifleman()
    {
        var ti = new LobbyStateService();

        ti.JoinLobby("user1", NationEnum.USA, Guid.NewGuid(), "conn1");
        ti.JoinLobby("user2", NationEnum.USA, Guid.NewGuid(), "conn2");

        var state = ti.GetLobbyState(NationEnum.USA);

        Assert.That(state.Players.Count, Is.EqualTo(2));

        var player1 = state.Players[0];
        var result = ti.SelectClass(player1.UserName, player1.Nation, ClassNameEnum.Rifleman, player1.Id);

        Assert.That(result.success, Is.True);
        Assert.That(result.message, Is.Null);

        state = ti.GetLobbyState(NationEnum.USA);

        Assert.That(state.Players[0].SelectedClass, Is.EqualTo(ClassNameEnum.Rifleman));
    }

    [Test]
    public void AssingPlayerToFullClass()
    {
        var ti = new LobbyStateService();

        for (int i = 0; i < 8; i++)
        {
            ti.JoinLobby("user" + i, NationEnum.USA, Guid.NewGuid(), "conn" + i);
        }

        var state = ti.GetLobbyState(NationEnum.USA);

        Assert.That(state.Players.Count, Is.EqualTo(8));

        for (int i = 0; i < 8; i++)
        {
            var player = state.Players[i];
            var result = ti.SelectClass(player.UserName, player.Nation, ClassNameEnum.Rifleman, player.Id);

            Assert.That(result.success, Is.True);
            Assert.That(result.message, Is.Null);
        }

        state = ti.GetLobbyState(NationEnum.USA);

        for (int i = 0; i < 8; i++)
        {
            Assert.That(state.Players[i].SelectedClass, Is.EqualTo(ClassNameEnum.Rifleman));
        }

        // Now try to add one more to the full class
        var extraPlayerId = Guid.NewGuid();
        ti.JoinLobby("extraUser", NationEnum.USA, extraPlayerId, "connExtra");
        var extraPlayer = ti.GetPlayers(NationEnum.USA).First(p => p.Id == extraPlayerId);
        var extraResult = ti.SelectClass(extraPlayer.UserName, extraPlayer.Nation, ClassNameEnum.Rifleman, extraPlayer.Id);

        Assert.That(extraResult.success, Is.False);
        Assert.That(extraResult.message, Is.EqualTo("Class Rifleman is full. Max players: 8"));
    }
}
