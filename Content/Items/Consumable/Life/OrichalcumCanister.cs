using Microsoft.Xna.Framework;
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
    internal class OrichalcumCanister : ModItem
    {
        public static readonly int LifePerFruit = MythrilCanister.LifePerFruit;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            int width = 30; int height = 18;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            //Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 2, silver: 80);
            Item.rare = ItemRarityID.LightRed;
            //Item.expert = true;

            Item.UseSound = SoundID.Item2;
            //Item.CloneDefaults(ItemID.LifeFruit);
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
            if (player.GetModPlayer<VanillaModdingPlayer>().DiamondHeart >= player.GetModPlayer<VanillaModdingPlayer>().MaxDiamondHeart)
            {
                // Returning null will make the item not be consumed
                return null;
            }

            // This method handles permanently increasing the player's max health and displaying the green heal text
            player.UseHealthMaxIncreasingItem(LifePerFruit);

            // This field tracks how many of the example fruit have been consumed
            player.GetModPlayer<VanillaModdingPlayer>().DiamondHeart++;

            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.OrichalcumBar, 20)
                .AddIngredient(ItemID.Diamond, 5)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
