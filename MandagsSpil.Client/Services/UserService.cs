using System.Net.Http.Json;
using Blazored.LocalStorage;
using MandagsSpil.Shared.Contracts.Identity;

namespace MandagsSpil.Client.Services;

public class UserService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly StorageService _storageService;

    public UserService(IHttpClientFactory httpClientFactory, StorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _storageService = storageService;
    }

    public async Task<(bool Success, string? Message)> CreateUser(UserDto userDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Auth");
        var response = await httpClient.PostAsJsonAsync("/user/create", userDto);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return (false, "Not found");

        if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
            return (false, "User record already exists.");

        if (response.IsSuccessStatusCode)
        {
            var newUser = await response.Content.ReadFromJsonAsync<UserDto>();

            if (newUser is null)
                return (false, "Error creating user");

            await _storageService.SetUserInfoAsync(newUser);
            await _storageService.SetUserNameAsync(newUser.Cod2Username ?? "Unknown Soldier");

            return (true, null);
        }

        return (false, "Error creating user");
    }

    public async Task<UserDto?> GetUserAsync()
    {
        var userInfo = await _storageService.GetUserInfoAsync();

        if (userInfo is not null)
            return userInfo;

        var httpClient = _httpClientFactory.CreateClient("Auth");

        var remoteUserInfoResponse = await httpClient.GetAsync("/user/info");

        if (remoteUserInfoResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (remoteUserInfoResponse.IsSuccessStatusCode)
        {
            var remoteUserInfo = await remoteUserInfoResponse.Content.ReadFromJsonAsync<UserDto>();

            if (remoteUserInfo is null)
                return null;

            await _storageService.SetUserInfoAsync(remoteUserInfo);
            await _storageService.SetUserNameAsync(remoteUserInfo.Cod2Username ?? "Unknown Soldier");

            return remoteUserInfo;
        }

        return null;
    }

    public async Task<bool> RemoveUser()
    {
        await _storageService.RemoveUserInfoAsync();
        await _storageService.RemoveUserNameAsync();

        return true;
    }

    public async Task<bool> UpdateUser(UserDto userDto)
    {
        var httpClient = _httpClientFactory.CreateClient("Auth");
        var response = await httpClient.PutAsJsonAsync("/user/update", userDto);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;


        if (response.IsSuccessStatusCode)
        {
            var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();

            if (updatedUser is null)
                return false;

            await _storageService.SetUserInfoAsync(updatedUser);
            await _storageService.SetUserNameAsync(updatedUser.Cod2Username ?? "Unknown Soldier");

            return true;
        }

        return true;
    }
}

public interface IUserService
{
    Task<UserDto?> GetUserAsync();
    Task<bool> UpdateUser(UserDto userDto);
    Task<(bool Success, string? Message)> CreateUser(UserDto userDto);
    Task<bool> RemoveUser();
}