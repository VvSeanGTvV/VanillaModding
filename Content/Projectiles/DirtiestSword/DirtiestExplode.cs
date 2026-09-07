using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.DirtiestSword
{
    internal class DirtiestExplode : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 10;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
            Projectile.light = 1;
        }

        public override bool? CanDamage()
        => false;

        public override void AI()
        {
            AdvAI.FrameAnimate(0, 3, 3, Projectile);

            if (Projectile.frame == 3)
            {
                Projectile.Kill();
            }
        }
    }
}
