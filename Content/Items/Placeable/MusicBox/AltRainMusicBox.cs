using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Items.Placeable.MusicBox
{
    internal class AltRainMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/Music_Tutorial"), ModContent.ItemType<AltRainMusicBox>(), ModContent.TileType<Tiles.AltRainMusicBox>());
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            ItemID.Sets.CanGetPrefixes[Type] = false;
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<Tiles.AltRainMusicBox>(), 0);
            Item.maxStack = 1;
            Item.width = 30; // The width of item hitbox
            Item.height = 30; // The height of item hitbox
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MusicBoxDesert, 1);
            recipe.AddIngredient(ItemID.MusicBoxSpace, 1);
            recipe.AddIngredient(ItemID.MusicBoxBoss4, 1);
            recipe.AddIngredient(ItemID.MusicBoxOcean, 1);
            recipe.AddIngredient(ItemID.MusicBoxSnow, 1);
            //recipe.AddIngredient(ModContent.ItemType<LobotomyThrowable>(), 1);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}
