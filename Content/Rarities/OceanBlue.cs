using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static VanillaModding.Content.Rarities.CosmicPurple;

namespace VanillaModding.Content.Rarities
{
    internal class OceanBlue : CustomDrawRarity
    {
        public override Color RarityColor => CTX;
        public override Color TextClr => CTX;
        private static float duration = ((float)(Main.GlobalTimeWrappedHourly * Math.PI));
        public override Color BloomClr => new Color(122, 222, 255);
        public static Color CTX = new Color(20, 197, 255);

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
            {
                return ItemRarityID.Red; // todo a somethin
            }

            return Type; // no 'lower' tier to go to, so return the type of this rarity.
        }
        public sealed class OceanTextSnippet(string text) : TextSnippet
        {
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
            {
                size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

                if (color == default || color == Main.MouseTextColorReal)
                {
                    color = Colors.AlphaDarken(CTX);
                }

                if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
                {
                    var font = FontAssets.MouseText.Value;
                    color.A = 255;

                    color = new Color(20 + (int)(69 * Math.Abs(Math.Sin(duration / 2f))), 197 + (int)(27 * Math.Abs(Math.Sin(duration / 2f))), 255);
                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, color * 2f, 0, Vector2.Zero, new(scale));
                    ChatManager.DrawColorCodedString(spriteBatch, font, text, position, Color.Lerp(Color.Black, color, 0.2f) * 1.5f, 0, Vector2.Zero, new(scale));
                }
                return true;
            }
            public override float GetStringLength(DynamicSpriteFont font)
            {
                float size = font.MeasureString(text).X;
                return size * Scale;
            }
        }
        public override void Update()
        {
            duration = ((float)(Main.GlobalTimeWrappedHourly * Math.PI) * 1.25f);
        }


        public override void Draw(Item Item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
            Vector2 origin, Vector2 baseScale, float time, bool renderSpecialEffects, DynamicSpriteFont font)
        {
            var crystalTextGlow = ModContent.Request<Texture2D>(nameof(VanillaModding) + "/" + "Assets/UI/CrystalTextGlow").Value;
            var fontSize = ChatManager.GetStringSize(font, text, new Vector2(1));
            var center = fontSize / 2f;

            // Get all snippets and convert all plain text snippets to the custom rarity snippet
            TextSnippet[] snippets = ChatManager.ParseMessage(text, textColor).ToArray();
            for (int i = 0; i < snippets.Length; i++)
            {
                TextSnippet textSnippet = snippets[i];
                if (snippets[i].GetType() == typeof(TextSnippet))
                {
                    snippets[i] = new OceanTextSnippet(textSnippet.Text);
                }
            }

            //Draw backglow
            var glowPosition = new Vector2(X + center.X, Y + center.Y / 1.5f);
            spriteBatch.Draw(crystalTextGlow, glowPosition, null, lightColor, rotation + MathHelper.PiOver2, new Vector2(6f, 33f),
               new Vector2(1.6f, fontSize.X / crystalTextGlow.Height * 1.2f), SpriteEffects.None, 0f);

            //Draw text
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new(X, Y), textColor, 0, Vector2.Zero, baseScale, out _, -1, true);
        }

        public override void SpecialDraw(SpriteBatch spriteBatch, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale, Color textColor, Color lightColor, float time, DynamicSpriteFont font)
        {
            var bubble = ModContent.Request<Texture2D>(nameof(VanillaModding) + "/" + "Assets/UI/Bubble").Value;
            var fontSize = ChatManager.GetStringSize(font, text, new Vector2(1));
            rand.SetSeed(1);

            int bubbleCount = rand.Next((int)fontSize.X / 7, (int)fontSize.X / 5) + 1;
            var color2 = lightColor;
            color2.A = 0;
            var bubbleOrigin = new Vector2(13, 13);
            for (int i = 0; i < bubbleCount; i++)
            {
                var v = new Vector2(rand.NextFloat(fontSize.X), rand.NextFloat(fontSize.Y * 0.6f) + 1f);
                float lifeTime = Main.GlobalTimeWrappedHourly * 4f + rand.NextFloat(MathHelper.TwoPi);
                lifeTime %= MathHelper.TwoPi;

                if (lifeTime > MathHelper.Pi)
                    continue;

                float sinValue = (float)Math.Sin(lifeTime);
                var white = (Color.Lerp(textColor, Color.White, 0.25f) * 1.5f) * sinValue;

                spriteBatch.Draw(bubble, new Vector2(X + (float)Math.Sin(duration), Y - lifeTime * MaxY + 3f) + v, null, white, 0, bubbleOrigin,
                    (float)lifeTime / MathHelper.TwoPi * 0.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(bubble, new Vector2(X + (float)Math.Cos(duration), Y - lifeTime * MaxY + 2f) + v, null, white * 0.5f, 0, bubbleOrigin,
                    (float)lifeTime / MathHelper.TwoPi, SpriteEffects.None, 0f);
            }
        }
    }
}
