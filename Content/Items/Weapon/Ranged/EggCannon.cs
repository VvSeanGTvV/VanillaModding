using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Items.Ammo;
using VanillaModding.Content.Projectiles.PhasicWarpEjector;

namespace VanillaModding.Content.Items.Weapon.Ranged
{
    internal class EggCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;

            AmmoID.Sets.SpecificLauncherAmmoProjectileMatches.Add(Type, new Dictionary<int, int> {
                { ModContent.ItemType<Content.Items.Weapon.Throwable.Egg>(), ModContent.ProjectileType<Content.Projectiles.Lepus.Egg>() },
                { ItemID.RottenEgg, ProjectileID.RottenEgg },
            });
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(56, 20);

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 40;

            Item.knockBack = 3.5f;
            Item.noMelee = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = Item.useTime = 24;

            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            Item.shoot = ModContent.ProjectileType<Content.Projectiles.Lepus.Egg>();
            Item.shootSpeed = 15f;
            Item.useAmmo = ModContent.ItemType<Content.Items.Weapon.Throwable.Egg>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Minishark, 1)
                .AddRecipeGroup(VanillaModdingRecipeGroupID.AnySilverBar, 20)
                .AddIngredient<Content.Items.Weapon.Throwable.Egg>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override Vector2? HoldoutOffset()
            => new Vector2(-5, 0);
    }
}
