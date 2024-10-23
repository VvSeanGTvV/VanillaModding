using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using VanillaModding.Items.Weapon.Melee.BloodyScythe;
using VanillaModding.Items.Ammo;
using VanillaModding.Items.Weapon.Ranged;

namespace VanillaModding.GlobalModification
{
    internal class ModifyGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Blood Moon
            if (npc.type == NPCID.BloodZombie || npc.type == NPCID.Drippler) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyScythe>(), 2));
            if (npc.type == NPCID.GoblinShark || npc.type == NPCID.BloodEelHead) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyScythe>(), 10));
            if (npc.type == NPCID.BloodNautilus) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyScythe>(), 50));

            if (npc.type == NPCID.MartianSaucerCore) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhasicWarpEjector>(), 25));
            //base.ModifyNPCLoot(npc, npcLoot);

        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Cyborg)
            {
                shop.Add<PhasicDisc_Ammo>();
            }
        }
    }
}
