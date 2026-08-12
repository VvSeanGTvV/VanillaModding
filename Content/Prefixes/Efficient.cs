using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaModding.Content.Prefixes
{
    internal class Efficient : ModPrefix
    {
        public LocalizedText PositiveEffects => this.GetLocalization(nameof(PositiveEffects));
        public virtual float Power => 1f;
        public virtual int tier => 1;
        public override PrefixCategory Category => PrefixCategory.Magic;
        public override float RollChance(Item item)
        {
            return 2f + Power + tier;
        }
        public override bool CanRoll(Item item)
        {
            return true;
        }

        // Use this function to modify these stats for items which have this prefix:
        // Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            manaMult *= 1f - 0.311f * Power;
            damageMult *= 1f - 0.311f * Power;
            useTimeMult *= 1f - 0.462f * Power;
            knockbackMult *= 1f - 0.314f * Power;
            //critBonus -= (int)Power * 2;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.0335f * Power;
        }

        /*public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {

        }*/

        public override void Apply(Item item)
        {
            item.rare += tier;
            base.Apply(item);
        }
    }
}
