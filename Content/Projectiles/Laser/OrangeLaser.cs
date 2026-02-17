using Microsoft.Xna.Framework;
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
            Projectile.width = 44;
            Projectile.height = 4;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 3;
            Projectile.light = 1f;
        }

        public override void AI()
        {
            // The projectile is rotated to face the direction of travel
            Projectile.rotation = Projectile.velocity.ToRotation();
        }   

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.CountsAsACritter && target.type != NPCID.Bunny)
            {
                target.active = false;
                NPC.NewNPC(Projectile.GetSource_FromAI(), (int)target.position.X, (int)target.position.Y, NPCID.Bunny);
            }
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
