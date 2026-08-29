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
        /// <param name="position"> Dust to spawn in this position </param>
        /// <param name="dustColor"> Dust Color if supported </param>
        /// <param name="dustType"> DustID or DustType </param>
        /// <param name="amount"> Amount to spawn in Circle </param>
        /// <param name="Velocity"> How fast </param>
        /// <param name="Offset"> Padding or offset of the Circle </param>
        /// <returns>Returns an array of <see cref="Dust"/> that has spawned, if needed to modify</returns>
        public static Dust[] SpawnCircleDust(Vector2 position, int dustType, int amount, float Velocity = 6f, Vector2 Offset = default, Color dustColor = default)
        {
            if (Offset == default) Offset = Vector2.Zero;

            List<Dust> dusts = new();
            for (int i = 0; i < amount; i++)
            {
                float rot = MathHelper.TwoPi * i / amount;
                Vector2 velocity = new Vector2(Velocity, 0).RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(position + Offset.RotatedBy(rot), dustType, velocity, newColor: dustColor, Alpha: 25);
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
