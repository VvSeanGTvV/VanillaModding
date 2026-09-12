using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace VanillaModding.Common.Systems
{
    internal class VMGoreID
    {
        public static int LoadGore(string path)
            => ModContent.Find<ModGore>($"{nameof(VanillaModding)}/{path}").Type;

        public static readonly int EggCrack1 = LoadGore("Egg1");
        public static readonly int EggCrack2 = LoadGore("Egg2");
    }
}
