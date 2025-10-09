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
        /// <param name="path"></param>
        /// <returns>SoundStyle from Assets/Sounds/{path}.ogg </returns>
        public static SoundStyle LoadSound(string path) 
            => new SoundStyle($"{nameof(VanillaModding)}/Assets/Sounds/{path}");

        public static readonly SoundStyle DeathNoteItemAsylum = LoadSound("DICE/DeathNote_ItemAsylum") with { Volume = 0.25f };
        public static readonly SoundStyle Whirr = LoadSound("DeliveryDrone/DroneWhirr") with { Volume = 0.05f, PitchVariance = 0f, MaxInstances = 25 };
        public static readonly SoundStyle FireInTheHole = LoadSound("Lobotomy/FireInTheHole") with { Volume = 0.25f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle FireInTheHoleHigh = LoadSound("Lobotomy/FireintheHole_HIGH") with { Volume = 0.25f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle Hallelujah = LoadSound("Bombs/HolyHandGrenadeHallelujah");
    }
}
