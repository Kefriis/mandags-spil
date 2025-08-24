using System;
using Microsoft.JSInterop;

namespace MandagsSpil.Client.Services;

public class StorageService
{
    private readonly IJSRuntime _jsRuntime;

    public StorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetUserNameAsync(string userName)
    {
        await SetItemAsync("username", userName);
    }

    public async Task<string?> GetUserNameAsync()
    {
        var result = await GetItemAsync("username");
        if(string.IsNullOrWhiteSpace(result))
            return "Unknown Soldier";

        return result;
    }

    public async Task SetItemAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public async Task<string?> GetItemAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
    }
}