using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Items.Placeable.Background;

namespace VanillaModding.Content.Items.Placeable.Block
{
    internal class WhiteStucco : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Block.WhiteStucco>());
            Item.width = 16;
            Item.height = 16;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.SiltBlock)
                .AddIngredient(ItemID.StoneBlock, 10)
                .AddIngredient<WhiteThread>()
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe(1)
                .AddIngredient<WhiteStuccoWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
