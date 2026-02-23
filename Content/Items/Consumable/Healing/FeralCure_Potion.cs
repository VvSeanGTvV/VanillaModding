using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Consumable.Healing
{
    internal class FeralCure_Potion : ModItem
    {
        public override void SetDefaults()
        {
            int width = 20; int height = 26;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            //Item.potionDelay = 2100;
            //Item.potion = true;

            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.Green;

            Item.UseSound = SoundID.Item3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.BatBat, 1)
                .AddIngredient(ItemID.Bottle, 1)
                .AddTile(TileID.Bottles)
                .Register();

            CreateRecipe(2)
                .AddIngredient(ItemID.BatBat, 1)
                .AddIngredient(ItemID.Bottle, 1)
                .AddTile(TileID.AlchemyTable)
                .Register();

            CreateRecipe(5)
                .AddIngredient(ItemID.BatBat, 1)
                .AddIngredient(ItemID.Bottle, 1)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }

        public override bool? UseItem(Player player)
        {
            if (player.HasBuff(BuffID.Rabies)) player.ClearBuff(BuffID.Rabies);
            return base.UseItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.HasBuff(BuffID.Rabies)) return true;
            return false;
        }
    }
}
