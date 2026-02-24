using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Accessories.Clicker
{
    internal class Mouse : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 24;

            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 5);
        }
        // This is the main hook that allows for our info display to actually work with this accessory. 
        public override void UpdateInfoAccessory(Player player)
        {
            player.GetModPlayer<VanillaModdingPlayer>().showClicksTotal = true;
        }
    }
}
