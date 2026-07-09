using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Weapon.Combo.MightyScythe;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VanillaModding
{
    internal class VanillaModdingSystem : ModSystem
    {
        /// <summary>
        /// Make this item behave like a sickle, allowing it to cut plants and grass tiles. Do note, this is ONLY on vanilla plants/grass tiles.
        /// </summary>
        public static bool[] Sickle = new bool[ItemLoader.ItemCount];
        public override void Load()
        {
            On_Player.ItemCheck_CutTiles += Hook_ItemCheck_CutTiles;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_CutTiles -= Hook_ItemCheck_CutTiles;
        }

        private void Hook_ItemCheck_CutTiles(On_Player.orig_ItemCheck_CutTiles orig, Player self, Item sItem, Rectangle itemRectangle, bool[] shouldIgnore)
        {
            int startX = itemRectangle.Left / 16;
            int endX = itemRectangle.Right / 16;
            int startY = itemRectangle.Top / 16;
            int endY = itemRectangle.Bottom / 16;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    int type = tile.TileType;

                    //Mod.Logger.Debug($"x:{itemRectangle.Center.X} y:{itemRectangle.Center.Y} id:{type}");
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

                    if (!isHayTile) 
                        continue;

                    // Only items can make grass/plants drop hay if they are marked as sickles
                    if (!Sickle[sItem.type])
                        continue;

                    if (Main.tileCut[type] && WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0))
                    {
                        Mod.Logger.Debug($"x:{x} y:{y} | xp:{self.position.X} yp:{self.position.Y} | id:{type}");
                        int amount =
                           (type == TileID.Plants2 ||
                            type == TileID.JunglePlants2 ||
                            type == TileID.HallowedPlants2)
                           ? Main.rand.Next(2, 4)
                           : Main.rand.Next(1, 2);

                        int id = Item.NewItem(
                            new EntitySource_TileBreak(x * 16, y * 16),
                            x * 16, y * 16, 0, 0,
                            ItemID.Hay,
                            amount,
                            false,
                            -1
                        );

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, id, 1f);
                    }
                }
            }

            orig(self, sItem, itemRectangle, shouldIgnore);
        }

        public override void AddRecipeGroups()
        {

        }
    }
}
