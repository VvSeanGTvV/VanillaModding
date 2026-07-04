using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Tiles;

namespace VanillaModding.Content.Items.Placeable
{
    internal class LogodomyAltar : ModItem
    {
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Example Pylon tile.
            Item.DefaultToPlaceableTile(ModContent.TileType<LogodomyAltarTile>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(copper: 1));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 20)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
