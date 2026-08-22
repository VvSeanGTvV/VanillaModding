using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Accessories;
using VanillaModding.Content.Items.Consumable.Healing;
using VanillaModding.Content.Items.Pets;
using VanillaModding.Content.Items.Weapon.Combo.MightyScythe;
using VanillaModding.Content.Rarities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VanillaModding
{
    internal class VanillaModdingSystem : ModSystem
    {
        public static readonly List<(int id, CustomDrawRarity Rarity)> RarityRender = new();

        public static int DiscoR;
        public static int DiscoG;
        public static int DiscoB;
        private bool DiscoRB, DiscoGB, DiscoBB;
        int Updatetimer = 0;

        public struct SickleData
        {
            public bool isSickle;
            public int[] GrassDrop = new int[2];
            public int[] TallGrassDrop = new int[2];

            /// <summary>
            /// Creates a Sickle Data that allows the item to behave like a sickle, allowing it to cut plants and grass tiles.
            /// By default it uses the vanilla Sickle's drop rate, but you can customize it by changing the min/max values for grass and tall grass drops.
            /// </summary>
            /// <param name="Sickle"></param>
            /// <param name="minGrassDrop"></param>
            /// <param name="maxGrassDrop"></param>
            /// <param name="minTallGrassDrop"></param>
            /// <param name="maxTallGrassDrop"></param>
            public SickleData(bool Sickle = true, int minGrassDrop = 1, int maxGrassDrop = 2, int minTallGrassDrop = 2, int maxTallGrassDrop = 4)
            {
                isSickle = Sickle;
                GrassDrop[0] = minGrassDrop;
                GrassDrop[1] = maxGrassDrop;
                TallGrassDrop[0] = minTallGrassDrop;
                TallGrassDrop[1] = maxTallGrassDrop;
            }
        }
        /// <summary>
        /// Make this item behave like a sickle, allowing it to cut plants and grass tiles.
        /// by Default it uses the vanilla Sickle's drop rate.
        /// </summary>
        public static SickleData[] Sickle = new SickleData[ItemLoader.ItemCount];

        /// <summary>
        /// All the avaliable tile IDs that are considered "hay tiles" for the purpose of sickle cutting. This includes various types of plants and grass tiles.
        /// Incase whether, I want a mod support atleast this is more sustainable and expandable.
        /// </summary>
        public static int[] Grass = new int[]
        {
            TileID.Plants,
            TileID.Plants2,
            TileID.JunglePlants,
            TileID.JunglePlants2,
            TileID.HallowedPlants,
            TileID.HallowedPlants2,
            TileID.CrimsonPlants,
            TileID.CorruptPlants,
            TileID.AshPlants
        };

        public static int[] TallGrass = new int[]
        {
            TileID.Plants2,
            TileID.JunglePlants2,
            TileID.HallowedPlants2
        };

        private void RarityCustomLoad(CustomDrawRarity[] ModdedRarities)
        {
            foreach (CustomDrawRarity Rarity in ModdedRarities) RarityRender.Add(new(Rarity.Type, Rarity));
        }

        public static bool RarityCustomExist(int Rarity) => RarityRender.Any(x => x.id == Rarity);
        public static CustomDrawRarity RarityCustomByID(int Rarity) => RarityRender.Find(x => x.id == Rarity).Rarity;

        public override void Load()
        {
            RarityCustomLoad(
            [
                ModContent.GetInstance<CosmicPurple>(),
                ModContent.GetInstance<HoloRainbow>(),
            ]);
            On_Player.ItemCheck_CutTiles += Hook_ItemCheck_CutTiles;
            On_Player.ItemCheck_PayMana += Hook_ItemCheck_PayMana;
        }

        // During Item before use of Mana
        private bool Hook_ItemCheck_PayMana(On_Player.orig_ItemCheck_PayMana orig, Player self, Item sItem, bool canUse)
        {
            int num = (int)((float)sItem.mana * self.manaCost);
            if (self.armor.Any(i => i.type == ModContent.ItemType<RestorationFlower>()))
            {
                if (self.statMana < num)
                {
                    self.QuickMana();
                    self.statMana -= num;
                }
            }
            else

            if (self.statMana < num) canUse = false;
            return orig(self, sItem, canUse);

        }

        public override void Unload()
        {
            On_Player.ItemCheck_CutTiles -= Hook_ItemCheck_CutTiles;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.RestorationPotion)
                .AddIngredient<LesserRestoration_Potion>(2)
                .AddIngredient(ItemID.GlowingMushroom, 1)
                .AddTile(TileID.Bottles)
                .Register();
        }

        public override void PostAddRecipes()
        {
            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe.HasResult(ItemID.CellPhone))
                {
                    recipe.AddIngredient<ShinyBlackSlab>();
                }
            }
        }

        public override void PostUpdateEverything()
        {
            Updatetimer++;
            if (Updatetimer > 60) Updatetimer = 0;
        }
        
        public override void PreUpdateItems()
        {
            if (Updatetimer % 10 == 0) DoUpdateRGBRarity();
        }

        private void DoUpdateRGBRarity()
        {
            if (!DiscoRB)
            {
                DiscoR++;
                if (DiscoR > 255)
                {
                    DiscoR = 255;
                    DiscoRB = !DiscoRB;
                }
            }
            else
            {
                DiscoR--;
                if (DiscoR < 0)
                {
                    DiscoR = 0;
                    DiscoRB = !DiscoRB;
                }
            }

            if (!DiscoGB)
            {
                DiscoG++;
                if (DiscoG > 255)
                {
                    DiscoG = 255;
                    DiscoGB = !DiscoGB;
                }
            }
            else
            {
                DiscoG--;
                if (DiscoG < 0)
                {
                    DiscoG = 0;
                    DiscoGB = !DiscoGB;
                }
            }

            if (!DiscoBB)
            {
                DiscoB++;
                if (DiscoB > 255)
                {
                    DiscoB = 255;
                    DiscoBB = !DiscoBB;
                }
            }
            else
            {
                DiscoB--;
                if (DiscoB < 0)
                {
                    DiscoB = 0;
                    DiscoBB = !DiscoBB;
                }
            }
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
                    bool isHayTile = Grass.Contains(type);
                    bool isTallGrassTile = TallGrass.Contains(type);

                    if (!isHayTile) 
                        continue;

                    // Only items can make grass/plants drop hay if they are marked as sickles
                    if (!Sickle[sItem.type].isSickle)
                        continue;

                    // This is where it cuts the tile and checks in World Gen whether this can cut tile.
                    if (Main.tileCut[type] && WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0))
                    {
                        // Amount of hay taken from Sickle Data
                        int amount =
                           (isTallGrassTile)
                           ? Main.rand.Next(Sickle[sItem.type].TallGrassDrop[0], Sickle[sItem.type].TallGrassDrop[1])
                           : Main.rand.Next(Sickle[sItem.type].GrassDrop[0], Sickle[sItem.type].GrassDrop[1]);

                        // Spawn this hay item along with ID
                        int id = Item.NewItem(
                            new EntitySource_TileBreak(x * 16, y * 16),
                            x * 16, y * 16, 0, 0,
                            ItemID.Hay,
                            amount,
                            false,
                            -1
                        );

                        // When on Multiplayer sync the item drop to all clients
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, id, 1f);
                    }
                }
            }

            orig(self, sItem, itemRectangle, shouldIgnore);
        }

        public override void AddRecipeGroups() //uh
        {

        }
    }
}
