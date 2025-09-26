using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace VanillaModding.Content
{
    internal class VanillaModdingPlayer : ModPlayer
    {
        // This variable will hold the dynamic value for your buff.
        public bool rolling;
        public bool hasAnyDiceEffect;
        public int DiceMult;
        public int totalRolls;

        // The ResetEffects hook is important for buffs to work correctly.
        // It resets the effects applied by your buff when it expires.
        public override void ResetEffects()
        {
            hasAnyDiceEffect = false;
        }

        public void ResetDice()
        {
            totalRolls = 0;
            DiceMult = 0;
            rolling = false;
            hasAnyDiceEffect = false;
        }

        public override void OnEnterWorld()
        {
            ResetDice();
            base.OnEnterWorld();
        }

        public override void UpdateDead()
        {
            ResetDice();
            base.UpdateDead();
        }

        public override void OnRespawn()
        {
            ResetDice();
            base.OnRespawn();
        }
    }
}
