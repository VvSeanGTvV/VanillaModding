using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Tiles.Bars;

namespace VanillaModding.Content.Items.Materials.Bars
{
    internal class ElectrifiedBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
            ItemID.Sets.SortingPriorityMaterials[Type] = 59;
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(gold: 3, silver: 50);
            Item.rare = ItemRarityID.Yellow;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;

            Item.createTile = ModContent.TileType<ElectrifiedBarTile>();
            Item.placeStyle = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemID.HallowedBar, 2)
                .AddIngredient(ItemID.Ectoplasm, 1)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }

        public override void PostUpdate()
            => Lighting.AddLight(Item.Center, new Color(140, 249, 255).ToVector3() * 0.45f * Main.essScale);
        public override Color? GetAlpha(Color lightColor)
            => new Color(1f * 0.97f, 1f * 0.97f, 1f * 0.99f, 0.87f);
    }
}
