﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using VanillaModding.Content.Items.Weapon.Melee.BloodyScythe;
using VanillaModding.Content.Items.Ammo;
using VanillaModding.Content.Items.Weapon.Ranged;
using Microsoft.Xna.Framework;
using VanillaModding.Content.Items.Consumable;

namespace VanillaModding.Common.GlobalNPCs
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

            if (shop.NpcType == NPCID.Mechanic)
                shop.Add(ModContent.ItemType<HappyPackage>(), Condition.DownedMechBossAny);
        }
    }

    internal class StunnedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool stunned;
        private Rectangle NpcFrame;

        public override void ResetEffects(NPC npc)
            => stunned = false;

        public override void SetDefaults(NPC npc)
        {
            if (!stunned)
                NpcFrame = npc.frame;
            else
                npc.frame = NpcFrame;
        }

        public override bool PreAI(NPC npc)
        {
            if (stunned)
            {
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.ai[0] = 0;
                npc.ai[1] = 0;

                npc.velocity = new Vector2(0, 6);
                npc.frameCounter = 0;
                npc.noGravity = false;
                return false;
            }
            return base.PreAI(npc);
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (stunned)
                return;
        }
    }
}
