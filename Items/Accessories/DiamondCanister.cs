using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using VanillaModding.Items.SoulofEssence;
using Terraria.Localization;

namespace VanillaModding.Items.Accessories
{
    internal class DiamondCanister : ModItem
    {

        
        int Increasement = 75;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Increasement);
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 18;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.HallowedBar, 5);
            recipe1.AddIngredient(ModContent.ItemType<HeartCanister>(), 1);
            recipe1.AddIngredient(ItemID.Diamond, 3);
            recipe1.AddTile(TileID.MythrilAnvil);
            recipe1.Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += Increasement;
            hideVisual = true;
            // Set the HasExampleImmunityAcc bool to true to ensure we have this accessory
            // And apply the changes in ModPlayer.PostHurt correctly
            //player.
            //player.GetModPlayer<ExampleImmunityPlayer>().HasExampleImmunityAcc = true;
        }
    }
}
