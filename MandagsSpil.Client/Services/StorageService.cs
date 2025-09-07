using System;
using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace MandagsSpil.Client.Services;

public class StorageService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly string _userNameKey = "username";
    private readonly string _developerKey = "developer";

    public StorageService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task SetUserNameAsync(string userName)
    {
        await _localStorageService.SetItemAsync(_userNameKey, userName);
    }

    public async Task<string?> GetUserNameAsync()
    {
        var result = await _localStorageService.GetItemAsync<string>(_userNameKey);
        if(string.IsNullOrWhiteSpace(result))
            return "Unknown Soldier";

        return result;
    }

    public async Task RemoveUserNameAsync()
    {
        await _localStorageService.RemoveItemAsync(_userNameKey);
    }

    public async Task SetDeveloperModeAsync(bool isDeveloper)
    {
        await _localStorageService.SetItemAsync(_developerKey, isDeveloper);
    }

    public async Task<bool> GetDeveloperModeAsync()
    {
        var result = await _localStorageService.GetItemAsync<string>(_developerKey);
        if (string.IsNullOrWhiteSpace(result))
            return false;

        return bool.TryParse(result, out var isDev) && isDev;
    }
}