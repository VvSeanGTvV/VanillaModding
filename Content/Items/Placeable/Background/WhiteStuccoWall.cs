using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Placeable.Block;

namespace VanillaModding.Content.Items.Placeable.Background
{
    internal class WhiteStuccoWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Tiles.Background.WhiteStuccoWall>());
            Item.width = 24;
            Item.height = 24;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<WhiteStucco>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
