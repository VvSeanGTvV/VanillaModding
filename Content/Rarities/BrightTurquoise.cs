using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Rarities
{
    internal class BrightTurquoise : ModRarity
    {
        public override Color RarityColor => new Color(43, 255, 188);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
            {
                return ItemRarityID.Red;
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
}
