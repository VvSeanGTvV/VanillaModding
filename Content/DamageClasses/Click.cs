using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace VanillaModding.Content.DamageClasses
{
    internal class Click : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;

            return new StatInheritanceData(
                damageInheritance: 1f,
                critChanceInheritance: 0.75f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0.44f,
                knockbackInheritance: 1f
            );
        }

        public override void SetDefaultStats(Player player)
        {
            // This method lets you set default statistical modifiers for your example damage class.
            // Here, we'll make our example damage class have more critical strike chance and armor penetration than normal.
            player.GetCritChance<Click>() += 2;
            player.GetArmorPenetration<Click>() += 2;
            player.GetDamage<Click>() += 1f;
            player.GetKnockback<Click>() += 1f;
            // These sorts of modifiers also exist for damage (GetDamage), knockback (GetKnockback), and attack speed (GetAttackSpeed).
            // You'll see these used all around in reference to vanilla classes and our example class here. Familiarize yourself with them.
        }
    }
}
