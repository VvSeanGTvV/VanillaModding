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
using VanillaModding.Common;
using VanillaModding.Common.Systems;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.Hammer
{
    internal class PwnageddonProj : ModProjectile
    {
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].GetModPlayer<VanillaModdingPlayer>().Empowered;
        public override string Texture => $"{nameof(VanillaModding)}/Content/Items/Weapon/Melee/Pwnageddon";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }

        public int time = 0;
        public bool HighBong = false;
        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += MathHelper.ToRadians(5.5f) * Projectile.direction;

            Projectile.velocity.X *= 0.97f;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            if (Projectile.velocity.Y > 32f)
            {
                Projectile.velocity.Y = 32f;
            }
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            float numberOfDusts = 13f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(9f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(6f, 0).RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Sandnado, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = true;
                dust.velocity = velOffset;
                dust.scale = 2.5f;
            }

            if (HighBong) SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit with { Pitch = 6 * 0.1f - 0.2f }, Projectile.Center);
            else SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit with { Pitch = (EmpoweredHammer + 2f) * 0.1f - 0.2f }, Projectile.Center);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (EmpoweredHammer >= 3)
            {
                Projectile.ai[1] = target.whoAmI;
                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.SafeNormalize(Vector2.UnitX).X * 5, -15f), ModContent.ProjectileType<PwnageddonEcho>(), Projectile.damage * 2, Projectile.knockBack * 1.5f, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[hammer].localAI[0] = Math.Sign(Projectile.velocity.X);
                Main.projectile[hammer].netUpdate = true;
                HighBong = true;
                EmpoweredHammer = 0;
            }
            else
            {
                EmpoweredHammer++;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(new Color(lightColor.R, lightColor.G, 0, lightColor.A)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }

    internal class PwnageddonEcho : ModProjectile
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
            Projectile.width = Projectile.height = 46;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 248, 0, 255) with { A = 0 };
        }

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 42f)
            {
                Projectile.velocity.Y *= 0.9575f;
                Projectile.velocity.X *= 0.98f;
                Projectile.rotation += MathHelper.ToRadians(Projectile.ai[0] * 0.5f) * Projectile.localAI[0] * Projectile.direction;
            }
            else if (Projectile.ai[0] >= 42f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 0.5f;
                Projectile.extraUpdates = 2;

                if (Projectile.ai[1] != -5)
                    targeted = Main.npc[(int)Projectile.ai[1]];
                if (targeted == null || !targeted.CanBeChasedBy(Projectile, false) || !targeted.active)
                {
                    Projectile.ai[1] = -5;
                    targeted = AdvAI.FindClosestNPC(2000, Projectile.Center, npc => !npc.friendly);
                }
                if (targeted != null)
                {
                    Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - targeted.Center).SafeNormalize(Vector2.Zero) * 40f, 0.05f);
                    if (Projectile.penetrate <= -1) Projectile.penetrate = 1;
                }
                else
                    Projectile.Kill();
            }

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.GoldFlame, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            float numberOfDusts = 45f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(15f, 0).RotatedBy(rot);
                Vector2 velOffset = new Vector2(12.5f, 0).RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Sandnado, velOffset);
                dust.noGravity = true;
                dust.velocity = velOffset * (i % 2 == 0 ? 0.9f : i % 3 == 0 ? 0.8f : 1f);
                dust.scale = 3f;
            }

            SoundEngine.PlaySound(VanillaModdingSoundID.HammerBigHit, Projectile.Center);
            SoundEngine.PlaySound(VanillaModdingSoundID.DeathNoteItemAsylum, Projectile.Center);
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
                Main.EntitySpriteDraw(texture, drawPos, null, Color.Gold with { A = 0 } * 0.5f, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
