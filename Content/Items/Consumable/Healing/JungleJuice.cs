using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class JungleJuice : ModItem
    {
        public override void SetDefaults()
        {
            int width = 20; int height = 26;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.healLife = 180;
            Item.potion = true;

            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 2, silver: 10);
            Item.rare = ItemRarityID.Lime;

            Item.UseSound = SoundID.Item3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.GreaterHealingPotion, 3)
                .AddIngredient(ItemID.LifeFruit, 1)
                .AddTile(TileID.Bottles)
                .Register();

            CreateRecipe(3)
                .AddIngredient(ItemID.GreaterHealingPotion, 3)
                .AddIngredient(ItemID.LifeFruit, 1)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}
