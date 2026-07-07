using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Common;

namespace VanillaModding.Content.Buffs
{
    internal class Adrenaline : ModBuff
    {
        public static readonly int DamageOffset = 75;
        public static readonly int KnockbackOffset = 75;
        public static readonly int SpeedOffset = 125;

        public override void Update(Player player, ref int buffIndex)
        {
            VanillaModdingPlayer VMP = player.GetModPlayer<VanillaModdingPlayer>();
            if (VMP != null) VMP.NaturalAdrenaline = false;
            player.lifeRegen += 20;
            player.moveSpeed += 0.8f;
            player.GetDamage<GenericDamageClass>() += DamageOffset / 100f;
            player.GetAttackSpeed<GenericDamageClass>() += SpeedOffset / 100f;
            player.GetKnockback<GenericDamageClass>() += (KnockbackOffset / 2f) / 100f;
        }
    }

    internal class AdrenalineExhausted : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = false; // This buff can be applied by other players in Pvp, so we need this to be true.
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true; // This buff is a debuff, so we need this to be true.
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen = 1;
            player.moveSpeed *= 0.85f;
            player.GetDamage<GenericDamageClass>() -= (Adrenaline.DamageOffset / 10f) / 100f;
            player.GetAttackSpeed<GenericDamageClass>() -= (Adrenaline.SpeedOffset / 10f) / 100f;
            player.GetKnockback<GenericDamageClass>() -= (Adrenaline.KnockbackOffset / 10f) / 100f;
        }
    }
}
