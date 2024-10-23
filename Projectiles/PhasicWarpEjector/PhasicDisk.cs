using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.External.AI;

namespace VanillaModding.Projectiles.PhasicWarpEjector
{
    internal class PhasicDisk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAMineThatDealsTripleDamageWhenStationary[Type] = true; // Deal triple damage when not moving and "armed".
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.

            Main.projFrames[Type] = 2;

            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 300; // 300 = 5 seconds (60 ticks -> 1s)
            Projectile.penetrate = -1; // Infinite penetration so that the blast can hit all enemies within its radius.
            Projectile.DamageType = DamageClass.Ranged;
        }

        public void FrameAnimateLight(int startFrame, int finalFrame, int frameSpeed, Projectile projectile)
        {
            projectile.frameCounter++;

            if (projectile.frameCounter > frameSpeed)
            {
                projectile.frameCounter = 0;
                projectile.frame++;

                if (projectile.frame > finalFrame)
                {
                    projectile.frame = startFrame;
                }
            }
            else if (Projectile.frame == 0) Lighting.AddLight(Projectile.position, 1f * 0.79607843137f, 1f * 0.96078431372f, 1f * 0.96862745098f);
        }

        public override void AI()
        {
            FrameAnimateLight(0, 1, 10, Projectile);

            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.PrepareBombToBlow();
            }
            else
            {

                // If the mine is moving very slowly, just make it stop entirely.
                if (Projectile.velocity.X > -0.1f && Projectile.velocity.X < 0.1f)
                {
                    Projectile.velocity.X = 0f;
                }

                if (Projectile.velocity.Y > -0.1f && Projectile.velocity.Y < 0.1f)
                {
                    Projectile.velocity.Y = 0f;
                }

                Projectile.velocity *= 0.97f; // Make it slow down.
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //AdvAI.Explode(Projectile, 100, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 3;
            // Bounce off of tiles.
            /*if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = oldVelocity.X * -0.4f;
            }

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.4f;
            }*/
            // Return false so the projectile doesn't get killed. If you do want your projectile to explode on contact with tiles, do not return true here.
            // If you return true, the projectile will die without being resized (no blast radius).
            // Instead, set `Projectile.timeLeft = 3;` like the Example Rocket Projectile.
            return false;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false; // This is important or the explosion will be in the wrong place if the mine explodes on slopes.
            Projectile.alpha = 255; // Make the mine invisible.

            // Resize the hitbox of the projectile for the blast "radius".
            // Rocket I: 128, Rocket III: 200, Mini Nuke Rocket: 250
            // Measurements are in pixels, so 128 / 16 = 8 tiles.
            Projectile.Resize(164, 164);
            // Set the knockback of the blast.
            // Rocket I: 8f, Rocket III: 10f, Mini Nuke Rocket: 12f
            Projectile.knockBack = 9f;
        }

        public override void OnKill(int timeLeft)
        {
            // Play an exploding sound.
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // Resize the projectile again so the explosion dust and gore spawn from the middle.
            // Rocket I: 22, Rocket III: 80, Mini Nuke Rocket: 50
            Projectile.Resize(22, 22);

            // Spawn a bunch of smoke dusts.
            for (int i = 0; i < 30; i++)
            {
                var smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smokeDust.velocity *= 1.4f;
            }

            // Spawn a bunch of fire dusts.
            for (int j = 0; j < 20; j++)
            {
                var fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                fireDust.noGravity = true;
                fireDust.velocity *= 7f;
                fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                fireDust.velocity *= 3f;
            }

            // Spawn a bunch of smoke gores.
            for (int k = 0; k < 2; k++)
            {
                float speedMulti = 0.4f;
                if (k == 1)
                {
                    speedMulti = 0.8f;
                }

                var smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity += Vector2.One;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X -= 1f;
                smokeGore.velocity.Y += 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity.X += 1f;
                smokeGore.velocity.Y -= 1f;
                smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                smokeGore.velocity *= speedMulti;
                smokeGore.velocity -= Vector2.One;
            }

            // To make the explosion destroy tiles, take a look at the commented out code in Example Rocket Projectile.
        }
    }
}
