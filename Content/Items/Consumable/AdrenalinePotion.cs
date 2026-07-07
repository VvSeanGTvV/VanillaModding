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
using VanillaModding.Content.Buffs;

namespace VanillaModding.Content.Items.Consumable
{
    internal class AdrenalinePotion : ModItem
    {
        public override void SetDefaults()
        {
            int width = 20; int height = 26;
            Item.Size = new Vector2(width, height);

            Item.useTime = Item.useAnimation = 17;
            Item.useTurn = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.consumable = true;
            Item.noMelee = true;

            Item.value = Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.LightRed;

            Item.UseSound = SoundID.Item3;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.CrystalShard, 1)
                .AddIngredient(ItemID.Fireblossom, 2)
                .AddIngredient(ItemID.FallenStar, 1)
                .AddIngredient(ItemID.BottledWater, 1)
                .AddTile(TileID.Bottles)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.CrystalShard, 1)
                .AddIngredient(ItemID.Fireblossom, 2)
                .AddIngredient(ItemID.FallenStar, 1)
                .AddIngredient(ItemID.BottledWater, 1)
                .AddTile(TileID.AlchemyTable)
                .Register();

            CreateRecipe(2)
                .AddIngredient(ItemID.CrystalShard, 2)
                .AddIngredient(ItemID.Fireblossom, 4)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddIngredient(ItemID.BottledWater, 2)
                .AddTile(TileID.AlchemyTable)
                .Register();
        }

        public override bool? UseItem(Player player)
        {
            VanillaModdingPlayer VMP = player.GetModPlayer<VanillaModdingPlayer>();
            if (!player.HasBuff(ModContent.BuffType<Adrenaline>()))
            {
                player.AddBuff(ModContent.BuffType<Adrenaline>(), 60 * 15);
                VMP.Adrenaline = true;
            }
            return true;
        }
    }
}
