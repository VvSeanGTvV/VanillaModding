using System;
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
using VanillaModding.Content.Buffs;

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

    internal class VanillaModdingNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// NPC, currently rolling a Dice
        /// </summary>
        public bool rolling = false;
        /// <summary>
        /// Has any existing Debuff/buff 
        /// </summary>
        public bool hasAnyDiceEffect = false;
        /// <summary>
        /// Dice number that has been rolled
        /// </summary>
        public int DiceMult;
        /// <summary>
        /// Total rolls, also used for dice incremental chance of death outcome.
        /// </summary>
        public int totalRolls;

        /// <summary>
        /// Adjustable Life Max by any buffs.
        /// </summary>
        public int statLifeMax2;
        /// <summary>
        /// Adjustable Defense by any buffs.
        /// </summary>
        public int statDefenseMax2;

        /// <summary>
        /// Not adjustable usually never meant to be adjusted at all times.
        /// </summary>
        private int statLifeMax = -1, statDefense = -1;

        public override bool PreAI(NPC npc)
        {
            if (statLifeMax <= 0) statLifeMax = npc.lifeMax;
            if (statDefense <= 0) statDefense = npc.defense;
            return base.PreAI(npc);
        }

        public override void ResetEffects(NPC npc)
        {
            statLifeMax2 = 0;
            statDefenseMax2 = 0;
            //hasAnyDiceEffect = false;
            base.ResetEffects(npc);
        }

        public override void AI(NPC npc)
        {
            hasAnyDiceEffect = npc.HasBuff<DiceBuff>() || npc.HasBuff<DiceDebuff>();
            npc.defense = statDefense + statDefenseMax2;
            npc.lifeMax = statLifeMax + statLifeMax2;
            base.AI(npc);
        }
    }
}
