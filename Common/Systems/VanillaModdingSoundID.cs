using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace VanillaModding.Common.Systems
{
    internal class VanillaModdingSoundID
    {
        /// <summary>
        /// Loads specific sounds from the mod's Assets folder.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns>SoundStyle from Assets/{type}/{path}.ogg </returns>
        public static SoundStyle LoadSound(string type, string path) 
            => new SoundStyle($"{nameof(VanillaModding)}/Assets/{type}/{path}");

        public static readonly SoundStyle DeathNoteItemAsylum = LoadSound("Sounds", "DICE/DeathNote_ItemAsylum") with { Volume = 0.25f };
        public static readonly SoundStyle Whirr = LoadSound("Sounds", "DeliveryDrone/DroneWhirr") with { Volume = 0.05f, PitchVariance = 0f, MaxInstances = 25 };
        public static readonly SoundStyle FireInTheHole = LoadSound("Sounds", "Lobotomy/FireInTheHole") with { Volume = 1f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle FireInTheHoleHigh = LoadSound("Sounds", "Lobotomy/FireintheHole_HIGH") with { Volume = 1f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle Hallelujah = LoadSound("Sounds", "Bombs/HolyHandGrenadeHallelujah");
    }
}
