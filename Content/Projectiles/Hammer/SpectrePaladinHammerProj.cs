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

                //int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.SafeNormalize(Vector2.UnitX).X * 10f, 0), ModContent.ProjectileType<ShellBreakerEcho>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack * 1.5f, Projectile.owner, 0f, Projectile.ai[1]);
                //Main.projectile[hammer].netUpdate = true;
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
}
