using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Content.Items.Materials;

namespace VanillaModding.Content.Items.Consumable.Life
{
    internal class HallowedCanister : ModItem
    {
        public static readonly int LifePerFruit = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            int width = 30; int height = 18;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 30;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 5, silver: 80);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item4;
        }

        public override bool CanUseItem(Player player)
        {
            // This check prevents this item from being used before vanilla health upgrades are maxed out.
            return player.ConsumedLifeCrystals >= Player.LifeCrystalMax && player.ConsumedLifeFruit >= Player.LifeFruitMax;
        }

        public override bool? UseItem(Player player)
        {
            // Moving the exampleLifeFruits check from CanUseItem to here allows this example fruit to still "be used" like Life Fruit can be
            // when at the max allowed, but it will just play the animation and not affect the player's max life
            if (player.GetModPlayer<VanillaModdingPlayer>().EctoHeart >= player.GetModPlayer<VanillaModdingPlayer>().MaxEctoHeart)
            {
                // Returning null will make the item not be consumed
                return null;
            }
            player.UseHealthMaxIncreasingItem(LifePerFruit);
            player.GetModPlayer<VanillaModdingPlayer>().EctoHeart++;

            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LifeFruit, 1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddIngredient<SoulofUnity>(5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
