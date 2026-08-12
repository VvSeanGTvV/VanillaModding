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
using VanillaModding.Content.Pets.AndroidGuy;
using VanillaModding.Content.Pets.SandTrapperPet;

namespace VanillaModding.Content.Items.Pets
{
    public class PetAndroid : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        { // This method gets called every frame your buff is active on your player.
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<AndroidGuy>());
        }
    }

    internal class ShinyBlackSlab : ModItem
    {
        // Names and descriptions of all ExamplePetX classes are defined using .hjson files in the Localization folder
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<AndroidGuy>(), ModContent.BuffType<PetAndroid>());

            Item.noUseGraphic = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2); // The item applies the buff, the buff spawns the projectile

            return false;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            /*CreateRecipe()
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();*/
        }
    }
}
