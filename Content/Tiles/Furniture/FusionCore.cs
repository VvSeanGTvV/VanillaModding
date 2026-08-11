using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
    }
}
