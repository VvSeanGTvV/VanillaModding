using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials.Bars;

namespace VanillaModding.Content.Items.Placeable.CratingStation
{
    internal class FusionCore : ModItem
    {
        public override void SetDefaults()
        {
            // ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
            // DefaultToPlaceableTile handles setting various Item values that placeable items use
            // Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.FusionCore>());
            Item.width = 32; // The item texture's width
            Item.height = 32; // The item texture's height
            Item.value = Item.sellPrice(0, 5, 0 ,0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient(ItemID.MythrilAnvil)
                .AddIngredient(ItemID.AdamantiteForge)
                .AddIngredient<ElectrifiedBar>(10)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient(ItemID.MythrilAnvil)
                .AddIngredient(ItemID.TitaniumForge)
                .AddIngredient<ElectrifiedBar>(10)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .Register();
        }
    }
}
