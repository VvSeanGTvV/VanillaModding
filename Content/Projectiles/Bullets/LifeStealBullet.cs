using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.Bullets
{
    internal class LifeStealBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            AdvAI.FrameAnimate(0, 3, 5, Projectile);

            Lighting.AddLight(Projectile.Center, Color.IndianRed.ToVector3() * 0.5f);
            for (int i = 0; i < 10; i++)
            {
                Vector2 velOpposite = Projectile.velocity.RotatedBy(MathHelper.ToRadians(180f));
                int dust = Dust.NewDust(Projectile.Center + new Vector2(Main.rand.NextFloat(-0.75f, 0.75f), Main.rand.NextFloat(-0.5f, 0.5f)),0, 0, DustID.VampireHeal, velOpposite.X * 0.25f, velOpposite.Y * 0.25f);
                Main.dust[dust].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life > 0 && !target.immortal) SpawnHelper.CreateLifeSoul(Projectile.GetSource_FromThis(), target, damageDone, 0.085f, Projectile.owner);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SpawnHelper.CreateLifeSoul(Projectile.GetSource_FromThis(), target, info.Damage, 0.085f, Projectile.owner);
            base.OnHitPlayer(target, info);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f)), 0, 0, DustID.VampireHeal, Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                Main.dust[dust].noGravity = true;
            }
            //SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);
            base.OnKill(timeLeft);
        }
    }

    internal class LifeSoul : ModProjectile
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

            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Lighting.AddLight(Projectile.oldPos[k], Color.IndianRed.ToVector3() * 0.5f);
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velOpposite = Projectile.velocity.RotatedBy(MathHelper.ToRadians(180f));
                    int dust = Dust.NewDust(Projectile.oldPos[k], 0, 0, DustID.VampireHeal, velOpposite.X * 0.25f, velOpposite.Y * 0.25f);
                    Main.dust[dust].noGravity = true;
                }
            }

            Player owner = Main.player[Projectile.owner];
            Vector2 directionToPlayer = owner.Center - Projectile.Center;
            float distance = directionToPlayer.Length();
            float returnSpeed = 24f;
            float inertia = 10f;

            // Kill when close enough
            if (distance < 20f || owner.dead || owner == null || owner.statLife >= owner.statLifeMax2)
            {
                if (owner != null && !owner.dead)
                {
                    if (owner.statLife < owner.statLifeMax2 && Projectile.ai[0] > 0)
                    {
                        int healAmount = (int)Projectile.ai[0];
                        owner.Heal(Math.Max((int)(healAmount * Projectile.ai[1]), 1));
                        owner.HealEffect(Math.Max((int)(healAmount * Projectile.ai[1]), 1));
                        if (owner.statLife > owner.statLifeMax2) owner.statLife = owner.statLifeMax2;
                        SoundEngine.PlaySound(SoundID.Item150, Projectile.Center);
                    }
                }
                Projectile.timeLeft = 0;
                return;
            }
            directionToPlayer.Normalize();
            directionToPlayer *= returnSpeed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1) + directionToPlayer) / inertia;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
