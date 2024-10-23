using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VanillaModding.Projectiles.OcramProjectile
{
    internal class OcramArmThrowaway : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 28;
            Projectile.scale = 1.5f;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        // Additional hooks/methods here.

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.15f * Projectile.velocity.Length() / 10f * Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.225f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 24f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                Projectile.velocity.Y = 24f;
            }
        }
    }
}
