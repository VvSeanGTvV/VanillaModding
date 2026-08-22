using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;
using static VanillaModding.Content.Rarities.CosmicPurple;

namespace VanillaModding.Content.Rarities
{
    public class CustomDrawRarity : ModRarity
    {

        public virtual Color BloomClr => new Color(65, 65, 65, 0);
        public virtual Color TextClr => new Color(255, 255, 255, 255);

        public virtual void Update()
        {

        }

        public virtual void Draw(Item Item, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale, Color? textColor = null, Color? lightColor = null, bool? renderSpecialEffects = null)
        {
            Draw(Item, Main.spriteBatch, text, X, Y, Colors.AlphaDarken(textColor ?? TextClr), lightColor ?? BloomClr, rotation, origin, baseScale, Main.GlobalTimeWrappedHourly,
                renderSpecialEffects ?? false, FontAssets.MouseText.Value);
        }

        public virtual void Draw(Item Item, DrawableTooltipLine line)
        {
            Draw(Item, line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale);
        }

        public virtual void Draw(Item Item, SpriteBatch spriteBatch, string text, int X, int Y, Color textColor, Color lightColor, float rotation,
            Vector2 origin, Vector2 baseScale, float time, bool renderSpecialEffects, DynamicSpriteFont font)
        {

        }

        public virtual void SpecialDraw(string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale, Color? textColor = null, Color? lightColor = null, bool? renderSpecialEffects = null)
        => SpecialDraw(Main.spriteBatch, text, X, Y, rotation, origin, baseScale, Colors.AlphaDarken(textColor ?? TextClr), lightColor ?? BloomClr, Main.GlobalTimeWrappedHourly, FontAssets.MouseText.Value);

        public virtual void SpecialDraw(DrawableTooltipLine line)
        => SpecialDraw(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale);

        public virtual void SpecialDraw(SpriteBatch spriteBatch, string text, int X, int Y, float rotation, Vector2 origin, Vector2 baseScale, Color textColor, Color lightColor, float time, DynamicSpriteFont font)
        {

        }
    }
}
