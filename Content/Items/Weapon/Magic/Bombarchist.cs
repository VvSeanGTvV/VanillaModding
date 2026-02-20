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

namespace VanillaModding.Content.Items.Weapon.Magic
{
    internal class Bombarchist : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = false;

            Item.mana = 16;
            Item.damage = 120;
            Item.value = Item.sellPrice(gold: 10, silver: 95);
            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<BombarchistProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    internal class BombarchistProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HappyBomb);
            AIType = ProjectileID.Grenade;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
    }
}
