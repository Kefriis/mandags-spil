using System;
using MandagsSpil.Shared.Contracts;

namespace MandagsSpil.Client.Extensions;

public static class StringExtensions
{
    public static NationEnum ToNationEnum(this string? nation)
    {
        var result = Enum.Parse<NationEnum>(nation ?? "Unknown", true);
        
        return Enum.IsDefined(typeof(NationEnum), result) ? result : NationEnum.Unknown;
    }
     
}
