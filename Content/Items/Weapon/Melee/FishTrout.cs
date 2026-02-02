using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.Projectiles.FishProjectile;
using VanillaModding.Content.Projectiles.MightyScythe;
using VanillaModding.Content.Projectiles.MightyScythe.MightyProjectile;

namespace VanillaModding.Content.Items.Weapon.Melee
{
    internal class FishTrout : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 147;

            Item.DamageType = DamageClass.Melee;

            Item.width = 64;
            Item.height = 28;

            Item.useTime = 60 * 1;
            Item.useAnimation = 60 * 1;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 6, 0, 0);

            Item.shootsEveryUse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.FishProjectile.FishTrout>();

            Item.rare = ItemRarityID.Blue;
            Item.UseSound = VanillaModdingSoundID.FishSpeeenShort;
            Item.autoReuse = true;

            Item.knockBack = 20;

            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f, adjustedItemScale / 1.25f); //(60f * 8f) player.itemAnimationMax * 2f

            return false;
        }

        public override bool? CanHitNPC(Player player, NPC target) => false;
        public override bool CanHitPvp(Player player, Player target) => false;
    }
}
