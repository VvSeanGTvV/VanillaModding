using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;
using VanillaModding.Content.NPCs.DeliveryDrone;

namespace VanillaModding.Content.Achievements
{
    internal class NotFriendlyDrone : ModAchievement
    {
        public CustomIntCondition Condition { get; private set; }

        // This achievement is "hidden", meaning its name and description will both appear as "???" in the achievements menu, until at least 1 ExampleWormHead has been killed.
        public override bool Hidden => Condition.Value == 0;

        public override void SetStaticDefaults()
        {
            Condition = AddIntCondition(100);
            AchievementsHelper.OnNPCKilled += NPCKilledListener;
        }

        public override void Unload()
        {
            AchievementsHelper.OnNPCKilled -= NPCKilledListener;
        }

        private void NPCKilledListener(Player player, short npcId)
        {
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }

            if (npcId == ModContent.NPCType<DeliveryDrone>())
            {
                // Int conditions will automatically complete once you've incremented it enough. There is no need to call the Complete method manually.
                Condition.Value++;
            }
        }
    }
}
