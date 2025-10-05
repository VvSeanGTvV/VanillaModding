using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Projectiles.DiceProjectile;

namespace VanillaModding.Content.Items.Consumable
{
    internal class RedEnvelope : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
        }

        public override bool CanRightClick()
            => true;

        /// <summary>
        /// Quick calculation and spawn of the appropriate coins based on the total amounts provided.
        /// </summary>
        public void QuickSpawnCoins(Player player, int totalCopper = 0, int totalSilver = 0, int totalGold = 0, int totalPlatinum = 0)
        {
            // Convert all coin values to total copper
            int totalValueInCopper =
                totalCopper +
                (totalSilver * 100) +
                (totalGold * 10000) +
                (totalPlatinum * 1000000);

            // Now calculate the number of coins for each denomination
            int platinum = totalValueInCopper / 1000000;
            totalValueInCopper %= 1000000;

            int gold = totalValueInCopper / 10000;
            totalValueInCopper %= 10000;

            int silver = totalValueInCopper / 100;
            totalValueInCopper %= 100;

            int copper = totalValueInCopper;

            // Spawn each type if greater than 0
            IEntitySource source = player.GetSource_OpenItem(Type);

            if (platinum > 0)
                player.QuickSpawnItem(source, ItemID.PlatinumCoin, platinum);
            if (gold > 0)
                player.QuickSpawnItem(source, ItemID.GoldCoin, gold);
            if (silver > 0)
                player.QuickSpawnItem(source, ItemID.SilverCoin, silver);
            if (copper > 0)
                player.QuickSpawnItem(source, ItemID.CopperCoin, copper);
        }

        public override void RightClick(Player player)
        {
            QuickSpawnCoins(player, Main.rand.Next(5, 500)); // Random amount between 1 silver and 1 gold
        }
    }
}
