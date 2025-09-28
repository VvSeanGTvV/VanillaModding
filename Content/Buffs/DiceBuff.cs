using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Consumable;

namespace VanillaModding.Content.Buffs
{
    internal class DiceBuff : ModBuff
    {
        public static readonly int DefenseBonus = 10;
        public static readonly int LifeBonus = 10;
        public static readonly int ManaBonus = 10;
        public override LocalizedText Description => base.Description.WithFormatArgs(0);

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            modPlayer.hasAnyDiceEffect = true; // Set the flag to true to indicate that the buff is active.
            player.statDefense += modPlayer.DiceMult + modPlayer.DiceMult; // Grant a +10 * dice = Defense boost to the player while the buff is active.
            player.statLifeMax2 += modPlayer.DiceMult * modPlayer.DiceMult * modPlayer.DiceMult; // Grant a +10 * dice = Life boost to the player while the buff is active.
            player.statManaMax2 += modPlayer.DiceMult * modPlayer.DiceMult * modPlayer.DiceMult; // Grant a +10 * dice = Mana boost to the player while the buff is active.
            //Logging.PublicLogger.Info($"Buff is active. Current Dice Multiplier: {modPlayer.DiceMult}");
        }
    }

    internal class DiceDebuff : ModBuff
    {
        public static readonly int DefenseBonus = 10;
        public static readonly int LifeBonus = 10;
        public static readonly int ManaBonus = 10;
        public override LocalizedText Description => base.Description.WithFormatArgs(0);

        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            modPlayer.hasAnyDiceEffect = true; // Set the flag to true to indicate that the buff is active.
            player.statDefense -= modPlayer.DiceMult + modPlayer.DiceMult; // Grant a +10 * dice = Defense boost to the player while the buff is active.
            player.statLifeMax2 = Math.Max(player.statLifeMax2  - modPlayer.DiceMult * modPlayer.DiceMult, 0); // Grant a +10 * dice = Life boost to the player while the buff is active.
            player.statManaMax2 = Math.Max(player.statManaMax2 - modPlayer.DiceMult * modPlayer.DiceMult, 0); // Grant a +10 * dice = Mana boost to the player while the buff is active.
            //Logging.PublicLogger.Info($"Buff is active. Current Dice Multiplier: {modPlayer.DiceMult}");
            if (player.statLifeMax2 < 1) {
                player.KillMe(PlayerDeathReason.ByCustomReason(Dice.RanOutofHealth.ToNetworkText()), 10.0, 0);
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
