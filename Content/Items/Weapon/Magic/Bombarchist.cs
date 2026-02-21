using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Content.NPCs.DuneTrapper;
using VanillaModding.Content.Projectiles.Bombs;

namespace VanillaModding.Content.Items.Weapon.Magic
{
    internal class Bombarchist : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.accessory = false;

            Item.mana = 16;
            Item.damage = 120;
            Item.value = Item.sellPrice(gold: 10, silver: 95);
            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<BombarchistProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 30;
            Item.useAnimation = 30;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, new Vector2(player.Center.X, player.Center.Y - 550f), new Vector2(Main.rand.NextFloat(5f, 10f) * player.direction, 15f), type, damage, knockback, player.whoAmI);
            for (int i = 0; i < 10; i++)
            {
                if (Main.rand.NextBool(10)) Projectile.NewProjectile(source, new Vector2(player.Center.X + Main.rand.NextFloat(-200f, 200f), player.Center.Y - 550f), new Vector2(Main.rand.NextFloat(5f, 10f) * player.direction, 15f), type, damage, knockback, player.whoAmI);
            }
            //return base.Shoot(player, source, new Vector2(player.Center.X, player.Center.Y + 550f), new Vector2(5f, 150f), type, damage, knockback);
            return false;
        }
    }

    internal class BombarchistProj : ModProjectile
    {
        private const int DefaultWidthHeight = 22;
        private const int ExplosionWidthHeight = 250;
        public override void SetDefaults()
        {
            // While the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
            Projectile.width = DefaultWidthHeight;
            Projectile.height = DefaultWidthHeight;
            //Projectile.friendly = true;
            Projectile.penetrate = -1;

            // 5 second fuse.
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            // These help the projectile hitbox be centered on the projectile sprite.
            //DrawOffsetX = -2;
            //DrawOriginOffsetY = -5;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Vanilla explosions do less damage to Eater of Worlds in expert mode, so we will too.
            if (Main.expertMode)
            {
                if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                {
                    modifiers.FinalDamage /= 5;
                }

                if (target.type == ModContent.NPCType<DuneTrapperHead>() && target.type == ModContent.NPCType<DuneTrapperTail>() && target.type == ModContent.NPCType<DuneTrapperBody>())
                {
                    modifiers.FinalDamage /= 5;
                }
            }
            if (!(Projectile.ai[2] == Main.myPlayer) && Projectile.ai[1] != 0 && Main.player[Main.myPlayer].team == Main.player[target.whoAmI].team && !(Main.netMode == NetmodeID.SinglePlayer))
            {
                modifiers.FinalDamage *= 0;
            }

            if (Main.rand.NextBool(20)) target.AddBuff(BuffID.Confused, 60 * 5);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(20)) target.AddBuff(BuffID.Confused, 60 * 5);
        }
        // The projectile is very bouncy, but the spawned children projectiles shouldn't bounce at all.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
            if (Projectile.ai[1] != 0)
            {
                return true;
            }

            // This code makes the projectile very bouncy.
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.25f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.25f;
            }
            return false;
        }

        bool played;
        public override void AI()
        {
            Player myPlayer = Main.player[Projectile.owner];
            bool gone = (!myPlayer.active || myPlayer.dead);
            // The projectile is in the midst of exploding during the last 3 updates.
            if (Projectile.ai[1] != 0)
            {
                Projectile.tileCollide = false;
                // Set to transparent. This projectile technically lives as transparent for about 3 frames
                Projectile.alpha = 255;

                // change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

                Projectile.damage = 120;
                Projectile.knockBack = 10f;
                Projectile.friendly = false;
                Projectile.hostile = true;
                Projectile.timeLeft = 1;
            }
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                // Set to transparent. This projectile technically lives as transparent for about 3 frames
                Projectile.alpha = 255;

                // change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);
                Projectile.friendly = true;

                Projectile.damage = 120;
                Projectile.knockBack = 5f;
            }
            else
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                    {
                        if (Projectile.Hitbox.Intersects(npc.Hitbox))
                        {
                            Projectile.timeLeft = 3;
                        }
                    }
                }

                float activationRange = 600f;
                float rangeSq = activationRange * activationRange;

                bool closeEnough =
                    Vector2.DistanceSquared(Projectile.Center, myPlayer.Center) <= rangeSq;

                if (!Projectile.tileCollide && (!gone && closeEnough || gone))
                    if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                        Projectile.tileCollide = true;
                    else
                        Projectile.timeLeft = 300;

                // Smoke and fuse dust spawn. The position is calculated to spawn the dust directly on the fuse.
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
                    dust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
                    dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                    dust.noGravity = true;
                    dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;

                    dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
                    dust.scale = 1f + Main.rand.Next(5) * 0.1f;
                    dust.noGravity = true;
                    dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;
                }
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                // Roll speed dampening. 
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.96f;

                    if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                // Delayed gravity
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            // Rotation increased by velocity.X 
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {

            // Play explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            // Smoke Dust spawn
            var enS = Projectile.GetSource_FromThis();
            //if (Projectile.ai[1] == 0) Projectile.NewProjectile(enS, Projectile.Center, new Vector2(0, 0), Projectile.type, Projectile.damage, Projectile.knockBack, -1, 0, 1, Projectile.owner);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }
            // reset size to normal width and height.
            Projectile.Resize(DefaultWidthHeight, DefaultWidthHeight);
        }
    }
}
