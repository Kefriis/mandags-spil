using System;

namespace MandagsSpil.Client.Services;

public class Weapon
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nation { get; set; } = string.Empty;
    public int Weight { get; set; }
}

public static class WeaponService
{
    static List<Weapon> americanWeapons = new List<Weapon>
    {
        new Weapon { Number = 1, Name = "Grease Gun", Nation = "American", Weight = 8 },
        new Weapon { Number = 2, Name = "M1A1", Nation = "American", Weight = 10 },
        new Weapon { Number = 3, Name = "M1 Garand", Nation = "American", Weight = 3 },
        new Weapon { Number = 4, Name = "Springfield", Nation = "American", Weight = 2},
        new Weapon { Number = 5, Name = "Trench Gun", Nation = "American", Weight = 1 },
        new Weapon { Number = 6, Name = "Thompson", Nation = "American", Weight = 4 },
        new Weapon { Number = 7, Name = "BAR", Nation = "American", Weight = 2 }
    };

    static List<Weapon> germanWeapons = new List<Weapon>
{
    new Weapon { Number = 1, Name = "MP40", Nation = "German", Weight = 8 },
    new Weapon { Number = 1, Name = "Sturmgewehr 44", Nation = "German", Weight = 3 },
    new Weapon { Number = 2, Name = "Kar98k", Nation = "German", Weight = 6 },
    new Weapon { Number = 3, Name = "Gewehr 43", Nation = "German", Weight = 9 }, // semi-auto, a bit shit
    new Weapon { Number = 5, Name = "Scoped Kar98k", Nation = "German", Weight = 2 },
    new Weapon { Number = 6, Name = "Shotgun", Nation = "German", Weight = 2 } // shotgun
};

    static List<Weapon> britishWeapons = new List<Weapon>
{
    new Weapon { Number = 1, Name = "Sten", Nation = "British", Weight = 8 },
    new Weapon { Number = 2, Name = "Lee-Enfield", Nation = "British", Weight = 6 },
    new Weapon { Number = 3, Name = "Bren", Nation = "British", Weight = 9 }, // semi-auto/LMG
    new Weapon { Number = 4, Name = "Scoped Lee-Enfield", Nation = "British", Weight = 3 },
    new Weapon { Number = 6, Name = "Thompson", Nation = "American", Weight = 2 },
    new Weapon { Number = 5, Name = "Shotgun", Nation = "British", Weight = 2 } // shotgun
};

    static List<Weapon> russianWeapons = new List<Weapon>
{
    new Weapon { Number = 1, Name = "PPSH-41", Nation = "Russian", Weight = 8 },
    new Weapon { Number = 2, Name = "PPS-42", Nation = "Russian", Weight = 8 },
    new Weapon { Number = 3, Name = "Mosin-Nagant", Nation = "Russian", Weight = 6 },
    new Weapon { Number = 4, Name = "SVT-40", Nation = "Russian", Weight = 5 }, // semi-auto, a bit shit
    new Weapon { Number = 5, Name = "Scoped Mosin-Nagant", Nation = "Russian", Weight = 2 },
    new Weapon { Number = 6, Name = "Shotgun", Nation = "Russian", Weight = 1 } // shotgun
};
    public static List<Weapon> GetWeaponsByNation(string nation)
    {
        return nation switch
        {
            "american" => americanWeapons,
            "us" => americanWeapons,
            "german" => germanWeapons,
            "de" => germanWeapons,
            "british" => britishWeapons,
            "uk" => britishWeapons,
            "russian" => russianWeapons,
            "ru" => russianWeapons,
            _ => new List<Weapon>()
        };
    }
}
