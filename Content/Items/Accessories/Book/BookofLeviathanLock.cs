using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Accessories.Book
{
    internal class BookofLeviathanLock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.value = Item.sellPrice(gold: 0, silver: 50);
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
