using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Items.Consumable.Life
{
    internal class PlatinumCanister : ModItem
    {
        public static readonly int MaxPlatinumCanister = 10;
        public static readonly int LifePerFruit = 20;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit, MaxPlatinumCanister);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeFruit);
        }

        public override bool CanUseItem(Player player)
        {
            // This check prevents this item from being used before vanilla health upgrades are maxed out.
            return player.ConsumedLifeCrystals == Player.LifeCrystalMax && player.ConsumedLifeFruit == Player.LifeFruitMax;
        }

        public override bool? UseItem(Player player)
        {
            // Moving the exampleLifeFruits check from CanUseItem to here allows this example fruit to still "be used" like Life Fruit can be
            // when at the max allowed, but it will just play the animation and not affect the player's max life
            if (player.GetModPlayer<VanillaModdingPlayer>().PlatinumCanister >= MaxPlatinumCanister)
            {
                // Returning null will make the item not be consumed
                return null;
            }

            // This method handles permanently increasing the player's max health and displaying the green heal text
            player.UseHealthMaxIncreasingItem(LifePerFruit);

            // This field tracks how many of the example fruit have been consumed
            player.GetModPlayer<VanillaModdingPlayer>().PlatinumCanister++;

            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.PlatinumBar, 20);
            recipe.AddIngredient(ItemID.LifeFruit, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
