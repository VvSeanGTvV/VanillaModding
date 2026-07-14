using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(gold: 3, silver: 50);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 1)
                .AddIngredient(ItemID.Ectoplasm, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
