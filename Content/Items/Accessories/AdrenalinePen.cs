using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Content.Items.Consumable;

namespace VanillaModding.Content.Items.Accessories
{
    internal class AdrenalinePen : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(8, 24);

            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Pink;

            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<AdrenalinePotion>(), 3)
                .AddIngredient(ItemID.HallowedBar, 3)
                .AddIngredient(ItemID.MechanicalBatteryPiece, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            VanillaModdingPlayer VMP = player.GetModPlayer<VanillaModdingPlayer>();
            if (VMP != null) VMP.accEpipen = true;
        }
    }
}
