using CustomRarityLib.Rarity;
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
using static System.Net.Mime.MediaTypeNames;
using static VanillaModding.Content.Rarities.CosmicPurple;

namespace VanillaModding.Content.Rarities
{
    internal class HoloRainbow : CustomDrawRarity
    {
        private static float duration = ((float)(Main.GlobalTimeWrappedHourly * Math.PI));
        public override Color RarityColor => new Color(
                                        ((float)Math.Sin((((duration - (Math.PI / 1.5)) / 2) - 0.1f) * 0.5)),
                                        (((float)Math.Cos(((duration / 2) - 0.1f) * 0.5))),
                                        ((float)Math.Sin((((duration + (Math.PI / 1.5)) / 2) - 0.1f) * 0.5) * -1)
                                        );

        public override Color TextColor => defColor;

        public static Color defColor = new Color(255, 255, 255, 255);

        public override void Update()
        {
            duration = ((float)(Main.GlobalTimeWrappedHourly * Math.PI) * 1.25f);
        }

        public sealed class HoloRainbowTextSnippet(string text) : TextSnippet
        {
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
            {
                size = new Vector2(GetStringLength(FontAssets.MouseText.Value), FontAssets.MouseText.Value.MeasureString(" ").Y * scale);

                if (color == default || color == Main.MouseTextColorReal)
                {
                    color = Colors.AlphaDarken(defColor);
                }

                if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
                {
                    var pos = position;
                    var font = FontAssets.MouseText.Value;
                    string txt = "";
                    float b = 0;

                    color.A = 255;
                    foreach (var item in text)
                    {
                        pos = position;
                        pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;

                        b += 0.1f;
                        color = new Color(
                                        ((float)Math.Sin((((duration - (Math.PI / 1.5)) / 2) + b - 0.1f) * 0.5)),
                                        (((float)Math.Cos(((duration / 2) + b - 0.1f) * 0.5))),
                                        ((float)Math.Sin((((duration + (Math.PI / 1.5)) / 2) + b - 0.1f) * 0.5) * -1)
                                        );
                        for (int i = 0; i < 4; i++)
                        {
                            float j2 = duration * 0.75f % MathHelper.TwoPi;
                            ChatManager.DrawColorCodedString(spriteBatch, font, item.ToString(), pos + new Vector2(3f, 0).RotatedBy(MathHelper.ToRadians(90 * i) + j2), Color.Lerp(Color.Black, color, 0.2f), 0, Vector2.Zero, new(scale));
                        }
                        txt += item;
                    }
                    b = 0;
                    txt = "";
                    foreach (var item in text)
                    {
                        pos = position;
                        pos.X += FontAssets.MouseText.Value.MeasureString(txt).X;

                        b += 0.1f;
                        color = new Color(
                                    ((float)Math.Sin((((duration - (Math.PI / 1.5)) / 2) + b - 0.1f) * 0.5)),
                                    (((float)Math.Cos(((duration / 2) + b - 0.1f) * 0.5))),
                                    ((float)Math.Sin((((duration + (Math.PI / 1.5)) / 2) + b - 0.1f) * 0.5) * -1)
                                    );
                        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, item.ToString(), pos, Color.Lerp(Color.Black, color, 0.4f), 0, Vector2.Zero, new(scale));
                        ChatManager.DrawColorCodedString(spriteBatch, font, item.ToString(), pos, Color.Lerp(Color.White, color, 0.5f), 0, Vector2.Zero, new(scale));
                        txt += item;
                    }
                }
                return true;
            }
            public override float GetStringLength(DynamicSpriteFont font)
            {
                float size = font.MeasureString(text).X;
                return size * Scale;
            }
        }

        public override void Draw(Item Item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
            Vector2 origin, Vector2 baseScale, float time, bool renderSpecialEffects, DynamicSpriteFont font)
        {
            var fontSize = ChatManager.GetStringSize(font, text, new Vector2(1));
            var center = fontSize / 2f;
            if (Item.expert) textColor = Main.DiscoColor;

            // Get all snippets and convert all plain text snippets to the custom rarity snippet
            TextSnippet[] snippets = ChatManager.ParseMessage(text, textColor).ToArray();
            for (int i = 0; i < snippets.Length; i++)
            {
                TextSnippet textSnippet = snippets[i];
                if (snippets[i].GetType() == typeof(TextSnippet))
                {
                    snippets[i] = new HoloRainbowTextSnippet(textSnippet.Text);
                }
            }

            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new(X, Y), textColor, 0, Vector2.Zero, baseScale, out _, -1, true);
        }
    }
}
