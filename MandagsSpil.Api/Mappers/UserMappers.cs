using System;
using MandagsSpil.Api.Models;
using MandagsSpil.Shared.Contracts.Identity;

namespace MandagsSpil.Api.Mappers;

public static class UserMappers
{
    public static UserDto ToDto(this User user)
    {
        var result = new UserDto
        {
            UserId = user.UserId,
            Cod2Username = user.Cod2Username
        };

        return result;
    }

    public static User FromDto(this UserDto dto)
    {
        var result = new User
        {
            UserId = dto.UserId,
            Cod2Username = dto.Cod2Username
        };

        return result;
    }
}
