using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using VanillaModding.Content.Rarities;

namespace VanillaModding.Content.Items.Materials
{
    internal class LogodomyShard : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(gold: 1, silver: 50);
            Item.rare = ModContent.RarityType<BrightTurquoise>();
        }
    }
}
