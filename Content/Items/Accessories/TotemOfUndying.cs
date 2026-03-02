using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Accessories
{
    internal class TotemOfUndying : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<VanillaModdingPlayer>().accTotem = true;
            player.GetModPlayer<VanillaModdingPlayer>().totem = Item.type;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.SoulofLight, 1)
                .AddIngredient(ItemID.Emerald, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
