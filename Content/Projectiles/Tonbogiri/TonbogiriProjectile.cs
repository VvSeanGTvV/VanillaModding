using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using VanillaModding.Content.Dusts.Sparkle;
using Terraria.GameContent.Drawing;

namespace VanillaModding.Content.Projectiles.Tonbogiri
{
    internal class TonbogiriProjectile : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
        protected virtual float HoldoutRangeMin => 32f;
        protected virtual float HoldoutRangeMax => 148f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch,
            new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, Projectile.owner);
            hit.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
            target.AddBuff(BuffID.Bleeding, 720);
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            // Avoid spawning dusts on dedicated servers
            if (!Main.dedServ)
            {
                // These dusts are added later, for the 'ExampleMod' effect
                // ModContent.DustType<Sparkle>()
                if (Main.rand.NextBool(10))
                {
                    var Sparkle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SparkleOld>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 0.75f);
                    Sparkle.frame = new Rectangle(0, 0, 8, 8);
                }

                if (Main.rand.NextBool(20))
                {
                   var Sparkle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SparkleOld>(), Alpha: 128, Scale: 0.65f);
                   Sparkle.frame = new Rectangle(0, 8, 8, 16);
                }
            }

            return false; // Don't execute vanilla AI.
        }
    }
}
