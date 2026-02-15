using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using VanillaModding.Common;
using VanillaModding.Common.GlobalNPCs;

namespace VanillaModding.Content.Buffs
{
    internal class Stunned : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            StunnedNPC stunNPC = npc.GetGlobalNPC<StunnedNPC>();

            if (npc.noGravity) npc.velocity = Vector2.Zero;
            else
            {
                float velY = npc.velocity.Y;
                if (npc.velocity.Y < 0) velY = 0;
                npc.velocity = new Vector2(0, velY);
            }

            npc.frameCounter = 0;
            stunNPC.stunned = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            VanillaModdingPlayer modPlayer = player.GetModPlayer<VanillaModdingPlayer>();
            //player.velocity = new Vector2(0, 6);
            float velY = player.velocity.Y;
            if (player.velocity.Y < 0) velY = 0;
            player.velocity = new Vector2(0, velY);
            modPlayer.stunned = true;
        }
    }
}
