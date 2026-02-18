using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Materials
{
    internal class WhiteThread : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.buyPrice(gold: 2, silver: 20);
            Item.rare = ItemRarityID.White;
        }
    }
}
