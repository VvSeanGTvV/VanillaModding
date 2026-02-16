using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Buffs;

namespace VanillaModding.Content.Projectiles.Laser
{
    internal class OrangeLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GreenLaser);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextFloat() < 0.35f) target.AddBuff(BuffID.Slow, 120);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextFloat() < 0.35f) target.AddBuff(BuffID.Slow, 120);
            base.OnHitPlayer(target, info);
        }
    }
}
