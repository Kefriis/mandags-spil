using System;
using Blazored.LocalStorage;
using MandagsSpil.Shared.Contracts.Identity;
using Microsoft.JSInterop;

namespace MandagsSpil.Client.Services;

public class StorageService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly string _userNameKey = "username";
    private readonly string _developerKey = "developer";
    private readonly string _userInfoKey = "user_info";

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
        if (string.IsNullOrWhiteSpace(result))
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

    public async Task<Guid?> GetUserIdAsync()
    {
        var user = await _localStorageService.GetItemAsync<UserDto>(_userInfoKey);

        if (user is null || user.IdentityId is null)
            return null;

        return user.IdentityId;
    }

    public async Task SetUserInfoAsync(UserDto user)
    {
        await _localStorageService.SetItemAsync(_userInfoKey, user);
    }

    public async Task<UserDto?> GetUserInfoAsync()
    {
        return await _localStorageService.GetItemAsync<UserDto>(_userInfoKey);
    }
    
    public async Task RemoveUserInfoAsync()
    {
        await _localStorageService.RemoveItemAsync(_userInfoKey);
    }
}