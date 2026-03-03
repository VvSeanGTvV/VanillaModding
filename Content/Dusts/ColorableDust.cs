using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.Dusts
{
    internal class ColorableDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += 0.1f;
            dust.velocity.Y *= 0.74f;
            dust.velocity.X *= 0.84f;
            //dust.velocity.X += 0.05f;
            dust.alpha += 5;
            ReduceScale(dust);
            if (!dust.noLight)
            {
                Color color = (Color)GetAlpha(dust, dust.color * 0.5f);
                if (color == null) return false;
                Lighting.AddLight(dust.position, color.ToVector3());
            }
            return false;
        }

        protected virtual void ReduceScale(Dust dust)
        {
            dust.scale *= 0.98f;
            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * ((255 - dust.alpha) / 255f);
        }
    }
}
