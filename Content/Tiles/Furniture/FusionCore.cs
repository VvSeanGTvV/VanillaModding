using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace VanillaModding.Content.Tiles.Furniture
{
    internal class FusionCore : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            //Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            DustType = DustID.IceTorch;
            AdjTiles = [TileID.WorkBenches, TileID.Furnaces, TileID.AdamantiteForge, TileID.Anvils, TileID.MythrilAnvil];
            Main.tileLighted[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            //TileObjectData.newTile.CoordinateHeights = [18];
            TileObjectData.addTile(Type);

            AnimationFrameHeight = 54;

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(136, 233, 255), name);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = (136 / 255f) * 0.75f;
            g = (233 / 255f) * 0.75f;
            b = (255 / 255f) * 0.75f;
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            // We can change frames manually, but since we are just simulating a different tile, we can just use the same value
            frameCounter++;
            if (frameCounter > 6)
            {
                frameCounter = 0;
                frame++;
                if (frame > 3)
                {
                    frame = 0;
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            // If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
            // The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting modes
            // While at 100% world zoom, press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Because height of third tile is different we change it
            int height = 16;

            // Offset along the Y axis depending on the current frame
            int frameYOffset = (Main.tileFrame[Type]) * AnimationFrameHeight;

            // Firstly we draw the original texture and then glow mask texture
            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            return false;
        }
    }
}

