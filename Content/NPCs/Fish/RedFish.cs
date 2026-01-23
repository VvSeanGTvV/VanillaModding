using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Common.Systems;

using VanillaModding.External.AI;

namespace VanillaModding.Content.NPCs.Fish
{
    internal class RedFish : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        private float bobTimer;
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 22;

            NPC.defense = 5;
            NPC.lifeMax = 45000;
            NPC.friendly = true;
            NPC.catchItem = ModContent.ItemType<Items.Weapon.Throwable.Redfish.RedFish>();

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 10);
            NPC.aiStyle = -1;

        }

        public override void AI()
        {
            if (NPC.wet)
            {
                FindFrame(NPC.height);
                bobTimer += 0.05f;

                // Vertical bobbing
                float bobAmount = (float)Math.Sin(bobTimer) * 0.6f;
                NPC.velocity = new Microsoft.Xna.Framework.Vector2(NPC.spriteDirection * 1.2f, bobAmount);

                if (NPC.collideY) bobTimer = 0;
                if (NPC.collideX) FlipFish();
            }
        }

        private void FlipFish()
        {
            NPC.direction *= -1;
            NPC.spriteDirection = NPC.direction;
            NPC.velocity.X *= -1;
        }

        public override void FindFrame(int frameHeight)
        {
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
            int startFrame = 0;
            int finalFrame = 1;

            // Frame
            NPC.frameCounter++;
            if (NPC.frameCounter >= 30)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.frame.Y > finalFrame * frameHeight)
            {
                NPC.frame.Y = startFrame * frameHeight;
            }
        }
    }
}
