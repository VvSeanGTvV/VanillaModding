using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Projectiles.Tizona;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    public class Tizona : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 110;

            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<TizonaProjectile>();

            Item.DamageType = DamageClass.Melee;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = 1;
            Item.knockBack = 5;
            Item.value = 6000;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AdamantiteBar, 15);
            recipe.AddIngredient(ModContent.ItemType<SoulofBlight>(), 15);
            recipe.AddIngredient(ItemID.Excalibur, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            Recipe recipe1 = CreateRecipe();
            recipe1.AddIngredient(ItemID.TitaniumBar, 15);
            recipe1.AddIngredient(ModContent.ItemType<SoulofBlight>(), 15);
            recipe1.AddIngredient(ItemID.Excalibur, 1);
            recipe1.AddTile(TileID.MythrilAnvil);
            recipe1.Register();
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 720);
            base.OnHitNPC(player, target, hit, damageDone);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Bleeding, 720);
            base.OnHitPvp(player, target, hurtInfo);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f, adjustedItemScale / 1.25f);
            Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<DeadlyBulb>(), damage, knockback / 2, player.whoAmI, 0);
            /*float projectilesCount = 2;
            float rotation = MathHelper.ToRadians(25);
            for (int i = 0; i < projectilesCount; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy((MathHelper.Lerp(0, rotation, i / Math.Clamp(projectilesCount - 1, 1, projectilesCount)) - 0.5f) * -player.direction * player.gravDir) * 1.1f;
                Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<DeadlyBulb>(), damage / 2, knockback / 2, player.whoAmI, -1);
            }*/
            return false;
        }
    }
}