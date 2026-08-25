using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Projectiles.Tizona;
using static System.Net.Mime.MediaTypeNames;

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
            Item.useStyle = ItemUseStyleID.Swing;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            if (Main.myPlayer == player.whoAmI) Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f, adjustedItemScale / 1.25f);
            return false;
        }
    }
}