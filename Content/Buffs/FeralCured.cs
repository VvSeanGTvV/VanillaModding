using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.GlobalNPCs;

namespace VanillaModding.Content.Buffs
{
    internal class FeralCured : ModBuff
    {

        public float boostDamage = 0.15f;
        public override LocalizedText Description => base.Description.WithFormatArgs((int)Math.Floor(boostDamage*100));
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 4;
            player.GetDamage(DamageClass.Generic) += boostDamage;
            if (player.HasBuff(BuffID.Rabies)) player.ClearBuff(BuffID.Rabies);

            if (player.HasBuff(BuffID.Darkness)) player.ClearBuff(BuffID.Darkness);
            if (player.HasBuff(BuffID.Cursed)) player.ClearBuff(BuffID.Cursed);
            if (player.HasBuff(BuffID.Confused)) player.ClearBuff(BuffID.Confused);
            if (player.HasBuff(BuffID.Weak)) player.ClearBuff(BuffID.Weak);
            if (player.HasBuff(BuffID.Silenced)) player.ClearBuff(BuffID.Silenced);
        }

        public void ClearBuffNPC(NPC npc, int buff)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server) npc.DelBuff(BuffID.Rabies);
            else npc.RequestBuffRemoval(BuffID.Rabies);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen += 4;
            npc.damage = (int)(npc.damage * (1f + boostDamage));
            if (npc.HasBuff(BuffID.Rabies)) ClearBuffNPC(npc, BuffID.Rabies);

            if (npc.HasBuff(BuffID.Darkness)) ClearBuffNPC(npc, BuffID.Darkness);
            if (npc.HasBuff(BuffID.Cursed)) ClearBuffNPC(npc, BuffID.Cursed);
            if (npc.HasBuff(BuffID.Confused)) ClearBuffNPC(npc, BuffID.Confused);
            if (npc.HasBuff(BuffID.Weak)) ClearBuffNPC(npc, BuffID.Weak);
            if (npc.HasBuff(BuffID.Silenced)) ClearBuffNPC(npc, BuffID.Silenced);
        }
    }
}
