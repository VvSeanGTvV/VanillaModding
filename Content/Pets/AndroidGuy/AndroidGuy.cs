using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Pets;

namespace VanillaModding.Content.Pets.AndroidGuy
{
    internal class AndroidGuy : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            if (!player.dead && player.HasBuff(ModContent.BuffType<PetAndroid>()))
            {
                Projectile.timeLeft = 2;
            }

            // Gravity
            if (Projectile.velocity.Y < 10f)
                Projectile.velocity.Y += 0.4f;

            // Distance from player
            float distance = player.Center.X - Projectile.Center.X;

            // Walk toward player
            if (Math.Abs(distance) > 50f)
            {
                float speed = 2f;

                if (distance > 0)
                    Projectile.velocity.X = speed;
                else
                    Projectile.velocity.X = -speed;
            }
            else
            {
                // Slow down when close
                Projectile.velocity.X *= 0.8f;
            }

            // Jump if there's an obstacle
            if (Projectile.velocity.Y == 0f)
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
            if (Math.Abs(Projectile.velocity.X) > 0.1f && Projectile.velocity.Y == 0f)
            {
                Projectile.frameCounter++;

                if (Projectile.frameCounter >= 8)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
            else
            {
                // Idle frame
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }

            // Face movement direction
            if (Projectile.velocity.X != 0)
            {
                Projectile.spriteDirection =
                    Projectile.velocity.X > 0 ? 1 : -1;
            }
        }
    }
}
