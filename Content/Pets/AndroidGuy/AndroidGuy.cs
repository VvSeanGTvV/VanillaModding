using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;
using VanillaModding.Common.Utilities;
using VanillaModding.Content.Items.Pets;

namespace VanillaModding.Content.Pets.AndroidGuy
{
    internal class AndroidGuy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            Main.projPet[Type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(1, Main.projFrames[Type] - 3, 8)
                .WithOffset(-10f, 0f)
                .WithSpriteDirection(-1)
                .WithCode(DelegateMethodsHelper.CharacterPreview.Static);
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 50;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        bool floatToPlayer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Distance from player
            float distanceX = player.Center.X - Projectile.Center.X;
            float distanceY = player.Center.Y - Projectile.Center.Y;
            float distance = Vector2.Distance(player.Center, Projectile.Center);

            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            if (!player.dead && player.HasBuff(ModContent.BuffType<PetAndroid>()))
            {
                Projectile.timeLeft = 2;
            }

            // Gravity
            if (Projectile.velocity.Y < 20f)
                Projectile.velocity.Y += 0.4f;

            bool grounded = (Collision.SolidCollision(
                    Projectile.position + new Vector2(0, (Projectile.height - 4) + Projectile.velocity.Y),
                    Projectile.width,
                    4, true));

            if (grounded && !floatToPlayer) Projectile.velocity.Y = 0;

            Projectile.tileCollide = !floatToPlayer;
            if (floatToPlayer)
            {
                float maxSpeed = 20f;

                // distanceY = player.Center.Y - projectile.Center.Y
                float desiredY = distanceY;

                // Scale speed based on distance
                float speed = MathHelper.Clamp(Math.Abs(desiredY) * 0.05f, 1f, maxSpeed);

                // Move toward player
                if (desiredY < 0) // player is above → go up (negative Y)
                    Projectile.velocity.Y = -speed;
                else              // player is below → go down (positive Y)
                    Projectile.velocity.Y = speed;

                floatToPlayer = (Math.Abs(distance) > 50f);
            }
            else floatToPlayer = (Math.Abs(distance) > 750f);

            if (Math.Abs(distance) > 1500f)
            {
                Projectile.position = player.Center - new Vector2(0, Projectile.height / 2);
                SoundEngine.PlaySound(VanillaModdingSoundID.MessageSamsung, Projectile.Center);
                for (int i = 0; i < 20; i++) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
            }
            if (Math.Abs(distanceX) > 50f)
            {
                float speed = 0.15f;
                float maxSpeed = 10f;
                if (Projectile.velocity.Y != 0f && !floatToPlayer) maxSpeed = 5f;
                if (floatToPlayer) maxSpeed = 20f;
                if (distanceX > 0) Projectile.velocity.X += speed;
                else Projectile.velocity.X -= speed;

                if (Projectile.velocity.X > maxSpeed) Projectile.velocity.X = maxSpeed;
                if (Projectile.velocity.X < -maxSpeed) Projectile.velocity.X = -maxSpeed;
            }
            else
            {
                if (Projectile.velocity.Y == 0f) Projectile.velocity.X *= 0.75f;
                else Projectile.velocity.X *= 0.95f;
            }

            // Jump if there's an obstacle
            if (Projectile.velocity.Y == 0f && !floatToPlayer)
            {
                if (Collision.SolidCollision(
                    Projectile.position + new Vector2(Projectile.velocity.X, 0),
                    Projectile.width,
                    Projectile.height))
                {
                    Projectile.velocity.Y = -6f;
                }
            }

            // Walking animation
            if (Math.Abs(Projectile.velocity.X) > 0.1f && Projectile.velocity.Y == 0f && !floatToPlayer)
            {
                // Scale animation speed by movement speed
                float speed = Math.Abs(Projectile.velocity.X);

                // Base frame speed: lower = faster animation
                int baseFrameTime = 8;

                // Dynamic frame time: faster movement = fewer ticks per frame
                int dynamicFrameTime = (int)(baseFrameTime / Math.Clamp(speed, 0.5f, 4f));

                Projectile.frameCounter++;

                if (Projectile.frameCounter >= dynamicFrameTime)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame >= Main.projFrames[Projectile.type] - 2)
                        Projectile.frame = 0;
                }
            }
            else
            {
                
                // Idle frame
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                if (Projectile.velocity.Y > 6f) Projectile.frame = Main.projFrames[Projectile.type] - 2;
                if (Projectile.velocity.Y < -6f || floatToPlayer)
                {
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
                    for (int i = 0; i < 5; i++) {
                        Dust.NewDust(Projectile.Center + new Vector2(-4, -4), 2, 2, DustID.Terra, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-10f, 10f));
                        Dust.NewDust(Projectile.Center + new Vector2(4, -4), 2, 2, DustID.Terra, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-10f, 10f));
                    }
                }
            }

            if (floatToPlayer) Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; else Projectile.rotation = 0;

            // Face movement direction
            if (Projectile.velocity.X != 0)
            {
                Projectile.spriteDirection =
                    Projectile.velocity.X > 0 ? -1 : 1;
            }

        }
    }
}
