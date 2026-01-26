using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace VanillaModding.Content.Dusts.SMASH
{
    internal class SMASH : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
             
            dust.frame = new Rectangle(0, 0, 142, 30);
            dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
            dust.noGravity = true; // Makes the dust have no gravity.
            dust.noLight = true; // Makes the dust emit no light.
            dust.scale *= 0.85f; // Multiplies the dust's initial scale by 1.5.
        }

        public override bool Update(Dust dust)
        {
            // Init lifetime counter
            dust.customData ??= 0f;
            dust.customData = (float)dust.customData + 1f;

            float time = (float)dust.customData;

            // Movement
            dust.position.Y -= 0.35f;

            // ---- PULSE SETUP ----
            float min = 0.5f;
            float max = 1f;
            float speed = 0.25f;

            // Pulse fade (1 → 0)
            float pulseFade = MathHelper.Clamp(1f - time / 120f, 0f, 1f);

            // Center + amplitude
            float center = (min + max) * 0.5f;
            float amplitude = (max - min) * 0.5f * pulseFade;

            dust.scale = center + amplitude * (float)Math.Sin(Main.GameUpdateCount * speed);

            // ---- ALPHA FADE ----
            dust.alpha += 2; // higher = faster fade
            dust.alpha = Math.Min(dust.alpha, 255);

            // ---- LIGHT ----
            float lightStrength = 0.35f * pulseFade;
            Lighting.AddLight(dust.position, lightStrength, lightStrength, lightStrength);

            // ---- KILL CONDITIONS ----
            if (dust.alpha >= 255)
            {
                dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = dust.frame;

            Vector2 origin = frame.Size() * 0.5f;

            // Convert dust.position (top-left) → center
            Vector2 center = dust.position + origin;

            Vector2 drawPos = center - Main.screenPosition;

            float time = (float)dust.customData;
            float lightStrength = 0.5f * (1f - time / 120f);
            Color color = dust.GetAlpha(new Color(lightStrength*255, lightStrength * 255, lightStrength * 255, dust.alpha));

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                frame,
                color,
                dust.rotation,
                origin,
                dust.scale,
                SpriteEffects.None,
                0f
            );

            return false; // skip vanilla dust drawing
        }

    }
}
