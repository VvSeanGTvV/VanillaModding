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
using VanillaModding.Common.Systems;
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
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<AndroidGuy>());
        }
    }

    internal class ShinyBlackSlab : ModItem
    {
        // Names and descriptions of all ExamplePetX classes are defined using .hjson files in the Localization folder
        public override void SetDefaults()
        {
            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = VanillaModdingSoundID.AndroidSummon;
            Item.value = Item.sellPrice(0, 0, 15, 0);

            Item.buffType = ModContent.BuffType<PetAndroid>();
            Item.shoot = ModContent.ProjectileType<AndroidGuy>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2); // The item applies the buff, the buff spawns the projectile
            Projectile.NewProjectile(source, player.Center.X, player.Center.Y, 0f, 0f, type, 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Emerald, 1)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Emerald, 1)
                .AddIngredient(ItemID.Glass, 5)
                .AddIngredient(ItemID.TungstenBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
