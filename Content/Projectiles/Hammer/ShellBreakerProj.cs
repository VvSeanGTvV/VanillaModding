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
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.Systems;
using VanillaModding.Common.Utilities;

namespace VanillaModding.Content.Projectiles.Hammer
{
    internal class ShellBreakerProj : ModProjectile
    {
        public ref int EmpoweredHammer => ref Main.player[Projectile.owner].GetModPlayer<VanillaModdingPlayer>().Empowered;
        public override string Texture => $"{nameof(VanillaModding)}/Content/Items/Weapon/Melee/ShellBreaker";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 56;
            Projectile.friendly = true;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;

            Projectile.ArmorPenetration = 15;
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
            Entity[] data = CollisionUtils.GetEntitysinCircle(Main.npc, Projectile.Center, 95, 5, ent =>
            {
                NPC npc = (NPC)ent;
                if (npc == null) return false;
                return !npc.friendly && !npc.dontTakeDamage;
            });

            foreach (Entity entity in data)
            {
                if (!entity.active) continue;
                NPC npc = (NPC)entity;
                if (npc == null || !npc.active || npc.whoAmI == Projectile.ai[1]) continue;


                StatModifier damageModifier = player.GetTotalDamage(Projectile.DamageType);
                player.ApplyDamageToNPC(
                    npc,
                    (int)Math.Max(1, damageModifier.ApplyTo(Projectile.damage * 0.7f)),
                    npc.knockBackResist * Projectile.knockBack,
                    npc.Center.X < Projectile.Center.X ? -1 : 1,
                    Main.rand.NextFloat() < Projectile.CritChance / 100f,
                    Projectile.DamageType,
                    true
                    );
            }

            Dust[] dusts = SpawnHelper.SpawnCircleDust(Projectile.Center, DustID.ChlorophyteWeapon, 23, offset: new Vector2(12f, 0));
            int i = 0;
            bool flip = Main.rand.NextBool();
            foreach (Dust dust in dusts)
            {
                dust.velocity *=  (float)(0.75f + Math.Abs(Math.Sin(i) * 0.25f));
                dust.noGravity = true;
                dust.scale = 2.5f;
                i += (flip) ? 1 : -1;
            }

            if (HighBong) SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit with { Pitch = 6 * 0.1f - 0.2f }, Projectile.Center);
            else SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit with { Pitch = (EmpoweredHammer + 2f) * 0.1f - 0.2f }, Projectile.Center);
            //SoundEngine.PlaySound(SoundID.Item16 with { Pitch = (EmpoweredHammer + 1f) * 0.1f - 0.2f, Volume = 1.15f }, Projectile.Center);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = target.whoAmI;
            if (EmpoweredHammer >= 3)
            {
                Player player = Main.player[Projectile.owner];

                ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.ChlorophyteLeafCrystalPassive,
                new ParticleOrchestraSettings { PositionInWorld = Projectile.Center }, Projectile.owner);
                int hammer = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.SafeNormalize(Vector2.UnitX).X * 10f, 0), ModContent.ProjectileType<ShellBreakerEcho>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack * 1.5f, Projectile.owner, 0f, Projectile.ai[1]);
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
                Color color = Projectile.GetAlpha(new Color(lightColor.R * 0.65f, lightColor.G, 0, lightColor.A)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k] - (Projectile.direction == -1 ? MathHelper.ToRadians(90f) : 0), drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }

    internal class ShellBreakerEcho : ModProjectile
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
                if (velocityStart)
                {
                    Projectile.velocity.Y -= 0.35f;
                    Projectile.velocity.X *= 0.989f;
                }
                rot *= 0.989f;
            }
            else
            {
                velocityStart = false;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 0.5f;
                if (Projectile.ai[1] >= 0 && targeted == null && Main.npc[(int)Projectile.ai[1]].active) targeted = Main.npc[(int)Projectile.ai[1]];
                if (targeted == null || !targeted.active) targeted = AdvAI.FindClosestNPC(2000, Projectile.Center, npc => !npc.friendly && npc.CanBeChasedBy(Projectile, false));
                if (targeted != null)
                {
                    Projectile.velocity = -Vector2.Lerp(-Projectile.velocity, (Projectile.Center - targeted.Center).SafeNormalize(Vector2.Zero) * 40f, 0.05f);
                    if (Projectile.penetrate <= -1) Projectile.penetrate = 3;
                }
                else Projectile.Kill();
            }

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.ChlorophyteWeapon, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.ChlorophyteWeapon, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100, new Color(255, 245, 198), 2f);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            rot = 15.5f;
            Projectile.velocity = -Projectile.velocity / 2;
            Dust[] dusts = SpawnHelper.SpawnCircleDust(Projectile.Center, DustID.FartInAJar, 25, 12.5f, offset: new Vector2(15f, 0));
            int i = 0;
            foreach (Dust dust in dusts)
            {
                dust.noGravity = true;
                dust.scale = 2.25f;
                i++;
            }

            Projectile.ai[0] = 0;
            SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item16 with { Volume = 1.45f }, Projectile.Center);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            rot = 15.5f;
            Projectile.velocity = -Projectile.velocity / 2;
            Dust[] dusts = SpawnHelper.SpawnCircleDust(Projectile.Center, DustID.FartInAJar, 25, 12.5f, offset: new Vector2(15f, 0));
            int i = 0;
            foreach (Dust dust in dusts)
            {
                dust.noGravity = true;
                dust.scale = 2.25f;
                i++;
            }

            Projectile.ai[0] = 0;
            SoundEngine.PlaySound(VanillaModdingSoundID.HammerHit, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item16 with { Volume = 1.45f }, Projectile.Center);
            base.OnHitPlayer(target, info);
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            
            Entity[] data = CollisionUtils.GetEntitysinCircle(Main.npc, Projectile.Center, 178, 10, ent =>
            {
                NPC npc = (NPC)ent;
                if (npc == null) return false;
                return !npc.friendly && !npc.dontTakeDamage;
            });

            foreach (Entity entity in data)
            {
                if (!entity.active) continue;
                NPC npc = (NPC)entity;
                if (npc == null || !npc.active || npc.whoAmI == Projectile.ai[1]) continue;

                StatModifier damageModifier = player.GetTotalDamage(Projectile.DamageType);
                player.ApplyDamageToNPC(
                    npc,
                    (int)Math.Max(1, damageModifier.ApplyTo(Projectile.damage * 0.6f)),
                    npc.knockBackResist * Projectile.knockBack,
                    npc.Center.X < Projectile.Center.X ? -1 : 1,
                    Main.rand.NextFloat() < Projectile.CritChance / 100f,
                    Projectile.DamageType,
                    true
                    );
            }

            Projectile[] projectiles = SpawnHelper.SpawnCircleProjectile(Projectile.Center, ProjectileID.SporeCloud, Projectile.damage / 10, 0, 8, Projectile.owner, 12f);
            Dust[] dusts = SpawnHelper.SpawnCircleDust(Projectile.Center, DustID.FartInAJar, 45, 12.5f, offset: new Vector2(15f, 0));
            int i = 0;
            foreach (Dust dust in dusts)
            {
                dust.noGravity = true;
                dust.velocity = dust.velocity * (float)(0.85f + Math.Abs(Math.Sin(i)) * 0.15f);
                dust.scale = 3f;
                i++;
            }


            SoundEngine.PlaySound(VanillaModdingSoundID.HammerBigHit, Projectile.Center);
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
                Main.EntitySpriteDraw(texture, drawPos, null, Color.LimeGreen with { A = 0 } * 0.5f, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}

