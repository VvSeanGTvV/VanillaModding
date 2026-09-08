using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Dusts.Sparkle;
using VanillaModding.Content.Projectiles.Tizona;

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
            if (Main.rand.NextBool(4) && !target.HasBuff(BuffID.Bleeding)) target.AddBuff(BuffID.Bleeding, 520);
            OnHitEntity(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.StardustPunch,
            new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) }, Projectile.owner);
            info.HitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);
            if (info.PvP)
            {
                if (Main.rand.NextBool(4) && !target.HasBuff(BuffID.Bleeding)) target.AddBuff(BuffID.Bleeding, 520);
                OnHitEntity(target);
            }
        }

        public void OnHitEntity(Entity target)
        {
            
            if (Main.myPlayer == Projectile.owner) explodeBulb(target);
        }

        private void explodeBulb(Entity target)
        {
            Vector2 velocity = Vector2.Normalize(target.Center - Projectile.Center) * 20f;
            var position = Projectile.position;
            var speedX = velocity.X;
            var speedY = velocity.Y;
            float speedMul = 1.5f;
            float numberProjectiles = 3; // 3 shots
            float rotation = MathHelper.ToRadians(45);//Shoots them in a 45 degree radius. (This is technically 90 degrees because it's 45 degrees up from your cursor and 45 degrees down)
            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //45 should equal whatever number you had on the previous line
            var enS = Projectile.GetSource_FromThis();
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Vector for spread. Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(enS, position, perturbedSpeed * speedMul, ModContent.ProjectileType<DeadlyBulb>(), (int)((Projectile.damage * 2) / numberProjectiles), Projectile.knockBack, Projectile.owner, target.whoAmI); //Creates a new projectile with our new vector for spread.

            }
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
