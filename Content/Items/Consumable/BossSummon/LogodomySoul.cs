using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Materials;
using VanillaModding.Content.Items.Placeable;
using VanillaModding.Content.NPCs.LobotomyGod;
using VanillaModding.Content.NPCs.Sirius;
using VanillaModding.Content.Tiles;

namespace VanillaModding.Content.Items.Consumable.BossSummon
{
    internal class LogodomySoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity

            Item.ResearchUnlockCount = 5;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
        }

        public override bool CanUseItem(Player player)
        {
            // Prevent duplicate bosses
            if (NPC.AnyNPCs(ModContent.NPCType<LobotomyGod>()) ||
                NPC.AnyNPCs(ModContent.NPCType<LobotomySpiritLife>()))
                return false;

            // Tile the player is pointing at
            Point tilePos = Main.MouseWorld.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(tilePos);

            return tile.HasTile &&
                   tile.TileType == ModContent.TileType<LogodomyAltarTile>();
        }

        private Point? FindNearbyAltar(Player player)
        {
            int playerTileX = (int)(player.Center.X / 16);
            int playerTileY = (int)(player.Center.Y / 16);

            for (int x = playerTileX - 20; x <= playerTileX + 20; x++)
            {
                for (int y = playerTileY - 20; y <= playerTileY + 20; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    if (tile.HasTile &&
                        tile.TileType == ModContent.TileType<LogodomyAltarTile>())
                    {
                        return new Point(x, y);
                    }
                }
            }

            return null;
        }

        public override bool? UseItem(Player player)
        {
            Point? altar = FindNearbyAltar(player);

            if (!altar.HasValue)
                return false;

            Point p = altar.Value;

            // Center of the tile
            int spawnX = p.X * 16 + 24;
            int spawnY = p.Y * 16 + 16;

            SoundEngine.PlaySound(SoundID.AbigailSummon, new Vector2(spawnX, spawnY));

            NPC.NewNPC(
                    player.GetSource_ItemUse(Item),
                    spawnX,
                    spawnY,
                    ModContent.NPCType<LobotomySpiritLife>());

            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0f * 0.97f, 1f * 0.97f, 0f * 0.97f, 0.5f);
        }
    }
}
