using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VanillaModding.Common.Utilities
{
    internal class DelegateMethodsHelper
    {
        public static bool CutTiles(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return false;

            if (Main.tile[x, y] == null)
                return false;

            if (!Main.tileCut[Main.tile[x, y].TileType])
                return true;

            if (DelegateMethods.tileCutIgnore[Main.tile[x, y].TileType])
                return true;

            if (WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0))
            {
                WorldGen.KillTile(x, y);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
            }

            return true;
        }

        public static bool CutTilesHay(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return false;

            if (Main.tile[x, y] == null)
                return false;

            if (!Main.tileCut[Main.tile[x, y].TileType])
                return true;

            if (DelegateMethods.tileCutIgnore[Main.tile[x, y].TileType])
                return true;

            Tile tile = Framing.GetTileSafely(x, y);
            int type = tile.TileType;

            // Vanilla tile cutting logic
            if (WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0))
            {
                // Kill tile
                WorldGen.KillTile(x, y, false, false, true);

                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);

                // --- CUSTOM HAY DROP LOGIC ---
                // These are the same tiles vanilla uses for Hay
                bool isHayTile =
                    type == TileID.Plants ||
                    type == TileID.Plants2 ||
                    type == TileID.JunglePlants ||
                    type == TileID.JunglePlants2 ||
                    type == TileID.HallowedPlants ||
                    type == TileID.HallowedPlants2 ||
                    type == TileID.CrimsonPlants ||
                    type == TileID.CorruptPlants ||
                    type == TileID.AshPlants;

                if (isHayTile)
                {
                    int amount =
                        (type == TileID.Plants2 ||
                         type == TileID.JunglePlants2 ||
                         type == TileID.HallowedPlants2)
                        ? Main.rand.Next(4, 6)
                        : Main.rand.Next(2, 4);

                    int id = Item.NewItem(
                        Entity.GetSource_None(),
                        new Vector2(x, y),
                        ItemID.Hay,
                        amount
                    );

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, id, 1f);
                }
            }

            return true;
        }
    }
}
