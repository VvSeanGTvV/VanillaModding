using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.Bullets
{
    internal class ShimmerBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Lighting.AddLight(Projectile.oldPos[k], Color.PowderBlue.ToVector3() * 0.5f);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velOpposite = Projectile.velocity.RotatedBy(MathHelper.ToRadians(180f));
                    int dust = Dust.NewDust(Projectile.oldPos[k], 0, 0, DustID.ShimmerSpark, velOpposite.X, 0);
                    Main.dust[dust].noGravity = true;
                }
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 0.1f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 32f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                Projectile.velocity.Y = 32f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Shimmer, 60);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (target.whoAmI != Projectile.owner) target.AddBuff(BuffID.Shimmer, 60);
            base.OnHitPlayer(target, info);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
