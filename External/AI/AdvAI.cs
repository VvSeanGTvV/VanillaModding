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
        /// Find the closest NPC by distance. Using via: Projectile.
        /// </summary>
        /// <param name="maxDetectDistance"> Maximum distance to detect a nearby NPC </param>
        /// <param name="projectile"> The Projectile acting as radar </param>
        /// <returns> NPC closest to Projectile </returns>
        public static NPC FindClosestNPC(float maxDetectDistance, Projectile projectile)
        {   
            return FindClosestNPC(maxDetectDistance, projectile, null);
        }

        /// <summary>
        /// Find the closest NPC by distance. Using via: Projectile w/ Filter.
        /// </summary>
        /// <param name="maxDetectDistance"> Maximum distance to detect a nearby NPC </param>
        /// <param name="projectile"> The Projectile acting as radar </param>
        /// <param name="filter"> Filters any NPC </param>
        /// <returns> NPC closest to Projectile </returns>
        public static NPC FindClosestNPC(float maxDetectDistance, Projectile projectile, Func<NPC, bool> filter = null)
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
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        if (filter != null && filter(closestNPC)) closestNPC = target; else if (filter == null) closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        /// <summary>
        /// Find the closest Player by distance. Using via: Projectile.
        /// </summary>
        /// <param name="maxDetectDistance"></param>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static Player FindClosestPlayer(float maxDetectDistance, Projectile projectile)
        {
            Player closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];

                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

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
        /// Find the closest Player by distance. Using via: Projectile.
        /// </summary>
        /// <param name="maxDetectDistance"></param>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static Player FindClosestPlayerNPC(float maxDetectDistance, NPC projectile)
        {
            Player closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];

                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

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
        /// <param name="startFrame"></param>
        /// <param name="finalFrame"></param>
        /// <param name="frameSpeed"></param>
        /// <param name="projectile"></param>
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
        /// <param name="range"></param>
        /// <param name="projectile"></param>
        /// <param name="damage"></param>
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
