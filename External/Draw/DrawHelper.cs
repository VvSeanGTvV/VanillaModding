using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Microsoft.Xna.Framework;

namespace VanillaModding.External.Draw
{
    internal class DrawHelper
    {
        public static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D value = TextureAssets.Extra[98].Value;
            Color color = shineColor * opacity * 0.5f;
            color.A = 0;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
            Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
            color *= num;
            color2 *= num;
            Main.EntitySpriteDraw(value, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
            Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
        }

        public static void DrawGridInsideCircle(
            Vector2 center,
            float radius,
            int tileSize,
            Color color,
            float thickness = 1f
        )
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            int max = (int)(radius / tileSize) + 1;

            // Vertical lines
            for (int i = -max; i <= max; i++)
            {
                float xOffset = i * tileSize;
                float xSquared = xOffset * xOffset;

                if (xSquared > radius * radius)
                    continue;

                float yLimit = (float)Math.Sqrt(radius * radius - xSquared);

                Vector2 start = center + new Vector2(xOffset, -yLimit);
                Vector2 end = center + new Vector2(xOffset, yLimit);

                DrawLine(start, end, color, thickness);
            }

            // Horizontal lines
            for (int i = -max; i <= max; i++)
            {
                float yOffset = i * tileSize;
                float ySquared = yOffset * yOffset;

                if (ySquared > radius * radius)
                    continue;

                float xLimit = (float)Math.Sqrt(radius * radius - ySquared);

                Vector2 start = center + new Vector2(-xLimit, yOffset);
                Vector2 end = center + new Vector2(xLimit, yOffset);

                DrawLine(start, end, color, thickness);
            }
        }

        public static void DrawDashedCircle(Vector2 center, float radius, int segments, int dashSize, Color color, float thickness = 2f)
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            float increment = MathHelper.TwoPi / segments;
            Vector2 previous = center + new Vector2(radius, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = increment * i;
                Vector2 current = center + radius * angle.ToRotationVector2();

                // This creates the dash pattern
                if ((i / dashSize) % 2 == 0)
                {
                    Vector2 edge = current - previous;
                    float rotation = edge.ToRotation();

                    Main.spriteBatch.Draw(
                        pixel,
                        previous - Main.screenPosition,
                        new Rectangle(0, 0, 1, 1),
                        color,
                        rotation,
                        new Vector2(0f, 0.5f),
                        new Vector2(edge.Length(), thickness),
                        SpriteEffects.None,
                        0f
                    );
                }

                previous = current;
            }
        }

        public static void DrawCircle(Vector2 center, float radius, int segments, Color color, float thickness = 2f)
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            float increment = MathHelper.TwoPi / segments;

            Vector2 previous = center + new Vector2(radius, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = increment * i;

                Vector2 current = center + radius * angle.ToRotationVector2();

                Vector2 edge = current - previous;
                float rotation = edge.ToRotation();

                Main.spriteBatch.Draw(
                    pixel,
                    previous - Main.screenPosition,
                    new Rectangle(0, 0, 1, 1),
                    color,
                    rotation,
                    new Vector2(0f, 0.5f),
                    new Vector2(edge.Length(), thickness),
                    SpriteEffects.None,
                    0f
                );

                previous = current;
            }
        }

        private static void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            Vector2 edge = end - start;
            float rotation = edge.ToRotation();
            float length = edge.Length();

            Main.spriteBatch.Draw(
                pixel,
                start - Main.screenPosition,
                new Rectangle(0, 0, 1, 1),
                color,
                rotation,
                new Vector2(0f, 0.5f), // IMPORTANT: center vertically
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
        }

        internal static void DrawGrid(Vector2 center, float range1, float range2, int v1, Color color, float v2)
        {
            throw new NotImplementedException();
        }
    }
}
