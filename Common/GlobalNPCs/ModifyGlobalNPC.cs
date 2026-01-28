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

        /// Not adjustable usually never meant to be adjusted at all times.
        public int defLifeMax { get; private set; }
        public int statDefense { get; private set; }

        /// <summary>
        /// Aggressive to a specific Player, useful for any critters that wants to become hostile upon hit.
        /// </summary>
        public int aggroTo = -1;
        /// <summary>
        /// Been Attacked recently.
        /// </summary>
        public bool attacked = false;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            defLifeMax = npc.lifeMax;
        }

        public override void ResetEffects(NPC npc)
        {
            statLifeMax2 = 0;
            statDefenseMax2 = 0;
            //hasAnyDiceEffect = false;
            base.ResetEffects(npc);
        }

        public override void PostAI(NPC npc)
        {
            int newLifeMax = defLifeMax + statLifeMax2;
            bool changableFromLifeMax = newLifeMax > 0;

            if (npc.lifeMax != newLifeMax && changableFromLifeMax)
            {
                float lifeRatio = (float)npc.life / npc.lifeMax;
                npc.lifeMax = newLifeMax;
                npc.life = (int)(npc.lifeMax * lifeRatio);
            }
            if (defLifeMax != npc.lifeMax && !changableFromLifeMax) defLifeMax = npc.lifeMax;

            npc.defense = npc.defDefense + statDefenseMax2;
            if ((statDefense != npc.defense && statDefenseMax2 == 0)) statDefense = npc.defense;
            base.PostAI(npc);
        }

        public override void AI(NPC npc)
        {
            hasAnyDiceEffect = npc.HasBuff<DiceBuff>() || npc.HasBuff<DiceDebuff>();
            


            // Checking if that aggro number is actually valid and not some junk data or dead person.
            if (aggroTo >= 0)
            {
                Player aggroToPlayer = Main.player[aggroTo];
                if (aggroToPlayer != null)
                {
                    if (aggroToPlayer.dead || !aggroToPlayer.active) aggroTo = -1;
                }
                else aggroTo = -1;
            }
            base.AI(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            attacked = true;
            aggroTo = player.whoAmI;
            
            base.OnHitByItem(npc, player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.owner >= 0)
            {
                attacked = true;
                aggroTo = projectile.owner;
            }
            
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }
    }
}
