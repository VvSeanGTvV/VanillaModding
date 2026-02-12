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
    internal class Colossal : ModPrefix
    {
        public LocalizedText NegativeEffects => this.GetLocalization(nameof(NegativeEffects));
        public virtual float Power => 1f;
        public virtual int tier => -1;

        public override PrefixCategory Category => PrefixCategory.Melee;
        public override float RollChance(Item item)
        {
            return 4f + Power + tier;
        }
        public override bool CanRoll(Item item)
        {
            return true;
        }

        // Use this function to modify these stats for items which have this prefix:
        // Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            scaleMult *= 1f + 0.55f * Power;
            damageMult *= 1f + 0.651f * Power;
            useTimeMult *= 1f + 0.622f * Power;
            knockbackMult *= 1f + 0.214f * Power;
            critBonus += (int)Power * 5;
        }

        // Modify the cost of items with this modifier with this function.
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.0335f * Power;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            // This localization is not shared with the inherited classes. ExamplePrefix and ExampleDerivedPrefix have their own translations for this line.
            yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", NegativeEffects.Value)
            {
                IsModifier = true,
                IsModifierBad = true,
            };
        }

        public override void Apply(Item item)
        {
            item.rare += tier;
            base.Apply(item);
        }
    }
}
