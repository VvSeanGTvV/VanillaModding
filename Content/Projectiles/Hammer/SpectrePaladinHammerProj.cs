using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.Systems;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.Hammer
{
    internal class SpectrePaladinHammerProj : ModProjectile
    {
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].GetModPlayer<VanillaModdingPlayer>().Empowered;
        public override string Texture => $"{nameof(VanillaModding)}/Content/Items/Weapon/Melee/Hammer/SpectrePaladinHammer";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(10.5f + Math.Abs(Projectile.velocity.X/10)) * Projectile.direction;

            Player owner = Main.player[Projectile.owner];

            Vector2 directionToPlayer = owner.Center - Projectile.Center;
            float distance = directionToPlayer.Length();
            if (distance >= 550f) Projectile.timeLeft = 10;

            if (Projectile.timeLeft <= 10)
            {
                Projectile.timeLeft = 10;

                Projectile.tileCollide = false;

                float returnSpeed = 27f;
                float inertia = 20f;

                // Kill when close enough
                if (distance < 52f)
                {
                    Projectile.timeLeft = 0;
                    return;
                }

                directionToPlayer.Normalize();
                directionToPlayer *= returnSpeed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + directionToPlayer) / inertia;
            }
        }

        public void OnHit()
        {

            if (Projectile.timeLeft > 10)
            {
                SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit with { Pitch = (EmpoweredHammer + 2f) * 0.1f - 0.2f }, Projectile.Center);
                for (int a = 0; a < 25; a++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 3f);
                    dust.noGravity = true;
                }
                Projectile.velocity = -Projectile.velocity;
            }
            Projectile.timeLeft = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHit();

            Projectile.ai[1] = target.whoAmI;
            if (EmpoweredHammer >= 3)
            {
                Player player = Main.player[Projectile.owner];

                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.velocity / 2 + new Vector2(0, -20f), ModContent.ProjectileType<SpectrePaladinHammerEcho>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack * 1.5f, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[hammer].netUpdate = true;
                EmpoweredHammer = 0;
            }
            else
            {
                EmpoweredHammer++;
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHit();
            base.OnHitPlayer(target, info);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHit();
            return false;
        }

        public override bool PreKill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 3f);
                dust.noGravity = true;
            }
            return base.PreKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 drawOriginPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(new Color(0f, lightColor.G, lightColor.B * 0.976f, lightColor.A * 0.5f)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k] - (Projectile.direction == -1 ? MathHelper.ToRadians(90f) : 0), drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, drawOriginPos, null, new Color(lightColor.R, lightColor.G, lightColor.B, lightColor.A * 0.5f), Projectile.rotation - (Projectile.direction == -1 ? MathHelper.ToRadians(90f) : 0), drawOrigin, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    internal class SpectrePaladinHammerEcho : ModProjectile
    {
        public NPC targeted;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 248, 0, 255) with { A = 0 };
        }

        public override bool? CanDamage()
        => Projectile.ai[0] >= 42f;

        public float rot = 15.5f;
        public bool velocityStart = true;
        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 42f)
            {
                Projectile.rotation += MathHelper.ToRadians(rot) * Projectile.direction;
                Projectile.velocity *= 0.939f;
                rot *= 0.989f;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 0.5f;
                if (Projectile.ai[1] >= 0 && targeted == null && Main.npc[(int)Projectile.ai[1]].active) targeted = Main.npc[(int)Projectile.ai[1]];
                if (targeted == null || !targeted.active) targeted = AdvAI.FindClosestNPC(2000, Projectile.Center, npc => !npc.friendly && npc.CanBeChasedBy(Projectile, false));
                if (targeted != null)
                {
                    Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - targeted.Center).SafeNormalize(Vector2.Zero) * 120f, 0.05f);
                    if (Projectile.penetrate <= -1) Projectile.penetrate = 3;
                }
                else Projectile.Kill();
            }

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.SpectreStaff, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.SpectreStaff, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override bool PreKill(int timeLeft)
        {
            SoundEngine.PlaySound(VanillaModdingSoundID.HammerBigHit, Projectile.Center);
            int pr = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.001f, ModContent.ProjectileType<SpectreExplodeHammer>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            Main.projectile[pr].rotation = Projectile.rotation;
            //SoundEngine.PlaySound(VanillaModdingSoundID.DeathNoteItemAsylum, Projectile.Center);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture, drawPos, null, new Color(0f, lightColor.G, lightColor.B * 0.976f, lightColor.A * 0.5f) with { A = 0 } * 0.5f, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }

    internal class SpectreExplodeHammer : ModProjectile
    {
        public override void SetStaticDefaults() => Main.projFrames[Type] = 8;
        private static float ExplosionRadius = 236f;

        public override void SetDefaults()
        {
            Projectile.width = 472;
            Projectile.height = 472;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600;
            Projectile.light = 1f;
            Projectile.timeLeft = 24;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 3 == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            if (Projectile.frame >= 8)
                Projectile.Kill();

            //Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2 * 1.5f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.15f;
            int hitsToMinMult = 12;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If you are hitting an armored target or kill a target, don't reduce damage based on enemy hits
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && Projectile.numHits > 0)
            {
                Projectile.numHits -= 1;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CollisionUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
    }
}
