using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VanillaModding.Content.Items.Weapon.Throwable.Redfish;

namespace VanillaModding.Content.NPCs.Mewgenics
{
    internal class Fatso : ModNPC
    {
        public Rectangle frameEye;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override bool CanChat() => true;
        public override string GetChat()
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<RedFish>() && Main.rand.NextBool(50))
                switch (Main.rand.Next(4))
                {
                    case 0: return "*grr*";
                    case 1: return "*angry*";
                    case 2: return "*stares at fish intensely*";
                    default: return "*silent*";
                }

            if (Main.rand.NextBool(50))
                switch (Main.LocalPlayer.HeldItem.type)
                {
                    case ItemID.Bass:
                    case ItemID.Goldfish:
                        switch (Main.rand.Next(4))
                        {
                            case 0: return "*stomach gurgles*";
                            case 1: return "*purrs*";
                            case 2: return "*stares at fish intensely*";
                            default: return "*stares at fish too intensely*";
                        }
                }

            switch (Main.rand.Next(4))
            {
                case 0: return "Meow.";
                case 1: return "*purrs softly*";
                case 2: return "*stares at you judgmentally*";
                default: return "Mrrrow?";
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool isNearTownPlayer = spawnInfo.Player.townNPCs > 2f;
            if (!spawnInfo.Player.ZoneOverworldHeight || !isNearTownPlayer || NPC.AnyNPCs(Type))
                return 0f;

            return 1f;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = false;
            NPC.friendly = true;
            NPC.width = 48;
            NPC.height = 50;
            NPC.lifeMax = 50;

            NPC.scale = 0.75f;
            frameEye = new Rectangle(0, 0, NPC.width, NPC.height);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            if (NPC.velocity.Y == 0) NPC.velocity.X = 0;

            base.AI();
        }

        int currentFrame = 0;
        bool flickEar = false;
        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 2;

            // Frame
            int frameSpeed = 15;
            NPC.frameCounter++;
            if (NPC.frameCounter > frameSpeed)
            {
                currentFrame++;
                if (!flickEar) flickEar = Main.rand.NextBool(50); else
                {
                    currentFrame = 2;
                    flickEar = false;
                }
                NPC.frameCounter = 0;
            }

            if (flickEar) currentFrame = 3;
            if (currentFrame > finalFrame && !flickEar) currentFrame = startFrame;

            NPC.frame.Y = currentFrame * frameHeight;

            // Have eye eye yehye
            FindFrameEyes(frameHeight);
        }

        bool blinked = false;
        int blinkCounter = 0;
        public void FindFrameEyes(int frameHeight)
        {
            if (!blinked) blinked = Main.rand.NextBool(100);
            else blinkCounter++;
            if (blinkCounter >= 15)
            {
                blinked = false;
                blinkCounter = 0;
            }
            frameEye.Y = 0 + ((blinked) ? frameHeight : 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float eyeMoveLimit = 2f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D facetexture = (Texture2D)ModContent.Request<Texture2D>(nameof(VanillaModding) + "/" + (ModContent.Request<Texture2D>(Texture).Name + "_face").Replace(@"\", "/"));
            SpriteEffects effects = (NPC.direction == 1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new(NPC.frame.Width / 2, NPC.frame.Height / 2);
            Vector2 originface = new(frameEye.Width / 2, frameEye.Height / 2);
            //Vector2 currPos = new Vector2(NPC.position.X - Main.screenPosition.X + NPC.width / 2 - texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale);
            
            Player player = Main.player[NPC.target];
            Vector2 toPlayer = player.Center - NPC.Center;

            if (toPlayer != Vector2.Zero)
                toPlayer.Normalize();

            Vector2 eyeOffset = toPlayer * eyeMoveLimit;

            Vector2 position = NPC.Center - Main.screenPosition;
            position = new Vector2(position.X, position.Y + 2f);

            Main.spriteBatch.Draw(texture, new Vector2(position.X, position.Y+1), NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            Main.spriteBatch.Draw(facetexture, position+eyeOffset, frameEye, drawColor, NPC.rotation, originface, NPC.scale, effects, 0f);
            return false;
        }
    }
}
