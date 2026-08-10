using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VanillaModding.Content.Dusts;
using VanillaModding.Content.Projectiles.Bullets;

namespace VanillaModding.Common.Utilities
{
    internal class SpawnHelper
    {
        /// <summary>
        /// A helper to spawn a circle of dust
        /// </summary>
        /// <param name="position">where to spawn it</param>
        /// <param name="dustColor">dust color</param>
        /// <param name="dustType">what dust is it</param>
        /// <param name="amount">How many dust in a circle spawned</param>
        /// <param name="X"> unknown </param>
        /// <returns>Returns an array of <see cref="Dust"/> that has spawned</returns>
        public static Dust[] SpawnCircleDust(Vector2 position, Color dustColor, int dustType, int amount, int X = 2)
        {
            Vector2 vel = Vector2.UnitX * X;
            List<Dust> dusts = new();

            for (int i = 0; i < amount; i++)
            {
                float rot = MathHelper.TwoPi * i / amount;
                Vector2 velocity = vel.RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, newColor: dustColor, Alpha: 25);
                dust.scale = 1f;

                dusts.Add(dust);
            }
            return dusts.ToArray();
        }

        /// <summary>
        /// Creates a Life steal projectile, which is the return version.
        /// </summary>
        /// <param name="source"> source </param>
        /// <param name="onHit"> The entity that got hit </param>
        /// <param name="damageReturn"> The Damage which returns as heal </param>
        /// <param name="returnEffectiveness"> How effective it returns, as leaving it 1 being full return no loss </param>
        /// <param name="owner"> Owner of the Projectile has to be a player </param>
        public static void CreateLifeSoul(IEntitySource source, Entity onHit, int damageReturn, float returnEffectiveness, int owner)
        {
            Player player = Main.player[owner];
            if (Main.myPlayer == player.whoAmI && player != null && !player.dead) Projectile.NewProjectile(source, onHit.Center, Vector2.Zero, ModContent.ProjectileType<LifeSoul>(), 0, 0, owner, damageReturn, returnEffectiveness);
        }
    }
}
