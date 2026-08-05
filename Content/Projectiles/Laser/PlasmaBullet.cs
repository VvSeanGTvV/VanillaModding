using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Utilities;
using VanillaModding.Content.Buffs;

namespace VanillaModding.Content.Projectiles.Laser
{
    internal class PlasmaBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 4;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.light = 1f;
        }

        public override void AI()
        {
            AdvAI.FrameAnimate(0, 3, 5, Projectile);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }   

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
            base.OnHitPlayer(target, info);
        }
    }
}
