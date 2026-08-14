using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class ResurrectionPotion : ModItem
    {
        public override void SetDefaults()
        {
            int width = 24; int height = 36;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Yellow;

            Item.UseSound = SoundID.Item3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GreaterHealingPotion, 1)
                .AddIngredient(ItemID.GuideVoodooDoll, 1)
                .AddIngredient(ItemID.SoulofMight, 3)
                .AddTile<Tiles.Furniture.FusionCore>()
                .Register();
        }
    }
}
