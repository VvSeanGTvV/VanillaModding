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
            => new($"{nameof(VanillaModding)}/Assets/Sounds/{path}");

        public static readonly SoundStyle DeathNoteItemAsylum = LoadSound("DICE/DeathNote_ItemAsylum") with { Volume = 0.25f };
        public static readonly SoundStyle FishSpeak = LoadSound("Fish/Fish") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle FishHit = LoadSound("Fish/FishHit") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle FishSpeeen = LoadSound("Fish/FishSpeeen") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle FishSpeeenShort = LoadSound("Fish/FishSpeeenShort") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle Whirr = LoadSound("DeliveryDrone/DroneWhirr") with { Volume = 0.05f, PitchVariance = 0f, MaxInstances = 25 };

        // LOBOTOMY SOUNDS (GD)
        public static readonly SoundStyle FireInTheHole = LoadSound("Lobotomy/FireInTheHole") with { Volume = 0.25f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle FireInTheHoleHigh = LoadSound("Lobotomy/FireintheHole_HIGH") with { Volume = 0.25f, Pitch = 0.25f, PitchVariance = 0.25f, MaxInstances = 25 };
        public static readonly SoundStyle ExtremeDemonFire = LoadSound("Lobotomy/LobotomyExtremeDemonFire") with { Volume = 0.25f, Pitch = 1f, MaxInstances = 25 };
        public static readonly SoundStyle ExtremeDemonFire2 = LoadSound("Lobotomy/LobotomyExtremeDemonFire2") with { Volume = 0.25f, Pitch = 0.985f, MaxInstances = 25 };
        public static readonly SoundStyle LobotomyLaserFire = LoadSound("Lobotomy/LobotomyLaserFire") with { Volume = 0.25f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle LobotomyEasySpell = LoadSound("Lobotomy/LobotomyEasySpell") with { Volume = 0.45f, Pitch = -0.5f, MaxInstances = 25 };
        public static readonly SoundStyle LobotomyEasy = LoadSound("Lobotomy/LobotomyEasy") with { Volume = 0.45f, Pitch = -0.5f, MaxInstances = 25 };
        public static readonly SoundStyle LobotomyEasyBounce = LoadSound("Lobotomy/LobotomyEasyBounce") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };
        public static readonly SoundStyle LobotomyInsane = LoadSound("Lobotomy/LobotomyInsaneBeam") with { Volume = 0.45f, Pitch = 0f, MaxInstances = 25 };

        public static readonly SoundStyle Hallelujah = LoadSound("Bombs/HolyHandGrenadeHallelujah");
    }
}
