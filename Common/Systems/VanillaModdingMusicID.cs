using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace VanillaModding.Common.Systems
{
    internal class VanillaModdingMusicID
    {
        /// <summary>
        /// Loads specific sounds from the mod's Assets folder.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>MusicSlotID from Assets/Music/{path}.ogg or .mp3 </returns>
        public static int LoadMusic(string path) 
            => MusicLoader.GetMusicSlot($"{nameof(VanillaModding)}/Assets/Music/{path}");

        public static readonly int LobotomyGod = LoadMusic("BloomGod");
        public static readonly int GettingSandy = LoadMusic("GettingSandy");
        public static readonly int NowhereHome = LoadMusic("NowhereHome");
        public static readonly int Boss1Mindustry = LoadMusic("Boss1");
        public static readonly int Ocram = LoadMusic("Ocram");
        public static readonly int TheChosenOne = LoadMusic("TheChosenOne");
    }
}
