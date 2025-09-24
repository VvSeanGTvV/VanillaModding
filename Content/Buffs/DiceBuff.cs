using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaModding.Content.Buffs
{
    internal class DiceBuff : ModBuff
    {
        public static readonly int DefenseBonus = 10;
        public static readonly int LifeBonus = 10;
        public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus, LifeBonus);

        public override void Update(Player player, ref int buffIndex)
        {
            DynamicDiceBuff modPlayer = player.GetModPlayer<DynamicDiceBuff>();
            modPlayer.hasDiceBuff = true; // Set the flag to true to indicate that the buff is active.
            player.statDefense += DefenseBonus * modPlayer.DiceMult; // Grant a +10 defense boost to the player while the buff is active.
            player.statLifeMax2 += LifeBonus * modPlayer.DiceMult; // Grant a +20 max life boost to the player while the buff is active.
        }
    }

    public class DynamicDiceBuff : ModPlayer
    {
        // This variable will hold the dynamic value for your buff.
        public bool hasDiceBuff;
        public int DiceMult;

        // The ResetEffects hook is important for buffs to work correctly.
        // It resets the effects applied by your buff when it expires.
        public override void ResetEffects()
        {
            hasDiceBuff = false;
            DiceMult = 1;
        }
    }
}
