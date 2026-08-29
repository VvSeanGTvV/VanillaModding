using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace VanillaModding.Common.Utilities
{
    internal class CollisionUtils
    {
        /// <summary>
        /// <para> Gets all the entities in a circle radius, though depends which you are going with either <see cref="Main.npc"/>, <see cref="Main.player"/>, or <see cref="Main.projectile"/>. </para>
        /// <para> Its a useful function when making an AoE damage or other damage style. </para>
        /// </summary>
        /// <param name="entities"> Entity Array to filter </param>
        /// <param name="centerPosition"> Position acting as center </param>
        /// <param name="radius"> Circle Radius </param>
        /// <param name="limit"><para> when Limit is > 0 then Limit is acting as cap of how many entity to grab within the radius </para></param>
        /// <param name="filter"><para> If return false, ignore the NPC entirely and go to next one. Otherwise leave it empty for no filter </para></param>
        /// <returns></returns>
        public static Entity[] GetEntitysinCircle(Entity[] entities, Vector2 centerPosition, float radius, int limit = 0, Func<Entity, bool> filter = null)
        {
            List<Entity> list = new List<Entity>();
            if (radius <= 0f)
                return null;

            foreach (Entity entity in entities)
            {
                if (limit > 0 && list.Count > limit) break;
                if (filter != null && !filter(entity))
                    continue;

                if (entity.active && CircularHitboxCollision(centerPosition, radius, entity.Hitbox)) list.Add(entity);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Checks whether the hitbox has hit within the radius
        /// </summary>
        /// <param name="centerCheckPosition"> Position acting as center </param>
        /// <param name="radius"> Circle Radius </param>
        /// <param name="targetHitbox"> Target </param>
        /// <returns> Considers a hit or not </returns>
        public static bool CircularHitboxCollision(Vector2 centerCheckPosition, float radius, Rectangle targetHitbox)
        {

            if (radius <= 0f)
                return false;

            float closestX = MathHelper.Clamp(centerCheckPosition.X, targetHitbox.Left, targetHitbox.Right);
            float closestY = MathHelper.Clamp(centerCheckPosition.Y, targetHitbox.Top, targetHitbox.Bottom);

            float dx = centerCheckPosition.X - closestX;
            float dy = centerCheckPosition.Y - closestY;

            return (dx * dx + dy * dy) <= (radius * radius);
        }
    }
}
