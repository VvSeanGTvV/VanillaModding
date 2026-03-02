using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Content.Dusts;

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
    }
}
