using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.Content.Projectiles.Arrows
{
    internal class BoringArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenTorch, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f);

            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (Projectile.velocity.Y > 32f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                Projectile.velocity.Y = 32f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.ToRadians(90f);
        }
    }
}
