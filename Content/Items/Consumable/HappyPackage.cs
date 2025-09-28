using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.NPCs.DeliveryDrone;
using VanillaModding.Content.NPCs.Sirius;
using VanillaModding.Content.Projectiles.DiceProjectile;

namespace VanillaModding.Content.Items.Consumable
{
    internal class HappyPackage : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            //ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            //ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<DeliveryDrone>();
            NPC.NewNPC(player.GetSource_FromAI(), (int)player.Center.X - (int)Main.rand.NextFloat(-1200, 1200), (int)player.Center.Y - 600, type, ai0: player.whoAmI);
            //NPC.SpawnOnPlayer(player.whoAmI, type);
            return true;
        }
    }
}
