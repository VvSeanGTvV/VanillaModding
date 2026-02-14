using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace VanillaModding.External.AI
{
    internal class AdvAI
    {

        /// <summary>
        /// Find the closest enemy <see cref="NPC"/> by distance, from the point vector in <paramref name="center"/>.
        /// </summary>
        /// <param name="maxDetectDistance"> Maximum distance that projectile can see. </param>
        /// <param name="center"> the point of position acting as radar or sonar. </param>
        /// <param name="filter"> <para> defaults to null for no filter use. If returns false, ignore the NPC entirely and go to next one. </para></param>
        /// <returns> <see cref="NPC"/> closest to Projectile </returns>
        public static NPC FindClosestNPC(float maxDetectDistance, Vector2 center, Func<NPC, bool> filter = null)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {

                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, center);

                    if (filter != null && !filter(target))
                        continue;

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        /// <summary>
        /// Find the closest <see cref="Player"/> by distance, from the point vector in <paramref name="center"/>.
        /// </summary>
        /// <param name="maxDetectDistance"> Maximum distance that projectile can see. </param>
        /// <param name="center"> the point of position acting as radar or sonar. </param>
        /// <param name="filter"> <para> defaults to null for no filter use. If returns false, ignore the Player entirely and go to next one. </para></param>
        /// <returns><see cref="Player"/> closest to Projectile.</returns>
        public static Player FindClosestPlayer(float maxDetectDistance, Vector2 center, Func<Player, bool> filter = null)
        {
            Player closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];

                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, center);

                if (filter != null && !filter(target))
                    continue;

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }

            return closestNPC;
        }

        /// <summary>
        /// Basic Animation Projectile function.
        /// </summary>
        /// <param name="startFrame"> The start of the frame. </param>
        /// <param name="finalFrame"> The end of the frame. (loops back to <paramref name="startFrame"/>). </param>
        /// <param name="frameSpeed"> How fast is the animation </param>
        /// <param name="projectile"> This Projectile. </param>
        public static void FrameAnimate(int startFrame, int finalFrame, int frameSpeed, Projectile projectile)
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
        }

        /// <summary>
        /// Basic Projectile Explosion function.
        /// </summary>
        /// <param name="range"> How large is the explosion. </param>
        /// <param name="projectile"> This Projectile. </param>
        /// <param name="damage"> How much damage does the explosion does. </param>
        public static void Explode(Projectile projectile, int range, int damage)
        {
            int def0 = 250;
            int def1 = 50 * (range / def0);
            int def2 = 80 * (range / def0);
            projectile.damage = damage;
            int DefaultWidth = projectile.width;
            int DefaultHeight = projectile.height;
            projectile.Resize(range, range);
            for (int i = 0; i < def1; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            for (int i = 0; i < def2; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(projectile.position.X + projectile.width / 2 - 24f, projectile.position.Y + projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }
            // reset size to normal width and height.
            projectile.Resize(DefaultWidth, DefaultHeight);
        }
    }
}
