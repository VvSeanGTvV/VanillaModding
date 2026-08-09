using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class LesserRestoration_Potion : ModItem
    {
        public override void SetDefaults()
        {
            int width = 20; int height = 26;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.healLife = 50;
            Item.healMana = 50;
            Item.potionDelay = Item.restorationDelay;
            Item.potion = true;

            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;

            Item.UseSound = SoundID.Item3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.LesserHealingPotion, 1)
                .AddIngredient(ItemID.LesserManaPotion, 1)
                .AddTile(TileID.Bottles)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LesserHealingPotion, 1)
                .AddIngredient(ItemID.LesserManaPotion, 1)
                .AddTile(TileID.AlchemyTable)
                .Register();
        }
    }
}
