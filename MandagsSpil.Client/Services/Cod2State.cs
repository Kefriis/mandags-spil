using System;

namespace MandagsSpil.Client.Services;

public class Cod2State
{
    public Guid Id { get; } = Guid.NewGuid();
    public string UserName { get; set; } = "Unknown Soldier"; // Default to "Unknown User"
    public string GameMode { get; set; } = "Lobby"; // Default to Lobby mode
    public bool isDeveloperMode { get; set; } = false;

    public event Action? OnStateChanged;

    public void SetUserName(string userName)
    {
        UserName = userName;
        OnStateChanged?.Invoke();
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
        OnStateChanged?.Invoke();
    }

    public void SetDeveloperMode(bool isDev)
    {
        isDeveloperMode = isDev;
        OnStateChanged?.Invoke();
    }
}