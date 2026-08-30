using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
        /// <param name="velocity"> How fast </param>
        /// <param name="offset"> Padding or offset of the Circle </param>
        /// <returns>Returns an array of <see cref="Dust"/> that has spawned, if needed to modify</returns>
        public static Dust[] SpawnCircleDust(Vector2 position, int dustType, int amount, float velocity = 6f, Vector2 offset = default, Color dustColor = default)
        {
            if (offset == default) offset = Vector2.Zero;

            List<Dust> dusts = new();
            for (int i = 0; i < amount; i++)
            {
                float rot = MathHelper.TwoPi * i / amount;
                Vector2 NewVelocity = new Vector2(velocity, 0).RotatedBy(rot);
                Dust dust = Dust.NewDustPerfect(position + offset.RotatedBy(rot), dustType, NewVelocity, newColor: dustColor, Alpha: 25);
                dusts.Add(dust);
            }
            return dusts.ToArray();
        }

        /// <summary>
        /// A helper to spawn a circle of projectile which also includes the handle for multiplayer
        /// </summary>
        /// <param name="owner"> Projectile's Owner </param>
        /// <param name="position"> Dust to spawn in this position </param>
        /// <param name="projectile"> ProjectileID </param>
        /// <param name="damage"> Projectile Damage </param>
        /// <param name="knockback"> Projectile Knockback </param>
        /// <param name="amount"> Amount to spawn in Circle </param>
        /// <param name="velocity"> How fast </param>
        /// <param name="offset"> Padding or offset of the Circle </param>
        /// <returns></returns>
        public static Projectile[] SpawnCircleProjectile(Vector2 position, int projectile, int damage, float knockback, int amount, int owner = -1, float velocity = 6f, Vector2 offset = default)
        {
            if (offset == default) offset = Vector2.Zero;

            List<Projectile> projectiles = new();
            for (int i = 0; i < amount; i++)
            {
                float rot = MathHelper.TwoPi * i / amount;
                Vector2 NewVelocity = new Vector2(velocity, 0).RotatedBy(rot);
                Projectile projectileSpawn = null;
                if (Main.myPlayer == owner) projectileSpawn = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_None(), position, NewVelocity, projectile, damage, knockback, owner)];
                else if (Main.netMode != NetmodeID.MultiplayerClient && owner <= -1) projectileSpawn = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_None(), position, NewVelocity, projectile, damage, knockback)];
                if (projectileSpawn != null) projectiles.Add(projectileSpawn);
            }
            return projectiles.ToArray();
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
