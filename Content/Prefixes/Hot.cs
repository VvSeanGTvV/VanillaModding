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
    internal class Hot : Spicy
    {
        public LocalizedText NegativeEffects => this.GetLocalization(nameof(NegativeEffects));
        public override float Power => base.Power + 3.35f;
        public override int tier => 1;
        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            // This localization is not shared with the inherited classes. ExamplePrefix and ExampleDerivedPrefix have their own translations for this line.
            yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", AddonEffects.Value)
            {
                IsModifier = true,
            };
            // This localization is not shared with the inherited classes. ExamplePrefix and ExampleDerivedPrefix have their own translations for this line.
            yield return new TooltipLine(Mod, "PrefixWeaponAwesomeDescription", NegativeEffects.Value)
            {
                IsModifier = true,
                IsModifierBad = true,
            };
        }

    }
}
