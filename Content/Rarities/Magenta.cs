using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Rarities
{
    internal class Magenta : ModRarity
    {
        public override Color RarityColor => new Color(255, 0, 255);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
            {
                return ItemRarityID.Red; // Make the rarity of items that have this rarity with a positive modifier the higher tier one.
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
    }
}
