using System;
using System.Collections.Generic;
using System.IO;
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
        public static readonly int ManaBonus = 10;
        public override LocalizedText Description => base.Description.WithFormatArgs(DefenseBonus, LifeBonus, ManaBonus);

        public override void Update(Player player, ref int buffIndex)
        {
            DynamicDiceBuff modPlayer = player.GetModPlayer<DynamicDiceBuff>();
            modPlayer.hasDiceBuff = true; // Set the flag to true to indicate that the buff is active.
            player.statDefense += DefenseBonus * modPlayer.DiceMult; // Grant a +10 * dice = Defense boost to the player while the buff is active.
            player.statLifeMax2 += LifeBonus * modPlayer.DiceMult; // Grant a +10 * dice = Life boost to the player while the buff is active.
            player.statManaMax2 += ManaBonus * modPlayer.DiceMult; // Grant a +10 * dice = Mana boost to the player while the buff is active.
            //Logging.PublicLogger.Info($"Buff is active. Current Dice Multiplier: {modPlayer.DiceMult}");
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
        }
    }
}
