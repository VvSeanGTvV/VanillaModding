using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using VanillaModding.Common.Systems;

namespace VanillaModding.Common
{
    internal class VanillaModdingConditions
    {
        public static Condition DownedDuneTrapper = new("Mods.VanillaModding.Conditions.downedDuneTrapper", () => DownedBossSystem.downedDuneTrapper);
    }
}
