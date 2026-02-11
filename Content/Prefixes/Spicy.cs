using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace VanillaModding.Content.Prefixes
{
    internal class Spicy : ModPrefix
    {
        public LocalizedText AddonEffects => this.GetLocalization(nameof(AddonEffects));
        public virtual float Power => 1f;
        public virtual int tier => 0;

        public override PrefixCategory Category => PrefixCategory.AnyWeapon;
        public override float RollChance(Item item)
        {
            return 4f + Power + tier;
        }
        public override bool CanRoll(Item item)
        {
            DamageClass currentClass = item.DamageType;
            return (currentClass == DamageClass.Melee ||
                currentClass == DamageClass.MeleeNoSpeed ||
                currentClass == DamageClass.SummonMeleeSpeed
                );
        }

        // Use this function to modify these stats for items which have this prefix:
        // Damage Multiplier, Knockback Multiplier, Use Time Multiplier, Scale Multiplier (Size), Shoot Speed Multiplier, Mana Multiplier (Mana cost), Crit Bonus.
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            //damageMult *= 1f + 0.0414f * Power;
            useTimeMult *= 1f - 0.135f * Power;
            //knockbackMult *= 1f + 0.0414f * Power;
            critBonus += (int)Power*2;
        }

        // Modify the cost of items with this modifier with this function.
        public override void ModifyValue(ref float valueMult)
        {
            valueMult *= 1f + 0.0215f * Power;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            // This localization is not shared with the inherited classes. ExamplePrefix and ExampleDerivedPrefix have their own translations for this line.
            yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AddonEffects.Value)
            {
                IsModifier = true,
            };
        }

        public override void Apply(Item item)
        {
            item.rare += tier;
            base.Apply(item);
        }
    }
}
